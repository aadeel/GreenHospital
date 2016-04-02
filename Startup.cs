using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DoctorPatient.Startup))]
namespace DoctorPatient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
