using UnityEngine;
namespace Etheron.Types
{
    public static class FacingDirection
    {
        public static readonly Vector3 None = Vector3.zero;
        public static readonly Vector3 Left = new Vector3(x: -1, y: 0, z: 0);
        public static readonly Vector3 Right = new Vector3(x: 1, y: 0, z: 0);
        public static readonly Vector3 Front = new Vector3(x: 0, y: 0, z: -1);
    }
}
