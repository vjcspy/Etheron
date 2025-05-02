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

        // ===== PENDING NEXT TARGET =====
        private Vector3 _pendingTarget;
        private float _pendingTimestamp;
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
                            // Lần đầu tiên nhận gói → gán trực tiếp
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
                            // Cập nhật target tiếp theo
                            _pendingTarget = serverPos;
                            _pendingTimestamp = serverTimestamp;
                            _hasPendingTarget = true;
                        }

                        // Facing direction & animation luôn cập nhật ngay lập tức
                        Vector3 facingDirection = new Vector3(
                            x: playerState.facingDirection.x,
                            y: playerState.facingDirection.y,
                            z: playerState.facingDirection.z
                        );

                        _xMachineEntity.transform.rotation = Quaternion.LookRotation(forward: facingDirection, upwards: Vector3.up);
                        _animator.SetInteger(id: AnimatorStateHash, value: playerState.visualization.state);
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

            if (t >= 1f)
            {
                _transform.position = _currentLerpTarget;
                _isLerping = false;

                // Nếu có target mới → bắt đầu luôn (không đợi frame sau)
                if (_hasPendingTarget)
                {
                    StartLerpToPending();

                    // Tính lại và Lerp luôn để tránh khựng
                    float retryElapsed = Time.time - _startTime;
                    float retryDuration = Mathf.Max(a: _endTimestamp - _startTimestamp, b: 0.001f);
                    float retryT = Mathf.Clamp01(value: retryElapsed / retryDuration);
                    _transform.position = Vector3.Lerp(a: _currentLerpStart, b: _currentLerpTarget, t: retryT);
                }
            }

            Debug.DrawLine(_currentLerpStart, _currentLerpTarget, Color.green);
            Debug.DrawRay(_transform.position, Vector3.up * 0.5f, Color.yellow);
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
