using Etheron.Core.Component;
using Etheron.Core.XMachine;
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

            // Update the visualization component data
            // VisualizationCompData visualizationCompData = _visualizationCompStorage.Get();
            switch (_xMachineEntity.xMachine.currentStateId)
            {
                case (int)PlayerState.Idle:
                    _animator.SetInteger(id: AnimatorStateHash, value: 0);
                    break;
                case (int)PlayerState.Walking:
                    _animator.SetInteger(id: AnimatorStateHash, value: 1);
                    break;
                case (int)PlayerState.Running:
                    _animator.SetInteger(id: AnimatorStateHash, value: 2);
                    break;
            }
        }
        public override void Stop()
        {
        }
    }
}
