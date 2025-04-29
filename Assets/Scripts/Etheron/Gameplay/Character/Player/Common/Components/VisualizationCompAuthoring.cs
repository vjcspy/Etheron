using Etheron.Core.Component;
using Etheron.Core.XMachine;
namespace Etheron.Gameplay.Character.Player.Common.Components
{
    public class VisualizationCompAuthoring : XCompAuthoring
    {

        protected override void Authoring(XMachineEntity xMachineEntity)
        {
            xMachineEntity.AddXComponent(
                component: new VisualizationCompData());
            xMachineEntity.RegisterXCompSystem(system: new VisualizationCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
