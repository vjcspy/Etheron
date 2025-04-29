using Etheron.Core.XMachine;
using Etheron.Gameplay.Character.Player.Common.Components;
using System;
namespace Etheron.Gameplay.Character.Player.Common.States
{
    public class PlayerRunningState : XMachineState
    {
        public PlayerRunningState(int id, XMachineEntity xMachineEntity) : base(id: id, xMachineEntity: xMachineEntity)
        {
        }

        internal override void Entry()
        {
            _xMachineEntity.AddXComponent(component: new InputCompData());
            _xMachineEntity.AddXComponent(component: new MoveCompData());
        }
    }
}
