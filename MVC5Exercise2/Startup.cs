using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVC5Exercise2.Startup))]
namespace MVC5Exercise2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
