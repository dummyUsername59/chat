using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using ChatApplication.DataProvider;
using System.Web.Optimization;
using System.Web.Routing;

namespace ChatApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public object Cacheprovider { get; private set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            InitDefaultRooms();
        }
        void InitDefaultRooms()
        {
            var createdDate = DateTime.Now;
            foreach (var v in Enumerable.Range(1, 10))
            {
                CacheProvider.Instance.Set(Guid.NewGuid().ToString(), new KeyValuePair<string, DateTime>("Room"+v,createdDate));
            }
            
        }
    }
}
