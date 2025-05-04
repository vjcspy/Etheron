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
	public partial class EntityVisualization : Schema {
#if UNITY_5_3_OR_NEWER
[Preserve]
#endif
public EntityVisualization() { }
		[Type(0, "uint8")]
		public byte state = default(byte);
	}
}
