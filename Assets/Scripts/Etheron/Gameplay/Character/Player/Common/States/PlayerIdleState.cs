using Etheron.Core.XMachine;
using Etheron.Gameplay.Character.Player.Common.Components;
using System;
namespace Etheron.Gameplay.Character.Player.Common.States
{
    public class PlayerIdleState : XMachineState
    {

        public PlayerIdleState(int id, XMachineEntity xMachineEntity) : base(id, xMachineEntity)
        {
        }
        internal override void Entry()
        {
            _xMachineEntity.AddXComponent(new InputCompData());
            _xMachineEntity.AddXComponent(new MoveCompData());
        }
    }
}
