using Cysharp.Threading.Tasks;
using Etheron.Colyseus;
using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using Etheron.Utils;
using System;
using UnityEngine;
namespace Etheron.Gameplay.Character.ServerPlayer.Components.VisualizationComp
{
    public class ServerPlayerVisualizationCompSystem : XCompSystem
    {
        private static readonly int AnimatorStateHash = Animator.StringToHash(name: "State");

        private Animator _animator;
        private ColyseusManager _colyseusManager;

        // ===== INTERPOLATION STATE =====
        private Vector3 _currentLerpStart;
        private Vector3 _currentLerpTarget;
        private float _endTimestamp;
        private bool _hasPendingTarget;
        private bool _isLerping;

        private bool _isRunning;
        private int _nextAnimatorState = -1;

        // ===== NEW FACING & ANIMATION STATE =====
        private Vector3 _nextFacingDirection = Vector3.forward;

        // ===== PENDING NEXT TARGET =====
        private Vector3 _pendingTarget;
        private float _pendingTimestamp;
        private int _previousAnimationState = -1;

        // ===== LAST APPLIED STATE =====
        private Quaternion _previousRotation;
        private float _startTime;
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
            _previousAnimationState = -1;

            int interval = _storage.Get().updateIntervalMs;
            SyncLoop(interval: interval).Forget();
        }

        private async UniTaskVoid SyncLoop(int interval)
        {
            try
            {
                while (_isRunning)
                {
                    if (!_storage.IsEnable() ||
                        _colyseusManager.currentMapRoom?.State?.players == null)
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

                    if (_colyseusManager.currentMapRoom.State.players.TryGetValue(key: data.sessionId, value: out Colyseus.Schemas.Player playerState))
                    {
                        Vector3 serverPos = new Vector3(
                            x: playerState.position.value.x,
                            y: playerState.position.value.y,
                            z: playerState.position.value.z
                        );

                        float serverTimestamp = playerState.position.timestamp;

                        if (_startTimestamp == 0f)
                        {
                            _transform.position = serverPos;
                            _currentLerpStart = serverPos;
                            _currentLerpTarget = serverPos;
                            _startTimestamp = serverTimestamp;
                            _endTimestamp = serverTimestamp;
                            _startTime = Time.time;
                            _isLerping = false;
                            _hasPendingTarget = false;
                        }
                        else if (serverTimestamp > _endTimestamp)
                        {
                            _pendingTarget = serverPos;
                            _pendingTimestamp = serverTimestamp;
                            _hasPendingTarget = true;
                        }

                        // Lưu lại hướng và animation cho Update xử lý sau
                        _nextFacingDirection = new Vector3(
                            x: playerState.facingDirection.x,
                            y: playerState.facingDirection.y,
                            z: playerState.facingDirection.z
                        );

                        _nextAnimatorState = playerState.visualization.state;
                    }

                    await UniTask.Delay(millisecondsDelay: interval);
                }
            }
            catch (Exception ex)
            {
                ELogger.LogError(message: $"[ServerPlayerVisualizationCompSystem] SyncLoop exception: {ex.Message}");
            }
        }

        public override void Update()
        {
            if (!_isLerping)
            {
                if (_hasPendingTarget)
                {
                    StartLerpToPending();
                }
                else
                {
                    return;
                }
            }

            float elapsed = Time.time - _startTime;
            float duration = Mathf.Max(a: _endTimestamp - _startTimestamp, b: 0.001f);
            float t = Mathf.Clamp01(value: elapsed / duration);

            _transform.position = Vector3.Lerp(a: _currentLerpStart, b: _currentLerpTarget, t: t);

            // Rotation update nếu cần
            Quaternion targetRotation = Quaternion.LookRotation(forward: _nextFacingDirection.normalized, upwards: Vector3.up);
            if (_previousRotation != targetRotation)
            {
                _xMachineEntity.transform.rotation = targetRotation;
                _previousRotation = targetRotation;
            }

            // Animator update nếu khác
            if (_previousAnimationState != _nextAnimatorState)
            {
                _animator.SetInteger(id: AnimatorStateHash, value: _nextAnimatorState);
                _previousAnimationState = _nextAnimatorState;
            }

            if (t >= 1f)
            {
                _transform.position = _currentLerpTarget;
                _isLerping = false;

                if (_hasPendingTarget)
                {
                    StartLerpToPending();
                    float retryElapsed = Time.time - _startTime;
                    float retryDuration = Mathf.Max(a: _endTimestamp - _startTimestamp, b: 0.001f);
                    float retryT = Mathf.Clamp01(value: retryElapsed / retryDuration);
                    _transform.position = Vector3.Lerp(a: _currentLerpStart, b: _currentLerpTarget, t: retryT);
                }
            }

            Debug.DrawLine(start: _currentLerpStart, end: _currentLerpTarget, color: Color.green);
            Debug.DrawRay(start: _transform.position, dir: Vector3.up * 0.5f, color: Color.yellow);
        }

        private void StartLerpToPending()
        {
            _currentLerpStart = _transform.position;
            _currentLerpTarget = _pendingTarget;
            _startTimestamp = _endTimestamp;
            _endTimestamp = _pendingTimestamp;
            _startTime = Time.time;

            _hasPendingTarget = false;
            _isLerping = true;
        }

        public override void OnDestroy()
        {
            _isRunning = false;
        }
    }
}
