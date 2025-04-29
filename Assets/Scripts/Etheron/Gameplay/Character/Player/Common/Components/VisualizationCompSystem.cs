using Etheron.Core.Component;
using Etheron.Core.XMachine;
using Etheron.Types;
using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components
{
    public class VisualizationCompSystem : XCompSystem
    {
        private static readonly int AnimatorStateHash = Animator.StringToHash(name: "State");
        private Animator _animator;
        private XCompStorage<VisualizationCompData> _visualizationCompStorage;

        public VisualizationCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity)
        {
        }
        public override void Start()
        {
            _visualizationCompStorage = _xMachineEntity.GetOrCreateXStorage<VisualizationCompData>();
            _animator = _xMachineEntity.GetComponentInChildren<Animator>();
        }
        public override void Update()
        {
            if (!_visualizationCompStorage.IsEnable()) return;

            VisualizationCompData visualizationCompData = _visualizationCompStorage.Get();
            int animationState = _xMachineEntity.xMachine.currentStateId switch
            {
                (int)PlayerState.Idle => 0,
                (int)PlayerState.Walking => 1,
                (int)PlayerState.Running => 2,
                _ => 0
            };
            _animator.SetInteger(id: AnimatorStateHash, value: animationState);

            if (visualizationCompData.facingDirection != FacingDirection.None)
            {
                _xMachineEntity.transform.rotation = Quaternion.LookRotation(forward: visualizationCompData.facingDirection.normalized, upwards: Vector3.up);
            }
        }
        public override void Stop()
        {
        }
    }
}
