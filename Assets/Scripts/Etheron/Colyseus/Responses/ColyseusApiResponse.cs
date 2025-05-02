namespace Etheron.Colyseus.Responses
{
    public class ColyseusError
    {
        public string code;
        public string message;
    }

    public class ColyseusApiResponse<T>
    {
        public T data;
        public ColyseusError error;
    }
}
