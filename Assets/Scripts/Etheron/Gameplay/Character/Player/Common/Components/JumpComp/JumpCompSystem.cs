using Etheron.Core.Component;
using Etheron.Core.XMachine;
using Etheron.Gameplay.Character.Player.Common.Components.InputComp;
using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components.JumpComp
{
    public class JumpCompSystem : XCompSystem
    {
        private Vector3 _gravity;
        private XCompStorage<GroundDetectionCompData> _groundDetectionCompStorage;
        private XCompStorage<InputCompData> _inputCompStorage;
        private XCompStorage<JumpCompData> _jumpCompStorage;
        private Rigidbody _rb;

        public JumpCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity)
        {
        }
        public override void Enable()
        {
            _jumpCompStorage = _xMachineEntity.GetXStorage<JumpCompData>();
            _groundDetectionCompStorage = _xMachineEntity.GetXStorage<GroundDetectionCompData>();
            _inputCompStorage = _xMachineEntity.GetXStorage<InputCompData>();
            _gravity = Physics.gravity;
            _rb = _xMachineEntity.GetComponent<Rigidbody>();
        }
        public override void Update()
        {
            if (_jumpCompStorage.IsEnable())
            {
                JumpCompData jumpCompData = _jumpCompStorage.Get();
                GroundDetectionCompData groundDetectionCompData = _groundDetectionCompStorage.Get();
                InputCompData inputCompData = _inputCompStorage.Get();
                if (inputCompData.jumpPressed && groundDetectionCompData.isGrounded)
                {
                    float jumpVelocity = Mathf.Sqrt(f: 2 * _gravity.magnitude * jumpCompData.jumpHeight);

                    Vector3 currentVelocity = _rb.linearVelocity;
                    currentVelocity.y = jumpVelocity;

                    _rb.linearVelocity = currentVelocity;

                    // Reset jumpPressed to false after applying jump
                    inputCompData.jumpPressed = false;
                    _inputCompStorage.Set(value: inputCompData);
                }
            }
        }
        public override void Disable()
        {
        }
    }
}
