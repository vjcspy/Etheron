using Etheron.Core.Component;
using Etheron.Core.XMachine;
using UnityEngine;
namespace Etheron.Gameplay.Character.Player.Common.Components
{


    public class GroundDetectionCompSystem : XCompSystem
    {
        private XCompStorage<GroundDetectionCompData> _groundDetectionCompStorage;

        public GroundDetectionCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity)
        {
        }

        public override void Start()
        {
            _groundDetectionCompStorage = _xMachineEntity.GetOrCreateXStorage<GroundDetectionCompData>();
        }

        public override void Update()
        {
            if (!_groundDetectionCompStorage.IsEnable())
            {
                return;
            }

            GroundDetectionCompData groundDetectionCompData = _groundDetectionCompStorage.Get();
            groundDetectionCompData.isGrounded = Physics.Raycast(origin: _xMachineEntity.transform.position, direction: Vector3.down, maxDistance: 1.0f);
            _groundDetectionCompStorage.Set(value: groundDetectionCompData);
        }

        public override void Stop()
        {
        }
    }
}
