using Etheron.Core.XComponent;
using UnityEngine;
namespace Etheron.Colyseus.Components.Map.ServerClient.Monster.ServerMonsterVisualization
{
    public class ServerMonsterVisualizationCompAuthoring : XCompAuthoring
    {

        [SerializeField] private int updateIntervalMs = 100;

        protected override void Authoring()
        {
            AddComponentData(component: new ServerMonsterVisualizationCompData
            {
                updateIntervalMs = updateIntervalMs
            });
            AddSystem(system: new ServerMonsterVisualizationCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
