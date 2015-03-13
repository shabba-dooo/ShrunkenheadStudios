using Ninject;
using Ninject.Web.Common;
using ShrunkenHeadStudios.Core;
using ShrunkenHeadStudios.Core.Objects;
using ShrunkenHeadStudios.Providers;
using System.Web.Mvc;
using System.Web.Routing;

namespace ShrunkenHeadStudios
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : NinjectHttpApplication
    {
        protected override IKernel CreateKernel()
        {
            var Kernel = new StandardKernel();

            Kernel.Load(new RepositoryModule());
            Kernel.Bind<IBlogRepository>().To<BlogRepository>();
            Kernel.Bind<IAuthProvider>().To<AuthProvider>();

            return Kernel;
        }

        protected override void OnApplicationStarted()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            base.OnApplicationStarted();
            ModelBinders.Binders.Add(typeof(Post), new PostModelBinder(Kernel));
        }
    }
}