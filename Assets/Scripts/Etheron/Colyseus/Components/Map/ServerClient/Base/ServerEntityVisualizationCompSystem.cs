using Cysharp.Threading.Tasks;
using Etheron.Colyseus.Components.Map.ServerClient.Player.ServerPlayerVisualization;
using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using System.Collections.Generic;
using UnityEngine;
namespace Etheron.Colyseus.Components.Map.ServerClient.Base
{
    public abstract class ServerEntityVisualizationCompSystem<TState> : XCompSystem
    {
        private const int capacity = 5;
        private static readonly int AnimatorStateHash = Animator.StringToHash(name: "State");

        private readonly Queue<InterpolationTarget> _interpolationBuffer = new Queue<InterpolationTarget>(capacity: capacity);

        private Animator _animator;

        private TState _cachedEntityState;
        protected ColyseusManager _colyseusManager;

        private Vector3 _currentLerpStart;
        private InterpolationTarget _currentTarget;
        private float _endTimestamp;
        private bool _isLerping;

        private bool _isRunning;
        private float _lerpTimer;
        private int _previousAnimationState = -1;

        private Quaternion _previousRotation;
        private float _startTimestamp;

        private Transform _transform;

        public ServerEntityVisualizationCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity) { }

        public override void OnCreate()
        {
            _colyseusManager = ColyseusManager.Instance;
            _transform = GetComponent<Transform>();
            _animator = _xMachineEntity.GetComponentInChildren<Animator>();
            _isRunning = true;

            _previousRotation = _transform.rotation;

            int interval = GetStorage<ServerPlayerVisualizationCompData>().Get().updateIntervalMs;
            SyncLoop(interval: interval).Forget();
        }

        private async UniTaskVoid SyncLoop(int interval)
        {
            while (_isRunning)
            {
                if (!TryGetEntityState(entityState: out _cachedEntityState))
                {
                    await UniTask.Delay(millisecondsDelay: 1000);
                    continue;
                }

                if (TryExtractInterpolationTarget(state: _cachedEntityState, target: out InterpolationTarget newTarget))
                {
                    if (_startTimestamp == 0f)
                    {
                        _transform.position = newTarget.position;
                        _currentLerpStart = newTarget.position;
                        _currentTarget = newTarget;
                        _startTimestamp = newTarget.timestamp;
                        _endTimestamp = newTarget.timestamp;
                        _isLerping = false;
                        _interpolationBuffer.Clear();
                    }
                    else if (newTarget.timestamp > _endTimestamp)
                    {
                        if (_interpolationBuffer.Count >= capacity)
                            _interpolationBuffer.Dequeue();

                        _interpolationBuffer.Enqueue(item: newTarget);
                    }
                }

                await UniTask.Delay(millisecondsDelay: interval);
            }
        }

        public override void Update()
        {
            if (!_isLerping && _interpolationBuffer.Count > 0)
            {
                BeginLerpTo(target: _interpolationBuffer.Dequeue());
            }

            if (_isLerping)
            {
                float duration = Mathf.Max(a: _endTimestamp - _startTimestamp, b: 0.001f);
                _lerpTimer += Time.deltaTime;
                float t = Mathf.Clamp01(value: _lerpTimer / duration);

                _transform.position = Vector3.Lerp(a: _currentLerpStart, b: _currentTarget.position, t: t);

                if (t >= 1f)
                {
                    ApplyCurrentTargetState();
                    _transform.position = _currentTarget.position;

                    _isLerping = false;
                    _lerpTimer = 0f;
                }
            }
        }

        private void BeginLerpTo(InterpolationTarget target)
        {
            _currentLerpStart = _transform.position;
            _startTimestamp = _endTimestamp;
            _endTimestamp = target.timestamp;

            _currentTarget = target;
            _isLerping = true;
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
                _animator.SetInteger(id: AnimatorStateHash, value: _currentTarget.animationState);
                _previousAnimationState = _currentTarget.animationState;
            }
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
