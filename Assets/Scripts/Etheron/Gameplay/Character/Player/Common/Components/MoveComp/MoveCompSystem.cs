using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using Etheron.Gameplay.Character.Player.Common.Components.InputComp;
using Etheron.Gameplay.Character.Player.Common.Components.VisualizationComp;
using Etheron.Types;
using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components.MoveComp
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

        public override void OnCreate()
        {
            _moveCompStorage = _xMachineEntity.GetOrCreateStorage<MoveCompData>();
            _inputCompStorage = _xMachineEntity.GetOrCreateStorage<InputCompData>();
            _visualizationCompStorage = _xMachineEntity.GetOrCreateStorage<VisualizationCompData>();

            _rb = _xMachineEntity.GetComponent<Rigidbody>();
        }

        public override void Update()
        {
            if (!_inputCompStorage.IsEnable() || !_moveCompStorage.IsEnable())
            {
                return;
            }

            InputCompData inputCompData = _inputCompStorage.Get();
            VisualizationCompData visualization = _visualizationCompStorage.Get();
            MoveCompData moveCompData = _moveCompStorage.Get();
            Vector2 movementInput = inputCompData.movementInput;

            if (Mathf.Abs(f: movementInput.x) < 0.01f)
            {
                visualization.facingDirection = FacingDirection.Front;
                _visualizationCompStorage.Set(value: visualization);

                _xMachineEntity.xMachine.Transition(toStateId: (int)PlayerState.Idle);

                return;
            }


            // Chọn speed
            float moveSpeed = moveCompData.moveType == MoveType.Run ? moveCompData.runSpeed : moveCompData.walkSpeed;

            // Di chuyển chỉ theo trục X (left/right), giữ Y cho gravity, Z luôn = 0
            Vector3 velocity = new Vector3(x: movementInput.x * moveSpeed, y: _rb.linearVelocity.y, z: 0f);

            _rb.linearVelocity = velocity;

            // Update visualization
            visualization.facingDirection = movementInput.x > 0f ? FacingDirection.Right : FacingDirection.Left;
            _visualizationCompStorage.Set(value: visualization);

            // Safe to transition because each state has its own guard
            _xMachineEntity.xMachine.Transition(toStateId: (int)PlayerState.Running);
        }

        public override void OnDestroy()
        {
            // Optional clear nếu cần
        }
    }
}
