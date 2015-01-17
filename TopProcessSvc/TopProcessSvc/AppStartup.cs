using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using TopProcessSvc;

[assembly: OwinStartup(typeof (AppStartup))]

namespace TopProcessSvc
{
    public class AppStartup
    {
        public void Configuration(IAppBuilder app)
        {
            // CORS is enabled by default
            app.MapSignalR(new HubConfiguration());
        }
    }
}