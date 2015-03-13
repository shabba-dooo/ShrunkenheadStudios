using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using ShrunkenHeadStudios.Core.Objects;
using NHibernate;
using NHibernate.Cache;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using NHibernate.Tool.hbm2ddl;

namespace ShrunkenHeadStudios.Core
{
     public class RepositoryModule: NinjectModule
    {
         public override void Load()
         {
             //Bind - map an interface to a class that implements it
             //Setting DB connecion string 
             //setting a cache for queries
             //exposeconfiguration - asking NHibernate to create tables from clases
             //mappings - Specifying the assembly where the domain & mapping classes exists 
             //Creates tables in DB - Removed to avoid tables being recreated again
             Bind<ISessionFactory>()
             .ToMethod(e => Fluently.Configure()
             .Database(MsSqlConfiguration.MsSql2008.ConnectionString(c => c.FromConnectionStringWithKey("ShrunkenHeadStudiosBlogDbConnString")))
             .Cache(c => c.UseQueryCache().ProviderClass<HashtableCacheProvider>())
             .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Post>())
            //.ExposeConfiguration(cfg => new SchemaExport(cfg).Execute(true, true, false))
             .BuildConfiguration()
             .BuildSessionFactory())
             .InSingletonScope();

             Bind<ISession>()
                 .ToMethod((ctx) => ctx.Kernel.Get<ISessionFactory>().OpenSession())
                 .InRequestScope();
         }
    }
}
