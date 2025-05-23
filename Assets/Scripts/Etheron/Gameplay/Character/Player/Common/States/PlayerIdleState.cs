﻿using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using Etheron.Gameplay.Character.Player.Common.Components.GroundDetectionComp;
namespace Etheron.Gameplay.Character.Player.Common.States
{
    public class PlayerIdleState : XMachineState
    {
        private XCompStorage<GroundDetectionCompData> _groundDetectionCompStorage;
        public PlayerIdleState(int id, XMachineEntity xMachineEntity) : base(id: id, xMachineEntity: xMachineEntity)
        {
        }

        public override void OnCreate()
        {
            _groundDetectionCompStorage = _xMachineEntity.GetStorage<GroundDetectionCompData>();
        }

        internal override bool Guard()
        {
            return _groundDetectionCompStorage.Get().isGrounded;
        }
    }
}
