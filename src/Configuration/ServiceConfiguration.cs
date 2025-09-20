using Microsoft.Extensions.DependencyInjection;
using GrindBreaker.Repositories;
using GrindBreaker.RPC;

namespace GrindBreaker.Configuration
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            // Register repositories
            services.AddSingleton<IProfileRepository, ProfileRepository>();
            
            // Register RPC classes
            services.AddSingleton<ProfileRPC>();
            
            return services;
        }
    }
}
