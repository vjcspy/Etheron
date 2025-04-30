using Cysharp.Threading.Tasks;
using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using Etheron.Types;
using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components.VisualizationComp
{
    public class VisualizationCompSystem : XCompSystem
    {
        private static readonly int AnimatorStateHash = Animator.StringToHash(name: "State");
        private static readonly int AnimatorYVelocityHash = Animator.StringToHash(name: "YVelocity");

        private Animator _animator;
        private Quaternion _cachedFacingRotation = Quaternion.identity;

        private int _currentAnimationState = -1;
        private bool _isRunning;
        private Vector3 _previousFacingDirection = Vector3.zero;
        private Rigidbody _rb;
        private XCompStorage<VisualizationCompData> _visualizationCompStorage;

        public VisualizationCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity)
        {
        }

        public override void Enable()
        {
            InitializeReferences();
            _isRunning = true;
            RunAnimationUpdateLoop().Forget();
        }

        public override void Update()
        {
            // Not used
        }

        public override void Disable()
        {
            _isRunning = false;
        }

        private void InitializeReferences()
        {
            _visualizationCompStorage = _xMachineEntity.GetOrCreateXStorage<VisualizationCompData>();
            _animator = _xMachineEntity.GetComponentInChildren<Animator>();
            _rb = _xMachineEntity.GetComponent<Rigidbody>();
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

                VisualizationCompData data = _visualizationCompStorage.Get();

                bool shouldSet = UpdateAnimationState(data: ref data);

                // UpdateYVelocityIfAir();

                if (UpdateFacingDirection(data: ref data))
                {
                    shouldSet = true;
                }

                if (shouldSet)
                {
                    _visualizationCompStorage.Set(value: data);
                }

                await UniTask.Delay(millisecondsDelay: intervalTime);
            }
        }

        private bool UpdateAnimationState(ref VisualizationCompData data)
        {
            int newAnimationState = _xMachineEntity.xMachine.currentStateId switch
            {
                (int)PlayerState.Idle => (int)PlayerAnimationStateParam.Idle,
                (int)PlayerState.Walking => (int)PlayerAnimationStateParam.Walking,
                (int)PlayerState.Running => (int)PlayerAnimationStateParam.Running,
                (int)PlayerState.Jump => (int)PlayerAnimationStateParam.Jump,
                (int)PlayerState.Fall => (int)PlayerAnimationStateParam.Fall,
                _ => 0
            };

            if (_currentAnimationState != newAnimationState)
            {
                _currentAnimationState = newAnimationState;
                _animator.SetInteger(id: AnimatorStateHash, value: _currentAnimationState);
            }

            if (data.animationState != _currentAnimationState)
            {
                data.animationState = _currentAnimationState;
                return true;
            }

            return false;
        }

        private void UpdateYVelocityIfAir()
        {
            if (_currentAnimationState == (int)PlayerAnimationStateParam.Jump)
            {
                _animator.SetFloat(id: AnimatorYVelocityHash, value: _rb.linearVelocity.y);
            }
        }

        private bool UpdateFacingDirection(ref VisualizationCompData data)
        {
            if (data.facingDirection != FacingDirection.None &&
                data.facingDirection != _previousFacingDirection)
            {
                _previousFacingDirection = data.facingDirection;
                _cachedFacingRotation = Quaternion.LookRotation(forward: _previousFacingDirection, upwards: Vector3.up);
                _xMachineEntity.transform.rotation = _cachedFacingRotation;
                return true;
            }

            return false;
        }
    }
}
