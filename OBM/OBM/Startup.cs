using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OBM.Startup))]
namespace OBM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
