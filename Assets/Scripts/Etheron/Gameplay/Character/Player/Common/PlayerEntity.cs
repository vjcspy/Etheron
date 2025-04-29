using Etheron.Core.XMachine;
using Etheron.Gameplay.Character.Player.Common.States;
using Etheron.Input;
namespace Etheron.Gameplay.Character.Player.Common
{
    public class PlayerEntity : XMachineEntity
    {

        #region XMachineEntity

        protected override XMachineState[] GetXMachineStates()
        {
            return new XMachineState[]
            {
                new PlayerIdleState(id: (int)PlayerState.Idle, xMachineEntity: this),
                new PlayerWalkingState(id: (int)PlayerState.Walking, xMachineEntity: this),
                new PlayerRunningState(id: (int)PlayerState.Running, xMachineEntity: this)
            };
        }

        #endregion

        #region Handle Input

        private void OnEnable()
        {
            // Initialize the input system
            InputSystem_Actions.PlayerActions playerActions = InputManager.Instance.InputActions.Player;
            playerActions.Enable();
        }

        private void OnDisable()
        {
            // Disable the input system
            InputSystem_Actions.PlayerActions playerActions = InputManager.Instance.InputActions.Player;
            playerActions.Disable();
        }

        #endregion

    }
}
