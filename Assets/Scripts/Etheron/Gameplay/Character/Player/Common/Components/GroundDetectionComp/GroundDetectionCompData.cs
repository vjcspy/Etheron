using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components.GroundDetectionComp
{
    public enum GroundDetectionCheckType
    {
        Raycast,
        SphereCast
    }

    public struct GroundDetectionCompData
    {
        public GroundDetectionCheckType checkType;
        public float checkDistance;
        public float sphereRadius;
        public Vector3 checkOffset;
        public LayerMask groundLayer;

        public bool isGrounded;
    }
}
