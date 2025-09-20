using GrindBreaker.RPC.Models;
using SharpWebview;

namespace GrindBreaker.RPC
{
    // This is dirty but provides a way to mock the webview instance in testing
    public class WebviewWrapper : IWebviewWrapper, IDisposable
    {
        public Webview WebviewInstance {get; private set;}

        public WebviewWrapper(Webview webview)
        {
            WebviewInstance = webview;
        }

        public void Return(string id, RPCResultType result, string json)
        {
            WebviewInstance.Return(id, (SharpWebview.RPCResult)result, json);
        }

        public void Dispose()
        {
            WebviewInstance.Dispose();
        }
    }
}
