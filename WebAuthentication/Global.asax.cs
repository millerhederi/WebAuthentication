using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Http.Filters;
using Autofac;
using Autofac.Integration.WebApi;
using WebAuthentication.Filters;
using WebAuthentication.Services;

namespace WebAuthentication
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var builder = new ContainerBuilder();

            var config = GlobalConfiguration.Configuration;

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<PasswordHasher>().As<IPasswordHasher>().SingleInstance();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            ConfigureGlobalFilters(config.Filters);
        }

        private static void ConfigureGlobalFilters(HttpFilterCollection filters)
        {
            filters.Add(new AuthenticationFilterAttribute());
        }
    }
}