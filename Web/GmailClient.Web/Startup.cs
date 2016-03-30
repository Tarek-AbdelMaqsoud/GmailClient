using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GmailClient.Web.Startup))]
namespace GmailClient.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
