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
	public partial class SyncObject : Schema {
#if UNITY_5_3_OR_NEWER
[Preserve]
#endif
public SyncObject() { }
		[Type(0, "string")]
		public string id = default(string);

		[Type(1, "string")]
		public string clientId = default(string);

		[Type(2, "string")]
		public string serverId = default(string);

		[Type(3, "ref", typeof(Position))]
		public Position position = null;
	}
}
