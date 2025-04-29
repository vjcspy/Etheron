using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components
{
    public enum GroundDetectionCheckType
    {
        Raycast,
        SphereCast
    }

    public struct GroundDetectionCompData
    {
        public GroundDetectionCheckType checkType;
        public bool isGrounded;
        public LayerMask groundLayer;
    }
}
