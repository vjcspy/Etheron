using Etheron.Colyseus.Components.Map.ServerClient.Base;
using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using UnityEngine;
namespace Etheron.Colyseus.Components.Map.ServerClient.Player.ServerPlayerVisualization
{
    public class ServerPlayerVisualizationCompSystem : ServerEntityFastVisualizationSystem<Schemas.Player>
    {
        private readonly XCompStorage<ServerPlayerVisualizationCompData> _storage;
        public ServerPlayerVisualizationCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity)
        {
            _storage = GetStorage<ServerPlayerVisualizationCompData>();
        }

        protected override bool TryGetEntityState(out Schemas.Player entityState)
        {
            string sessionId = _storage.Get().sessionId;
            if (string.IsNullOrEmpty(value: sessionId))
            {
                entityState = null;
                return false;
            }

            return _colyseusManager.currentMapRoom.State.players.TryGetValue(key: sessionId, value: out entityState);
        }

        protected override bool TryExtractInterpolationTarget(Schemas.Player state, out InterpolationTarget target)
        {
            target = new InterpolationTarget
            {
                position = new Vector3(x: state.position.value.x, y: state.position.value.y, z: state.position.value.z),
                timestamp = state.position.timestamp,
                facingDirection = new Vector3(x: state.position.facingDirection.x, y: state.position.facingDirection.y, z: state.position.facingDirection.z),
                animationState = state.visualization.state
            };
            return true;
        }
    }
}
