﻿using Cysharp.Threading.Tasks;
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

        public override void OnCreate()
        {
            _groundDetectionCompStorage = GetStorage<GroundDetectionCompData>();
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

            if (
                _xMachineEntity.xMachine.currentStateId is (int)PlayerState.Fall or (int)PlayerState.Jump
                && groundDetectionCompData.isGrounded
                )
            {
                _xMachineEntity.xMachine.Transition(toStateId: (int)PlayerState.Idle);
            }
            else if (!groundDetectionCompData.isGrounded)
            {
                if (_xMachineEntity.xMachine.currentStateId != (int)PlayerState.Jump)
                {
                    _xMachineEntity.xMachine.Transition(toStateId: (int)PlayerState.Fall);
                }
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

        public override void OnDestroy()
        {
        }
    }
}
