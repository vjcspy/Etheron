using System.Threading;
namespace Etheron.Utils.Thread
{
    // Object wrapper class cho struct
    public class ThreadSafeStructValue<T> where T : struct
    {
        private volatile Wrapper wrapper = new Wrapper(); // Initialize wrapper

        // Cập nhật giá trị mới cho struct
        public void Set(T newValue)
        {
            Wrapper location = wrapper;
            location.Value = newValue;

            Volatile.Write(location: ref location, value: location);
        }

        // Lấy giá trị của struct
        public T Get()
        {
            Wrapper location = wrapper;
            Wrapper wrapperCopy = Volatile.Read(location: ref location);

            return wrapperCopy.Value;
        }

        private class Wrapper
        {
            public T Value;
        }
    }
}
