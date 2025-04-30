using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components.VisualizationComp
{
    public enum PlayerAnimationStateParam
    {
        Idle = 0,
        Walking = 1,
        Running = 2,
        Air = 5
    }

    public struct VisualizationCompData
    {
        public int animationUpdateIntervalMs;
        public int animationState;
        public Vector3 facingDirection;
    }
}
