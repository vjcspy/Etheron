using Colyseus.Schema;
namespace Etheron.Colyseus.Schemas
{
    public class PlayerContext : Schema
    {
        [Type(index: 0, type: "uint8")]
        public byte State = 0;
    }

    public class PlayerPosition : Schema
    {
        [Type(index: 0, type: "number")]
        public float X = 0f;

        [Type(index: 1, type: "number")]
        public float Y = 0f;
    }

    public class Player : Schema
    {

        [Type(index: 2, type: "ref", childType: typeof(PlayerContext))]
        public PlayerContext Context = new PlayerContext();

        [Type(index: 0, type: "string")]
        public string Id = "";

        [Type(index: 1, type: "ref", childType: typeof(PlayerPosition))]
        public PlayerPosition Position = new PlayerPosition();
    }
}
