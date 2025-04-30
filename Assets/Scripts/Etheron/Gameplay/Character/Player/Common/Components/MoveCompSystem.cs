using Etheron.Core.Component;
using Etheron.Core.XMachine;
using Etheron.Gameplay.Character.Player.Common.Components.InputComp;
using Etheron.Types;
using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components
{
    public class MoveCompSystem : XCompSystem
    {
        private XCompStorage<InputCompData> _inputCompStorage;
        private XCompStorage<MoveCompData> _moveCompStorage;
        private Rigidbody _rb;

        private XCompStorage<VisualizationCompData> _visualizationCompStorage;

        public MoveCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity)
        {
        }

        public override void Enable()
        {
            _moveCompStorage = _xMachineEntity.GetOrCreateXStorage<MoveCompData>();
            _inputCompStorage = _xMachineEntity.GetOrCreateXStorage<InputCompData>();
            _visualizationCompStorage = _xMachineEntity.GetOrCreateXStorage<VisualizationCompData>();

            _rb = _xMachineEntity.GetComponent<Rigidbody>();
        }

        public override void Update()
        {
            if (!_inputCompStorage.IsEnable() || !_moveCompStorage.IsEnable())
            {
                return;
            }

            InputCompData input = _inputCompStorage.Get();
            VisualizationCompData visualization = _visualizationCompStorage.Get();
            Vector2 movementInput = input.movementInput;

            if (Mathf.Abs(f: movementInput.x) < 0.01f)
            {
                visualization.facingDirection = FacingDirection.Front;
                _visualizationCompStorage.Set(visualization);
                _xMachineEntity.xMachine.Transition(toStateId: (int)PlayerState.Idle);
                return;
            }
            MoveCompData move = _moveCompStorage.Get();

            // Chọn speed
            float moveSpeed = move.moveType == MoveType.Run ? move.runSpeed : move.walkSpeed;

            // Di chuyển chỉ theo trục X (left/right), giữ Y cho gravity, Z luôn = 0
            Vector3 velocity = new Vector3(x: movementInput.x * moveSpeed, y: _rb.linearVelocity.y, z: 0f);

            _rb.linearVelocity = velocity;

            // Update visualization
            visualization.facingDirection = movementInput.x > 0f ? FacingDirection.Right : FacingDirection.Left;
            _visualizationCompStorage.Set(visualization);

            // Safe to transition because each state has its own guard
            _xMachineEntity.xMachine.Transition(toStateId: (int)PlayerState.Running);
        }

        public override void Disable()
        {
            // Optional clear nếu cần
        }
    }
}
