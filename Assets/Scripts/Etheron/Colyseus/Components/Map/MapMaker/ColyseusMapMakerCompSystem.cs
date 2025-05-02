using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
namespace Etheron.Colyseus.Components.Map.MapMaker
{
    /*
     * ColyseusMapMakerCompSystem is a system that handles the map maker component of the Colyseus game.
     * Mỗi khi vào scene, dựa vào mapName to know where to load the map.
     *
     * But, there is a case that user want to reconnect to the last instance of the map.
     * TODO: Add reconnect mechanism.
     */
    public class ColyseusMapMakerCompSystem : XCompSystem
    {

        public ColyseusMapMakerCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity)
        {
        }
        public override void OnCreate()
        {
        }
        public override void Update()
        {
        }
        public override void OnDestroy()
        {
        }
    }
}
