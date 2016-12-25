using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RiskApp.Startup))]
namespace RiskApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
