using Etheron.Core.Component;
using Etheron.Core.XMachine;
using Etheron.Input;
using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components
{
    public class InputCompSystem : XCompSystem
    {
        private XCompStorage<InputCompData> _inputCompStorage;
        public InputCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity)
        {
        }
        public override void Start()
        {
            _inputCompStorage = _xMachineEntity.GetOrCreateXStorage<InputCompData>();
        }
        public override void Update()
        {
            if (!_inputCompStorage.IsEnable())
            {
                return;
            }

            // Get the input system
            InputSystem_Actions.PlayerActions playerActions = InputManager.Instance.InputActions.Player;

            // Get the movement input
            Vector2 movementInput = playerActions.Move.ReadValue<Vector2>();

            // Update the input component data
            InputCompData inputCompData = _inputCompStorage.Get();
            inputCompData.movementInput = movementInput;
            _inputCompStorage.Set(value: inputCompData);
        }
        public override void Stop()
        {
        }
    }
}
