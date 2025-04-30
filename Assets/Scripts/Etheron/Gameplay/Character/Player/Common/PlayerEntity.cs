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
                new PlayerRunningState(id: (int)PlayerState.Running, xMachineEntity: this),
                new PlayerJumpState(id: (int)PlayerState.Jump, xMachineEntity: this),
                new PlayerFallState(id: (int)PlayerState.Fall, xMachineEntity: this),
            };
        }

        #endregion
    }
}
