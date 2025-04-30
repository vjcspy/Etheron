using Cysharp.Threading.Tasks;
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

        private int _currentAnimationState = -1;
        private bool _isRunning;
        private Vector3 _previousFacingDirection = Vector3.zero;
        private XCompStorage<VisualizationCompData> _visualizationCompStorage;

        public VisualizationCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity)
        {
        }

        public override void Enable()
        {
            _visualizationCompStorage = _xMachineEntity.GetOrCreateXStorage<VisualizationCompData>();
            _animator = _xMachineEntity.GetComponentInChildren<Animator>();

            _isRunning = true;
            RunAnimationUpdateLoop().Forget();
        }
        public override void Update()
        {
        }

        public override void Disable()
        {
            _isRunning = false; // Dừng loop khi component bị stop
        }

        private async UniTaskVoid RunAnimationUpdateLoop()
        {
            int intervalTime = _visualizationCompStorage.Get().animationUpdateIntervalMs;
            while (_isRunning)
            {
                if (!_visualizationCompStorage.IsEnable())
                {
                    await UniTask.Delay(millisecondsDelay: intervalTime);
                    continue;
                }

                int newAnimationState = _xMachineEntity.xMachine.currentStateId switch
                {
                    (int)PlayerState.Idle => 0,
                    (int)PlayerState.Walking => 1,
                    (int)PlayerState.Running => 2,
                    _ => 0
                };

                if (_currentAnimationState != newAnimationState)
                {
                    _currentAnimationState = newAnimationState;
                    _animator.SetInteger(id: AnimatorStateHash, value: _currentAnimationState);
                }
                VisualizationCompData visualizationCompData = _visualizationCompStorage.Get();
                visualizationCompData.animationState = _currentAnimationState;
                _visualizationCompStorage.Set(value: visualizationCompData);

                if (visualizationCompData.facingDirection != FacingDirection.None)
                {
                    if (visualizationCompData.facingDirection != _previousFacingDirection)
                    {
                        _previousFacingDirection = visualizationCompData.facingDirection;
                        _xMachineEntity.transform.rotation = Quaternion.LookRotation(forward: _previousFacingDirection, upwards: Vector3.up);
                    }
                }

                await UniTask.Delay(millisecondsDelay: intervalTime);
            }
        }
    }
}
