using Etheron.Core.Component;
using Etheron.Core.XMachine;
using Etheron.Types;
using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components
{
    public class VisualizationCompAuthoring : XCompAuthoring
    {
        [SerializeField] private int animationUpdateIntervalMs = 50;
        protected override void Authoring(XMachineEntity xMachineEntity)
        {
            xMachineEntity.AddXComponent(
                component: new VisualizationCompData
                {
                    animationUpdateIntervalMs = animationUpdateIntervalMs,
                    animationState = 0,
                    facingDirection = FacingDirection.Front
                });
            xMachineEntity.RegisterXCompSystem(system: new VisualizationCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
