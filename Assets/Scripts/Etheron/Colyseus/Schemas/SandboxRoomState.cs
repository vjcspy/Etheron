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
	public partial class SandboxRoomState : Schema {
#if UNITY_5_3_OR_NEWER
[Preserve]
#endif
public SandboxRoomState() { }
		[Type(0, "map", typeof(MapSchema<Player>))]
		public MapSchema<Player> players = null;
	}
}
