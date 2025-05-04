// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 3.0.35
// 

using Colyseus.Schema;
#if UNITY_5_3_OR_NEWER
using UnityEngine.Scripting;
#endif

namespace Etheron.Colyseus.Schemas {
	public partial class Position : Schema {
#if UNITY_5_3_OR_NEWER
[Preserve]
#endif
public Position() { }
		[Type(0, "ref", typeof(Vector3))]
		public Vector3 value = null;

		[Type(1, "ref", typeof(Vector3))]
		public Vector3 facingDirection = null;

		[Type(2, "float32")]
		public float timestamp = default(float);
	}
}
