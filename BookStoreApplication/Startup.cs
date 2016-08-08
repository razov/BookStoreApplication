using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(BookStoreApplication.Startup))]

namespace BookStoreApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        }
    }
}
