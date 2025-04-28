using Etheron.Core.Component;
using Etheron.Core.XMachine;
using Etheron.Gameplay.Character.Player.Common.Components;
using Etheron.Gameplay.Character.Player.Common.States;
using Etheron.Input;
using System;
using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common
{
    public class PlayerEntity : XMachineEntity
    {
        [Header("Player Movement")]
        [SerializeField] private float _runSpeed = 5f;
        [SerializeField] private float _walkSpeed = 2f;
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


        protected override XCompSystem[] GetXCompSystems()
        {
            return
                new XCompSystem[]
                {
                    new InputCompSystem(xMachineEntity: this),
                    new MoveCompSystem(xMachineEntity: this)
                };
        }
        protected override XMachineState[] GetXMachineStates()
        {
            return new XMachineState[]
            {
                new PlayerIdleState(id: PlayerState.Idle, xMachineEntity: this),
                new PlayerRunningState(id: PlayerState.Running, xMachineEntity: this)
            };
        }
        protected override void Authoring()
        {
            // Add components to the entity
            AddXComponent(component: new InputCompData());
            AddXComponent(component: new MoveCompData
            {
                runSpeed = _runSpeed,
                walkSpeed = _walkSpeed,
                moveType = MoveType.Run
            });
        }
    }
}
