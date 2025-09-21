using SharpWebview;
using SharpWebview.Content;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using GrindBreaker.Configuration;
using GrindBreaker.RPC;
using System;

namespace GrindBreaker
{
    class Program
    {

        // path to the SPA index.html file
        private readonly static string _buildPath = "";

        [STAThread]
        static void Main(string[] args)
        {
            // Configure dependency injection
            var services = new ServiceCollection();
            services.ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            
            // Resolve RPC services
            var profileRPC = serviceProvider.GetRequiredService<ProfileRPC>();
            var candidacyRPC = serviceProvider.GetRequiredService<CandidacyRPC>();
            
            // get local flag
            var local = args.Contains("--local");
            var uiAddress = local ? GetNetworkLocalIPAddress() : _buildPath;
            using var window = new WebviewWrapper(new Webview(true));
            window
                .WebviewInstance
                .SetTitle("Grind Breaker")
                .SetSize(1920, 1080, WebviewHint.None)
                .Bind("GRIND_BREAKER_GetProfile", (id, req) => profileRPC.GetProfile(window, id, req))
                .Bind("GRIND_BREAKER_SaveProfile", (id, req) => profileRPC.SaveProfile(window, id, req))
                .Bind("GRIND_BREAKER_GetAllCandidacies", (id, req) => candidacyRPC.GetAllCandidacies(window, id, req))
                .Bind("GRIND_BREAKER_GetCandidacy", (id, req) => candidacyRPC.GetCandidacy(window, id, req))
                .Bind("GRIND_BREAKER_SaveCandidacy", (id, req) => candidacyRPC.SaveCandidacy(window, id, req))
                .Bind("GRIND_BREAKER_UpdateCandidacy", (id, req) => candidacyRPC.UpdateCandidacy(window, id, req))
                .Bind("GRIND_BREAKER_DeleteCandidacy", (id, req) => candidacyRPC.DeleteCandidacy(window, id, req))
                .Bind("GRIND_BREAKER_UpdateCandidacyStatus", (id, req) => candidacyRPC.UpdateCandidacyStatus(window, id, req))
                .Navigate(new UrlContent(uiAddress))
                .Run();
        }

        private static string GetNetworkLocalIPAddress()
        {
            var ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            return $"http://{ipAddress}:3000";
        }
    }
}
