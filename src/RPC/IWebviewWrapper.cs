using GrindBreaker.RPC.Models;

namespace GrindBreaker.RPC
{
    public interface IWebviewWrapper
    {
        void Return(string id, RPCResultType result, string json);
    }
}
