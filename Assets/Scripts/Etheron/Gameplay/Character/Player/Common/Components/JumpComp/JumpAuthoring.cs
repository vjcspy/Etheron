using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using System;
using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components.JumpComp
{
    public class JumpAuthoring: XCompAuthoring
    {
        [SerializeField] private float jumpHeight = 2f;
        protected override void Authoring(XMachineEntity xMachineEntity)
        {
            xMachineEntity.AddXComponent(
                component: new JumpCompData
                {
                    jumpHeight = jumpHeight,
                });
            xMachineEntity.RegisterXCompSystem(system: new JumpCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
