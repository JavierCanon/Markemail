using System.Web.Http.Dependencies;
using Ninject;
using Ninject.Web.WebApi;

namespace Eron.MailClient.Presentation.WebApplication.WebApi.Infrastructure
{
    public class NinjectDependencyResolver : NinjectDependencyScope, System.Web.Http.Dependencies.IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel)
            : base(kernel)
        {
            this._kernel = kernel;
        }

        public IDependencyScope BeginScope()
        {
            return new NinjectDependencyScope(_kernel.BeginBlock());
        }
    }
}