using Cysharp.Threading.Tasks;
using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using Etheron.Gameplay.Character.Player.Common.Components.VisualizationComp;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Etheron.Colyseus.Components.Map.ClientServer.Player.LocalSync
{
    public class LocalSyncCompSystem : XCompSystem
    {
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
            SyncLoopAsync(millisecondsDelay: _localSyncCompStorage.Get().syncIntervalMs).Forget();
        }

        public override void Update()
        {
        }

        public override void OnDestroy()
        {
            _isRunning = false;
        }

        private async UniTaskVoid SyncLoopAsync(int millisecondsDelay)
        {
            while (_isRunning)
            {
                if (_localSyncCompStorage.IsEnable())
                {
                    VisualizationCompData visualizationComp = _visualizationCompStorage.Get();
                    Vector3 velocity = _rb.linearVelocity;

                    if (_colyseusManager.currentMapRoom != null)
                    {
                        await _colyseusManager.currentMapRoom.Send(type: "local_sync", message: new
                        {
                            velocity.x,
                            velocity.y,
                            velocity.z,
                            facingDirection = new
                            {
                                visualizationComp.facingDirection.x,
                                visualizationComp.facingDirection.y,
                                visualizationComp.facingDirection.z
                            },
                            visualizationComp.animationState
                        });
                    }
                }

                await UniTask.Delay(millisecondsDelay: millisecondsDelay); // mỗi 100ms
            }
        }
    }
}
