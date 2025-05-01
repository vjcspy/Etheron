using Etheron.Core.XComponent;
using System;
using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components.GroundDetectionComp
{
    public class GroundDetectionCompAuthoring : XCompAuthoring
    {

        [SerializeField] private GroundDetectionCheckType checkType = GroundDetectionCheckType.Raycast;
        [SerializeField] private float checkDistance = 0.7f;
        [SerializeField] private float sphereRadius = 0.1f;
        [SerializeField] private Vector3 checkOffset = Vector3.zero;
        [SerializeField] private LayerMask groundLayer;

        private void OnDrawGizmosSelected()
        {
            Vector3 origin = transform.position + checkOffset;

            Gizmos.color = Color.green;
            switch (checkType)
            {
                case GroundDetectionCheckType.Raycast:
                    Gizmos.DrawLine(from: origin, to: origin + Vector3.down * checkDistance);
                    break;
                case GroundDetectionCheckType.SphereCast:
                    // Gizmos.DrawWireSphere(center: origin, radius: sphereRadius);
                    Gizmos.DrawWireSphere(center: origin + Vector3.down * checkDistance, radius: sphereRadius);
                    Gizmos.DrawLine(from: origin + Vector3.left * sphereRadius, to: origin + Vector3.down * checkDistance + Vector3.left * sphereRadius);
                    Gizmos.DrawLine(from: origin + Vector3.right * sphereRadius, to: origin + Vector3.down * checkDistance + Vector3.right * sphereRadius);
                    Gizmos.DrawLine(from: origin + Vector3.forward * sphereRadius, to: origin + Vector3.down * checkDistance + Vector3.forward * sphereRadius);
                    Gizmos.DrawLine(from: origin + Vector3.back * sphereRadius, to: origin + Vector3.down * checkDistance + Vector3.back * sphereRadius);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void Authoring()
        {
            AddComponentData(
                component: new GroundDetectionCompData
                {
                    checkType = checkType,
                    checkDistance = checkDistance,
                    sphereRadius = sphereRadius,
                    checkOffset = checkOffset,
                    groundLayer = groundLayer,

                    isGrounded = false
                }
            );
            AddSystem(system: new GroundDetectionCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
