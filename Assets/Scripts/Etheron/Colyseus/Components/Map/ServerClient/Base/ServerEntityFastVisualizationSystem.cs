using Cysharp.Threading.Tasks;
using Etheron.Colyseus.Components.Map.ServerClient.Player.ServerPlayerVisualization;
using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using Etheron.Gameplay.Values;
using UnityEngine;
namespace Etheron.Colyseus.Components.Map.ServerClient.Base
{
    public abstract class ServerEntityFastVisualizationSystem<TState> : XCompSystem
    {
        private Animator _animator;
        private TState _cachedEntityState;

        protected ColyseusManager _colyseusManager;
        private InterpolationTarget _currentTarget;
        private float _duration;
        private float _endTimestamp;
        private bool _hasNewTarget;
        private bool _isLerping;
        private bool _isRunning;

        private InterpolationTarget _latestTarget;

        private Vector3 _lerpStart;
        private float _lerpTimer;
        private int _previousAnimationState = -1;

        private Quaternion _previousRotation;
        private float _startTimestamp;

        private Transform _transform;

        protected ServerEntityFastVisualizationSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity) { }

        public override void OnCreate()
        {
            _colyseusManager = ColyseusManager.Instance;
            _transform = GetComponent<Transform>();
            _animator = _xMachineEntity.GetComponentInChildren<Animator>();

            _previousRotation = _transform.rotation;
            _isRunning = true;

            int interval = GetStorage<ServerPlayerVisualizationCompData>().Get().updateIntervalMs;
            SyncLoop(interval: interval).Forget();
        }

        private async UniTaskVoid SyncLoop(int interval)
        {
            await UniTask.Delay(millisecondsDelay: 2000); // Chờ Colyseus ổn định

            while (_isRunning)
            {
                if (!TryGetEntityState(entityState: out _cachedEntityState))
                {
                    await UniTask.Delay(millisecondsDelay: 1000);
                    continue;
                }

                if (TryExtractInterpolationTarget(state: _cachedEntityState, target: out InterpolationTarget newTarget))
                {
                    if (newTarget.timestamp > _latestTarget.timestamp)
                    {
                        if (_endTimestamp == 0)
                        {
                            _transform.position = newTarget.position;
                            _endTimestamp = newTarget.timestamp;
                        }
                        else
                        {
                            _latestTarget = newTarget;
                            _hasNewTarget = true;
                        }
                    }
                }

                await UniTask.Delay(millisecondsDelay: interval);
            }
        }

        public override void Update()
        {
            if (!_isLerping && _hasNewTarget)
            {
                BeginLerpTo(target: _latestTarget);
                _hasNewTarget = false;
            }

            if (_isLerping)
            {
                _lerpTimer += Time.deltaTime;
                float t = Mathf.Clamp01(value: _lerpTimer / _duration);

                _transform.position = Vector3.Lerp(a: _lerpStart, b: _currentTarget.position, t: t);

                if (t >= 1f)
                {
                    ApplyCurrentTargetState();
                    _isLerping = false;
                }
            }
        }

        private void BeginLerpTo(InterpolationTarget target)
        {
            _lerpStart = _transform.position;
            _startTimestamp = _endTimestamp;
            _endTimestamp = target.timestamp;
            _currentTarget = target;
            _duration = Mathf.Max(a: _endTimestamp - _startTimestamp, b: 0.001f);
            _isLerping = true;
            _lerpTimer = 0f;
        }

        private void ApplyCurrentTargetState()
        {
            Quaternion targetRotation = Quaternion.LookRotation(forward: _currentTarget.facingDirection.normalized, upwards: Vector3.up);
            if (_previousRotation != targetRotation)
            {
                _xMachineEntity.transform.rotation = targetRotation;
                _previousRotation = targetRotation;
            }

            if (_previousAnimationState != _currentTarget.animationState)
            {
                _animator.SetInteger(id: AnimatorHashes.State, value: _currentTarget.animationState);
                _previousAnimationState = _currentTarget.animationState;
            }

            _transform.position = _currentTarget.position;
        }

        public override void OnDestroy()
        {
            _isRunning = false;
        }

        protected abstract bool TryGetEntityState(out TState entityState);
        protected abstract bool TryExtractInterpolationTarget(TState state, out InterpolationTarget target);

        protected struct InterpolationTarget
        {
            public Vector3 position;
            public float timestamp;
            public Vector3 facingDirection;
            public int animationState;
        }
    }
}
