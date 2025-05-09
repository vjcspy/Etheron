﻿using Etheron.Core.XComponent;
using UnityEngine;
namespace Etheron.Colyseus.Components.Map.ServerClient.Player.ServerPlayerVisualization
{
    public class ServerPlayerVisualizationCompAuthoring : XCompAuthoring
    {
        [SerializeField] private int updateIntervalMs = 100;

        protected override void Authoring()
        {
            AddComponentData(component: new ServerPlayerVisualizationCompData
            {
                updateIntervalMs = updateIntervalMs
            });
            AddSystem(new ServerPlayerVisualizationCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
