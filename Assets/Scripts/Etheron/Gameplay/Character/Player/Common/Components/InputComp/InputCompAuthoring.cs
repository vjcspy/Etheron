using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
namespace Etheron.Gameplay.Character.Player.Common.Components.InputComp
{
    public class InputCompAuthoring : XCompAuthoring
    {
        protected override void Authoring(XMachineEntity xMachineEntity)
        {
            xMachineEntity.AddComponentData(
                component: new InputCompData());
            xMachineEntity.AddSystem(system: new InputCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
