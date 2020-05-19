using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(IdentityMvc.Areas.Identity.IdentityHostingStartup))]
namespace IdentityMvc.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}