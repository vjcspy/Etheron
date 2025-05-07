using Cysharp.Threading.Tasks;
using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using Etheron.Gameplay.Character.Player.Common.Components.VisualizationComp;
using Etheron.Utils.Thread;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Etheron.Colyseus.Components.Map.ClientServer.Player.LocalSync
{
    public class LocalSyncCompSystem : XCompSystem
    {
        private readonly ThreadSafeStructValue<LocalSyncData> _localSyncData = new ThreadSafeStructValue<LocalSyncData>();

        private ColyseusManager _colyseusManager;
        private bool _isRunning;
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
            Task.Factory.StartNew(
                action: () => SyncLoop(millisecondsDelay: syncIntervalMs).Forget(),
                cancellationToken: CancellationToken.None,
                creationOptions: TaskCreationOptions.LongRunning,
                scheduler: TaskScheduler.Default
            );
        }

        private async UniTask SyncLoop(int millisecondsDelay)
        {
            while (_isRunning)
            {
                if (_localSyncCompStorage.IsEnable() && _colyseusManager.currentMapRoom != null)
                {
                    LocalSyncData localSyncData = _localSyncData.Get();

                    await _colyseusManager.currentMapRoom.Send(type: "local_sync", message: new
                    {
                        position = new
                        {
                            localSyncData.position.x,
                            localSyncData.position.y,
                            localSyncData.position.z,
                            localSyncData.timestamp
                        },
                        facingDirection = new
                        {
                            localSyncData.facingDirection.x,
                            localSyncData.facingDirection.y,
                            localSyncData.facingDirection.z
                        },
                        localSyncData.animationState
                    });
                }

                await UniTask.Delay(millisecondsDelay: millisecondsDelay);
            }
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
