using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ClassProject.Startup))]
namespace ClassProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
