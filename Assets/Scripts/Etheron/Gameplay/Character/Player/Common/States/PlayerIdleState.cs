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
    }
}
