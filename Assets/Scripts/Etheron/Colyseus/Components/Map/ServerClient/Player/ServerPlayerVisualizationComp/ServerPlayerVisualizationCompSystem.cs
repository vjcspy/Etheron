using Cysharp.Threading.Tasks;
using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using System.Collections.Generic;
using UnityEngine;
namespace Etheron.Colyseus.Components.Map.ServerClient.Player.ServerPlayerVisualizationComp
{
    public class ServerPlayerVisualizationCompSystem : XCompSystem
    {
        private const int capacity = 5; // the higher this value, the more/greater the delay
        private static readonly int AnimatorStateHash = Animator.StringToHash(name: "State");

        // ===== INTERPOLATION BUFFER =====
        private readonly Queue<InterpolationTarget> _interpolationBuffer = new Queue<InterpolationTarget>(capacity: capacity);

        private Animator _animator;
        private ColyseusManager _colyseusManager;

        // ===== INTERPOLATION STATE =====
        private Vector3 _currentLerpStart;
        private InterpolationTarget _currentTarget;
        private float _endTimestamp;
        private bool _isLerping;

        // ===== SYNC CONTROL =====
        private bool _isRunning;
        private float _lerpTimer;
        private int _previousAnimationState = -1;

        // ===== LAST APPLIED STATE =====
        private Quaternion _previousRotation;
        private float _startTimestamp;

        // ===== COMPONENT STORAGE =====
        private XCompStorage<ServerPlayerVisualizationCompData> _storage;
        private Transform _transform;

        public ServerPlayerVisualizationCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity) { }

        public override void OnCreate()
        {
            _colyseusManager = ColyseusManager.Instance;
            _storage = GetStorage<ServerPlayerVisualizationCompData>();
            _transform = GetComponent<Transform>();
            _animator = _xMachineEntity.GetComponentInChildren<Animator>();
            _isRunning = true;

            _previousRotation = _transform.rotation;

            int interval = _storage.Get().updateIntervalMs;
            SyncLoop(interval: interval).Forget();
        }

        private async UniTaskVoid SyncLoop(int interval)
        {
            while (_isRunning)
            {
                if (!_storage.IsEnable() || _colyseusManager.currentMapRoom?.State?.players == null)
                {
                    await UniTask.Delay(millisecondsDelay: interval);
                    continue;
                }

                ServerPlayerVisualizationCompData data = _storage.Get();
                if (string.IsNullOrEmpty(value: data.sessionId))
                {
                    await UniTask.Delay(millisecondsDelay: interval);
                    continue;
                }

                if (_colyseusManager.currentMapRoom.State.players.TryGetValue(key: data.sessionId, value: out Schemas.Player playerState))
                {
                    InterpolationTarget newTarget = new InterpolationTarget
                    {
                        position = new Vector3(
                            x: playerState.position.value.x,
                            y: playerState.position.value.y,
                            z: playerState.position.value.z),
                        timestamp = playerState.position.timestamp,
                        facingDirection = new Vector3(
                            x: playerState.facingDirection.x,
                            y: playerState.facingDirection.y,
                            z: playerState.facingDirection.z),
                        animationState = playerState.visualization.state
                    };

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
                        {
                            _interpolationBuffer.Dequeue(); // Loại bỏ phần tử cũ nhất nếu đầy
                        }
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
                InterpolationTarget nextTarget = _interpolationBuffer.Dequeue();
                BeginLerpTo(target: nextTarget);
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
            // Cập nhật rotation
            Quaternion targetRotation = Quaternion.LookRotation(forward: _currentTarget.facingDirection.normalized, upwards: Vector3.up);
            if (_previousRotation != targetRotation)
            {
                _xMachineEntity.transform.rotation = targetRotation;
                _previousRotation = targetRotation;
            }

            // Cập nhật animator
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

        // ===== INTERPOLATION TARGET PACKAGE =====
        private struct InterpolationTarget
        {
            public Vector3 position;
            public float timestamp;
            public Vector3 facingDirection;
            public int animationState;
        }
    }
}
