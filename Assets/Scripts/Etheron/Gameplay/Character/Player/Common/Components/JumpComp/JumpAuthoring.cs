﻿using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using System;
using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components.JumpComp
{
    public class JumpAuthoring: XCompAuthoring
    {
        [SerializeField] private float jumpHeight = 2f;
        protected override void Authoring()
        {
            AddComponentData(
                component: new JumpCompData
                {
                    jumpHeight = jumpHeight,
                });
            AddSystem(system: new JumpCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
