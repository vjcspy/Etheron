using Cysharp.Threading.Tasks;
using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using Etheron.Gameplay.Character.Player.Common.Components.GroundDetectionComp;
using Etheron.Gameplay.Character.Player.Common.Components.InputComp;
using System;
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

        public JumpCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity) { }

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
            if (!_jumpCompStorage.IsEnable())
                return;

            HandleJumpInput();
        }

        private void HandleJumpInput()
        {
            InputCompData inputCompData = _inputCompStorage.Get();
            JumpCompData jumpCompData = _jumpCompStorage.Get();
            GroundDetectionCompData groundDetectionCompData = _groundDetectionCompStorage.Get();

            if (inputCompData.jumpPressed && groundDetectionCompData.isGrounded)
            {
                inputCompData.jumpPressed = false;
                _inputCompStorage.Set(value: inputCompData);

                ApplyJumpVelocity(jumpCompData: jumpCompData);
                StartJumpTransitionCheckAsync().Forget();
            }
        }

        private void ApplyJumpVelocity(JumpCompData jumpCompData)
        {
            float jumpVelocity = Mathf.Sqrt(f: 2 * _gravity.magnitude * jumpCompData.jumpHeight);
            Vector3 currentVelocity = _rb.linearVelocity;
            currentVelocity.y = jumpVelocity;
            _rb.linearVelocity = currentVelocity;
        }

        private async UniTaskVoid StartJumpTransitionCheckAsync()
        {
            float elapsedTime = 0f;
            const float initialDelay = 0.05f;
            const float checkDuration = 0.150f;
            const float pollInterval = 0.05f;

            await UniTask.Delay(delayTimeSpan: TimeSpan.FromSeconds(value: initialDelay), delayType: DelayType.DeltaTime);
            elapsedTime += initialDelay;

            while (elapsedTime < checkDuration)
            {
                if (HasLeftGround())
                {
                    TriggerJumpState();
                    return;
                }

                await UniTask.Delay(delayTimeSpan: TimeSpan.FromSeconds(value: pollInterval), delayType: DelayType.DeltaTime);
                elapsedTime += pollInterval;
            }
        }

        private bool HasLeftGround()
        {
            GroundDetectionCompData groundData = _groundDetectionCompStorage.Get();
            return !groundData.isGrounded;
        }

        private void TriggerJumpState()
        {
            _xMachineEntity.xMachine.Transition(toStateId: (int)PlayerState.Jump);
        }

        public override void Disable()
        {
        }
    }
}
