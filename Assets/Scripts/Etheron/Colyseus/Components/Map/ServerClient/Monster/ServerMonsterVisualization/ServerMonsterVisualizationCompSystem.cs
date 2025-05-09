﻿using Etheron.Colyseus.Components.Map.ServerClient.Base;
using Etheron.Colyseus.Components.Map.ServerClient.Player.ServerPlayerVisualization;
using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using UnityEngine;
namespace Etheron.Colyseus.Components.Map.ServerClient.Monster.ServerMonsterVisualization
{
    public class ServerMonsterVisualizationCompSystem : ServerEntityFastVisualizationSystem<Schemas.Monster>
    {
        private readonly XCompStorage<ServerMonsterVisualizationCompData> _storage;
        public ServerMonsterVisualizationCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity)
        {
            _storage = GetStorage<ServerMonsterVisualizationCompData>();
        }

        protected override bool TryGetEntityState(out Schemas.Monster entityState)
        {
            string serverSyncId = _storage.Get().serverSyncId;
            if (string.IsNullOrEmpty(value: serverSyncId))
            {
                entityState = null;
                return false;
            }

            return _colyseusManager.currentMapRoom.State.monsters.TryGetValue(key: serverSyncId, value: out entityState);
        }

        protected override bool TryExtractInterpolationTarget(Schemas.Monster state, out InterpolationTarget target)
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
