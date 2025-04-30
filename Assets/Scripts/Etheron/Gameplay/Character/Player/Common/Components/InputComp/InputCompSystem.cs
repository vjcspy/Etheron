using Etheron.Core.Component;
using Etheron.Core.XMachine;
using Etheron.Gameplay.Character.Player.Common.Components.InputComp;
using Etheron.Input;
using UnityEngine;
using System;

namespace Etheron.Gameplay.Character.Player.Common.Components
{
    public class InputCompSystem : XCompSystem
    {
        private XCompStorage<InputCompData> _inputCompStorage;
        private InputSystem_Actions.PlayerActions _playerActions;

        private readonly Action<UnityEngine.InputSystem.InputAction.CallbackContext> _onJumpPerformed;

        public InputCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity)
        {
            // Khởi tạo delegate một lần, tránh tạo GC Alloc mỗi frame
            _onJumpPerformed = OnJumpPerformed;
        }

        public override void Enable()
        {
            _inputCompStorage = _xMachineEntity.GetOrCreateXStorage<InputCompData>();
            _playerActions = InputManager.Instance.InputActions.Player;

            _playerActions.Enable();
            _playerActions.Jump.performed += _onJumpPerformed;
        }

        public override void Update()
        {
            if (!_inputCompStorage.IsEnable()) return;

            Vector2 movementInput = _playerActions.Move.ReadValue<Vector2>();

            InputCompData inputData = _inputCompStorage.Get();
            inputData.movementInput = movementInput;
            // jumpPressed sẽ được gán trong event, không gán ở đây
            _inputCompStorage.Set(inputData);
        }

        private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (!_inputCompStorage.IsEnable()) return;

            InputCompData inputData = _inputCompStorage.Get();
            inputData.jumpPressed = true;
            _inputCompStorage.Set(inputData);
        }

        public override void Disable()
        {
            _playerActions.Jump.performed -= _onJumpPerformed;
            _playerActions.Disable();
        }
    }
}
