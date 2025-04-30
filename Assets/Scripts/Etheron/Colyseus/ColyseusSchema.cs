using Colyseus.Schema;
namespace Etheron.Colyseus
{
    public class ColyseusSchema
    {
        public class Vector3 : Schema
        {
            [Type(index: 0, type: "number")]
            public float x = 0;

            [Type(index: 1, type: "number")]
            public float y = 0;

            [Type(index: 2, type: "number")]
            public float z = 0;
        }

        public class PlayerVisualization : Schema
        {
            [Type(index: 0, type: "uint8")]
            public byte state = 0;
        }

        public class Player : Schema
        {
            [Type(index: 0, type: "string")]
            public string id = "";

            [Type(index: 1, type: "ref", childType: typeof(Vector3))]
            public Vector3 position = new Vector3();

            [Type(index: 2, type: "ref", childType: typeof(PlayerVisualization))]
            public PlayerVisualization visualization = new PlayerVisualization();
        }

        public class SandboxRoomState : Schema
        {
            [Type(index: 0, type: "map", childType: typeof(Player))]
            public MapSchema<Player> players = new MapSchema<Player>();
        }

        public class LobbyRoomState : Schema
        {
            [Type(index: 0, type: "map", childType: typeof(Player))]
            public MapSchema<Player> players = new MapSchema<Player>();
        }
    }
}
