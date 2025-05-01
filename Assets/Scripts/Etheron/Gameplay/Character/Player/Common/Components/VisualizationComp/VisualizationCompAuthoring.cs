using Etheron.Core.XComponent;
using Etheron.Types;
using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components.VisualizationComp
{
    public class VisualizationCompAuthoring : XCompAuthoring
    {
        [SerializeField] private int animationUpdateIntervalMs = 50;
        protected override void Authoring()
        {
            AddComponentData(
                component: new VisualizationCompData
                {
                    animationUpdateIntervalMs = animationUpdateIntervalMs,
                    animationState = 0,
                    facingDirection = FacingDirection.Front
                });
            AddSystem(system: new VisualizationCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
