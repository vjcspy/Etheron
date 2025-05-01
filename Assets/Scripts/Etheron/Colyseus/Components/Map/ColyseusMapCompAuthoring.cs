using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using UnityEngine;
namespace Etheron.Colyseus.Components.Map
{
    public class ColyseusMapCompAuthoring : XCompAuthoring
    {
        [SerializeField] private string mapName;
        protected override void Authoring(XMachineEntity xMachineEntity)
        {
            ColyseusMapCompData colyseusMapCompData = new ColyseusMapCompData
            {
                mapName = mapName
            };
            xMachineEntity.AddComponentData(component: colyseusMapCompData);
            xMachineEntity.AddSystem(system: new ColyseusMapCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
