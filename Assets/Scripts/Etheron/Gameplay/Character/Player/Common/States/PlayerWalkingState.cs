using Etheron.Core.Component;
using Etheron.Core.XMachine;
using Etheron.Gameplay.Character.Player.Common.Components;
namespace Etheron.Gameplay.Character.Player.Common.States
{
    public class PlayerWalkingState : XMachineState
    {
        private readonly XCompStorage<GroundDetectionCompData> _groundDetectionCompStorage;

        public PlayerWalkingState(int id, XMachineEntity xMachineEntity) : base(id: id, xMachineEntity: xMachineEntity)
        {
            _groundDetectionCompStorage = xMachineEntity.GetXStorage<GroundDetectionCompData>();
        }

        internal override bool Guard()
        {
            return _groundDetectionCompStorage.Get().isGrounded;
        }
    }
}
