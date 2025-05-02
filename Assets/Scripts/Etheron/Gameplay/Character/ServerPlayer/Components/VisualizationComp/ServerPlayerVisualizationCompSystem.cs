using Cysharp.Threading.Tasks;
using Etheron.Colyseus;
using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using Etheron.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Etheron.Gameplay.Character.ServerPlayer.Components.VisualizationComp
{
    public class ServerPlayerVisualizationCompSystem : XCompSystem
    {
        private static readonly int AnimatorStateHash = Animator.StringToHash(name: "State");
        private readonly float _interpolationBackTime = 0.1f; // 100ms
        private readonly int _maxBufferSize = 30;

        private readonly List<(Vector3 pos, float time)> _positionBuffer = new List<(Vector3 pos, float time)>();
        private Animator _animator;
        private ColyseusManager _colyseusManager;
        private bool _isRunning;
        private Vector3 _lastServerPosition;
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
                            x: playerState.position.x,
                            y: playerState.position.y,
                            z: playerState.position.z
                        );
                        Vector3 facingDirection = new Vector3(
                            x: playerState.facingDirection.x,
                            y: playerState.facingDirection.y,
                            z: playerState.facingDirection.z);

                        _xMachineEntity.transform.rotation = Quaternion.LookRotation(forward: facingDirection, upwards: Vector3.up);
                        _animator.SetInteger(id: AnimatorStateHash, value: playerState.visualization.state);
                        _lastServerPosition = serverPos;
                        _positionBuffer.Add(item: (serverPos, Time.time));

                        // Giới hạn kích thước buffer
                        if (_positionBuffer.Count > _maxBufferSize)
                        {
                            _positionBuffer.RemoveAt(index: 0);
                        }
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
            float renderTime = Time.time - _interpolationBackTime;

            if (_positionBuffer.Count < 2)
            {
                // Fallback nếu không có đủ data để nội suy
                _transform.position = Vector3.Lerp(a: _transform.position, b: _lastServerPosition, t: 0.1f);
                return;
            }

            for (int i = 0; i < _positionBuffer.Count - 1; i++)
            {
                (Vector3 pos, float time) older = _positionBuffer[index: i];
                (Vector3 pos, float time) newer = _positionBuffer[index: i + 1];

                if (older.time <= renderTime && newer.time >= renderTime)
                {
                    float t = Mathf.InverseLerp(a: older.time, b: newer.time, value: renderTime);
                    Vector3 interpolated = Vector3.Lerp(a: older.pos, b: newer.pos, t: t);
                    _transform.position = interpolated;
                    return;
                }
            }

            // Nếu không có frame nào phù hợp (client bị trễ), move nhẹ về last known position
            _transform.position = Vector3.Lerp(a: _transform.position, b: _lastServerPosition, t: 0.1f);
        }

        public override void OnDestroy()
        {
            _isRunning = false;
        }
    }
}
