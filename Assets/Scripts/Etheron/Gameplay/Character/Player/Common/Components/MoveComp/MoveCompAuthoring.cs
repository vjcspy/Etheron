using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components.MoveComp
{
    public class MoveCompAuthoring : XCompAuthoring
    {
        [SerializeField] private float _runSpeed = 5f;
        [SerializeField] private float _walkSpeed = 2f;
        protected override void Authoring()
        {
            AddComponentData(
                component: new MoveCompData
                {
                    moveType = MoveType.Run,
                    runSpeed = _runSpeed,
                    walkSpeed = _walkSpeed
                }
            );
            AddSystem(system: new MoveCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
