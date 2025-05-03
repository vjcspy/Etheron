using UnityEngine;
namespace Etheron.Utils.Testing
{
    [RequireComponent(requiredComponent: typeof(Rigidbody))]
    public class RandomXMovementWithRigidbody : MonoBehaviour
    {
        [Header(header: "X Movement Range")]
        public float minX = -10f;
        public float maxX = 10f;

        [Header(header: "Movement Settings")]
        public float moveSpeed = 3f;
        public float pauseDuration = 1f;
        private bool _isMoving;

        private Rigidbody _rb;
        private float _targetX;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();

            ChooseNewTarget();
        }

        private void FixedUpdate()
        {
            if (!_isMoving) return;

            float direction = Mathf.Sign(f: _targetX - transform.position.x);
            _rb.linearVelocity = new Vector3(x: direction * moveSpeed, y: _rb.linearVelocity.y, z: _rb.linearVelocity.z);

            if (Mathf.Abs(f: _targetX - transform.position.x) < 0.1f)
            {
                _isMoving = false;
                Invoke(methodName: nameof(ChooseNewTarget), time: pauseDuration);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector3 from = new Vector3(x: minX, y: transform.position.y, z: transform.position.z);
            Vector3 to = new Vector3(x: maxX, y: transform.position.y, z: transform.position.z);
            Gizmos.DrawLine(from: from, to: to);
        }

        private void ChooseNewTarget()
        {
            _targetX = Random.Range(minInclusive: minX, maxInclusive: maxX);
            _isMoving = true;
        }
    }
}
