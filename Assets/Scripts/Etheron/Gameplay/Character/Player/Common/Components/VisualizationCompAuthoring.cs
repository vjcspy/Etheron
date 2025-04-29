using Etheron.Core.Component;
using Etheron.Core.XMachine;
using Etheron.Types;
namespace Etheron.Gameplay.Character.Player.Common.Components
{
    public class VisualizationCompAuthoring : XCompAuthoring
    {

        protected override void Authoring(XMachineEntity xMachineEntity)
        {
            xMachineEntity.AddXComponent(
                component: new VisualizationCompData
                {
                    animationState = 0,
                    facingDirection = FacingDirection.Front
                });
            xMachineEntity.RegisterXCompSystem(system: new VisualizationCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
