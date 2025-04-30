using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components.GroundDetectionComp
{


    public class GroundDetectionCompSystem : XCompSystem
    {
        private XCompStorage<GroundDetectionCompData> _groundDetectionCompStorage;

        public GroundDetectionCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity)
        {
        }

        public override void Enable()
        {
            _groundDetectionCompStorage = _xMachineEntity.GetOrCreateXStorage<GroundDetectionCompData>();
        }

        public override void Update()
        {
            if (!_groundDetectionCompStorage.IsEnable())
            {
                return;
            }

            GroundDetectionCompData groundDetectionCompData = _groundDetectionCompStorage.Get();
            groundDetectionCompData.isGrounded = PerformGroundCheck(groundDetectionCompData: groundDetectionCompData);
            _groundDetectionCompStorage.Set(value: groundDetectionCompData);

            if (!groundDetectionCompData.isGrounded)
            {
                _xMachineEntity.xMachine.Transition(toStateId: (int)PlayerState.Air);
            }
            else if (_xMachineEntity.xMachine.currentStateId == (int)PlayerState.Air)
            {
                _xMachineEntity.xMachine.Transition(toStateId: (int)PlayerState.Idle);
            }
        }

        private bool PerformGroundCheck(GroundDetectionCompData groundDetectionCompData)
        {
            Vector3 origin = _xMachineEntity.transform.position + groundDetectionCompData.checkOffset;

            switch (groundDetectionCompData.checkType)
            {
                case GroundDetectionCheckType.Raycast:
                    return Physics.Raycast(origin: origin, direction: Vector3.down, maxDistance: groundDetectionCompData.checkDistance, layerMask: groundDetectionCompData.groundLayer);

                case GroundDetectionCheckType.SphereCast:
                    return Physics.SphereCast(origin: origin, radius: groundDetectionCompData.sphereRadius, direction: Vector3.down, hitInfo: out RaycastHit _, maxDistance: groundDetectionCompData.checkDistance, layerMask: groundDetectionCompData.groundLayer);

                default:
                    return false;
            }
        }

        public override void Disable()
        {
        }
    }
}
