using Etheron.Core.Component;
using Etheron.Core.XMachine;
using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components
{
    public class MoveCompAuthoring : XCompAuthoring
    {
        [SerializeField] private float _runSpeed = 5f;
        [SerializeField] private float _walkSpeed = 2f;
        protected override void Authoring(XMachineEntity xMachineEntity)
        {
            xMachineEntity.AddXComponent(
                component: new MoveCompData
                {
                    moveType = MoveType.Run,
                    runSpeed = _runSpeed,
                    walkSpeed = _walkSpeed
                }
            );
            xMachineEntity.RegisterXCompSystem(system: new MoveCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
