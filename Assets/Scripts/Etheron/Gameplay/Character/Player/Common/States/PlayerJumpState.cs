using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using Etheron.Gameplay.Character.Player.Common.Components.GroundDetectionComp;
namespace Etheron.Gameplay.Character.Player.Common.States
{
    public class PlayerJumpState : XMachineState
    {
        private XCompStorage<GroundDetectionCompData> _groundDetectionCompStorage;

        public PlayerJumpState(int id, XMachineEntity xMachineEntity) : base(id: id, xMachineEntity: xMachineEntity)
        {

        }
        public override void OnCreate()
        {
            _groundDetectionCompStorage = _xMachineEntity.GetXStorage<GroundDetectionCompData>();
        }
    }
}
