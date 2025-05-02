using Etheron.Core.XComponent;
using UnityEngine;
namespace Etheron.Gameplay.Character.ServerPlayer.Components.VisualizationComp
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
