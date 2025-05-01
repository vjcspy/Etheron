using Etheron.Core.XComponent;
using UnityEngine;
namespace Etheron.Colyseus.Components.Map
{
    public class ColyseusMapCompAuthoring : XCompAuthoring
    {
        [SerializeField] private string mapName;
        protected override void Authoring()
        {
            AddComponentData(component: new ColyseusMapCompData
            {
                mapName = mapName
            });
            AddSystem(system: new ColyseusMapCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
