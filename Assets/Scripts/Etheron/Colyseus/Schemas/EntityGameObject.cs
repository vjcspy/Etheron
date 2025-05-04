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
	public partial class EntityGameObject : SyncObject {
#if UNITY_5_3_OR_NEWER
[Preserve]
#endif
public EntityGameObject() { }
		[Type(3, "ref", typeof(Position))]
		public Position position = null;

		[Type(4, "ref", typeof(EntityVisualization))]
		public EntityVisualization visualization = null;
	}
}
