using Microsoft.AspNetCore.Hosting;
using SForum.Areas.Identity;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]

namespace SForum.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => { });
        }
    }
}