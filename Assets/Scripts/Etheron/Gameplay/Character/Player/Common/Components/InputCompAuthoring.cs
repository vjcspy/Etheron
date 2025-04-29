using Etheron.Core.Component;
using Etheron.Core.XMachine;
namespace Etheron.Gameplay.Character.Player.Common.Components
{
    public class InputCompAuthoring : XCompAuthoring
    {
        protected override void Authoring(XMachineEntity xMachineEntity)
        {
            xMachineEntity.AddXComponent(
                component: new InputCompData());
            xMachineEntity.RegisterXCompSystem(system: new InputCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
