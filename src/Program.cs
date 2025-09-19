using SharpWebview;
using SharpWebview.Content;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace GrindBreaker
{
    class Program
    {

        // path to the SPA index.html file
        private readonly static string _buildPath = "";

        [STAThread]
        static void Main(string[] args)
        {
            // get local flag
            var local = args.Contains("--local");
            var uiAddress = local ? GetNetworkLocalIPAddress() : _buildPath;
            using var window = new Webview();
            window.SetTitle("Grind Breaker");
            window.SetSize(1920, 1080, WebviewHint.None);
            window.Navigate(new UrlContent(uiAddress));
            window.Run();
        }

        private static string GetNetworkLocalIPAddress()
        {
            var ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            return $"http://{ipAddress}:3000";
        }
    }
}
