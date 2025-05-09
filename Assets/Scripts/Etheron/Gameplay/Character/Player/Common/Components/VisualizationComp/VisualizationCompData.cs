﻿using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components.VisualizationComp
{
    public enum PlayerAnimationStateParam
    {
        Idle = 0,
        Walking = 1,
        Running = 2,
        Jump = 5,
        Fall = 6,
    }

    public struct VisualizationCompData
    {
        public int animationUpdateIntervalMs;
        public int animationState;
        public Vector3 facingDirection;
    }
}
