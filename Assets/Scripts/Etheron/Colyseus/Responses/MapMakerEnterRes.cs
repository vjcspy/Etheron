using System;
namespace Etheron.Colyseus.Responses
{

    [Serializable]
    public class MapMakerEnterRes
    {
        public Room room;
        public string sessionId;
    }

    [Serializable]
    public class Room
    {
        public int clients;
        public bool locked;
        public bool @private; // `private` là keyword, nên dùng `@private`
        public int maxClients;
        public bool unlisted;
        public string createdAt; // Không dùng DateTime ở đây
        public string name;
        public string processId;
        public string publicAddress;
        public string roomId;
    }
}
