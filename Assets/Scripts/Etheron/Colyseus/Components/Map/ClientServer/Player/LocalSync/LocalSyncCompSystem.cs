using Cysharp.Threading.Tasks;
using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using Etheron.Gameplay.Character.Player.Common.Components.VisualizationComp;
using Etheron.Utils.Thread;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Etheron.Colyseus.Components.Map.ClientServer.Player.LocalSync
{
    public class LocalSyncCompSystem : XCompSystem
    {
        private readonly ThreadSafeStructValue<LocalSyncData> _localSyncData = new ThreadSafeStructValue<LocalSyncData>();

        private ColyseusManager _colyseusManager;
        private bool _isRunning;
        private LocalSyncData _lastSyncedData;
        private XCompStorage<LocalSyncCompData> _localSyncCompStorage;

        private Rigidbody _rb;
        private XCompStorage<VisualizationCompData> _visualizationCompStorage;

        public LocalSyncCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity)
        {
        }

        public override void OnCreate()
        {
            _localSyncCompStorage = GetStorage<LocalSyncCompData>();
            _visualizationCompStorage = GetStorage<VisualizationCompData>();
            _colyseusManager = ColyseusManager.Instance;
            _rb = GetComponent<Rigidbody>();

            _isRunning = true;

            LocalSyncCompData config = _localSyncCompStorage.Get();
            RunSyncLoopInBackground(syncIntervalMs: config.serverSyncIntervalMs);
        }

        public override void Update()
        {
            if (!_localSyncCompStorage.IsEnable()) return;

            VisualizationCompData visualizationComp = _visualizationCompStorage.Get();
            Vector3 position = _rb.transform.position;

            _localSyncData.Set(newValue: new LocalSyncData
            {
                position = position,
                timestamp = Time.time,
                facingDirection = visualizationComp.facingDirection,
                animationState = visualizationComp.animationState
            });
        }

        public override void OnDestroy()
        {
            _isRunning = false;
        }

        private void RunSyncLoopInBackground(int syncIntervalMs)
        {
            // UniTask.RunOnThreadPool là cách đúng để chạy task background trên desktop/mobile
            UniTask.RunOnThreadPool(action: async () =>
            {
                await SyncLoop(millisecondsDelay: syncIntervalMs);
            });
        }

        private async UniTask SyncLoop(int millisecondsDelay)
        {
            while (_isRunning)
            {
                if (_localSyncCompStorage.IsEnable() && _colyseusManager.currentMapRoom != null)
                {
                    LocalSyncData currentData = _localSyncData.Get();

                    // Kiểm tra thay đổi để tránh gửi dữ liệu không cần thiết
                    if (HasChanged(current: currentData))
                    {
                        await _colyseusManager.currentMapRoom.Send(type: "local_sync", message: new
                        {
                            position = new
                            {
                                currentData.position.x,
                                currentData.position.y,
                                currentData.position.z,
                                currentData.timestamp
                            },
                            facingDirection = new
                            {
                                currentData.facingDirection.x,
                                currentData.facingDirection.y,
                                currentData.facingDirection.z
                            },
                            currentData.animationState
                        });

                        _lastSyncedData = currentData;
                    }
                }

                await UniTask.Delay(millisecondsDelay: millisecondsDelay);
            }
        }

        private bool HasChanged(LocalSyncData current)
        {
            return current.position != _lastSyncedData.position ||
                current.facingDirection != _lastSyncedData.facingDirection ||
                current.animationState != _lastSyncedData.animationState;
        }
    }

    public struct LocalSyncData
    {
        public Vector3 position;
        public float timestamp;
        public Vector3 facingDirection;
        public int animationState;
    }
}
