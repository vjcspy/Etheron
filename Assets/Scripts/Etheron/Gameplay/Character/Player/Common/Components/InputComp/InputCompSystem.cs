﻿using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using Etheron.Input;
using System;
using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components.InputComp
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

        public override void OnCreate()
        {
            _inputCompStorage = _xMachineEntity.GetOrCreateStorage<InputCompData>();
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

        public override void OnDestroy()
        {
            _playerActions.Jump.performed -= _onJumpPerformed;
            _playerActions.Disable();
        }
    }
}
