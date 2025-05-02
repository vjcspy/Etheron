using Cysharp.Threading.Tasks;
using Etheron.Colyseus;
using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using Etheron.Utils;
using UnityEngine;
namespace Etheron.Gameplay.Character.ServerPlayer.Components.VisualizationComp
{
    public class ServerPlayerVisualizationCompSystem : XCompSystem
    {
        private ColyseusManager _colyseusManager;
        private bool _isRunning;
        private Rigidbody _rb;
        private XCompStorage<ServerPlayerVisualizationCompData> _serverPlayerVisualizationStorage;

        public ServerPlayerVisualizationCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity) { }

        public override void OnCreate()
        {
            _serverPlayerVisualizationStorage = GetStorage<ServerPlayerVisualizationCompData>();
            _colyseusManager = ColyseusManager.Instance;
            _rb = GetComponent<Rigidbody>();
            _isRunning = true;
            SyncLoop(millisecondsDelay: _serverPlayerVisualizationStorage.Get().updateIntervalMs).Forget(); // chạy async
        }

        private async UniTaskVoid SyncLoop(int millisecondsDelay)
        {
            while (_isRunning)
            {
                if (_serverPlayerVisualizationStorage.IsEnable() &&
                    _colyseusManager.currentMapRoom != null &&
                    _colyseusManager.currentMapRoom.State?.players != null)
                {
                    ServerPlayerVisualizationCompData data = _serverPlayerVisualizationStorage.Get();
                    if (string.IsNullOrEmpty(value: data.sessionId)) continue;
                    ELogger.Log(message: "[ServerPlayerVisualizationCompSystem] SyncLoop: " + data.sessionId);
                    // if (_colyseusManager.currentMapRoom.State.players.TryGetValue(key: data.sessionId, value: out Colyseus.Schemas.Player playerState) && playerState != null)
                    // {
                    //     _rb.transform.position = new Vector3(
                    //         x: playerState.position.x,
                    //         y: playerState.position.y,
                    //         z: playerState.position.z
                    //     );
                    // }
                }

                await UniTask.Delay(millisecondsDelay: millisecondsDelay);
            }
        }

        public override void Update() { }

        public override void OnDestroy()
        {
            _isRunning = false;
        }
    }
}
