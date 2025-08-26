using System.Threading;

namespace BlendJson.Serialization
{
    public class GlobalContext
    {
        private int _id = -1;

        public int GetFileId() => Interlocked.Increment(ref _id);
    }
}
