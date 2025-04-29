using Etheron.Core.Component;
using Etheron.Core.XMachine;
using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components
{
    public class MoveCompSystem : XCompSystem
    {
        private XCompStorage<InputCompData> _inputCompStorage;
        private XCompStorage<MoveCompData> _moveCompStorage;
        private Rigidbody _rb;

        public MoveCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity)
        {
        }

        public override void Start()
        {
            _moveCompStorage = _xMachineEntity.GetOrCreateXStorage<MoveCompData>();
            _inputCompStorage = _xMachineEntity.GetOrCreateXStorage<InputCompData>();

            _rb = _xMachineEntity.GetComponent<Rigidbody>();
        }

        public override void Update()
        {
            if (!_inputCompStorage.IsEnable() || !_moveCompStorage.IsEnable())
            {
                return;
            }

            InputCompData input = _inputCompStorage.Get();
            Vector2 movementInput = input.movementInput;

            if (Mathf.Abs(f: movementInput.x) < 0.01f)
            {
                _xMachineEntity.xMachine.Transition((int)PlayerState.Idle);
                return;
            }
            MoveCompData move = _moveCompStorage.Get();

            // Chọn speed
            float moveSpeed = move.moveType == MoveType.Run ? move.runSpeed : move.walkSpeed;

            // Di chuyển chỉ theo trục X (left/right), giữ Y cho gravity, Z luôn = 0
            Vector3 velocity = new Vector3(x: movementInput.x * moveSpeed, y: _rb.linearVelocity.y, z: 0f);

            _rb.linearVelocity = velocity;
        }

        public override void Stop()
        {
            // Optional clear nếu cần
        }
    }
}
