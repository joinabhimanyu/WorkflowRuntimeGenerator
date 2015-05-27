using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using WorkflowRuntimeGenerator;
using System.Data.Entity;
using Microsoft.AspNet.FriendlyUrls;

//using WebFormsDemo.Models;

namespace WorkflowRuntimeGenerator
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
            //AuthConfig.RegisterOpenAuth();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //Database.SetInitializer(new ProductDatabseInitializer());

            //Local Configurator page
            string password = "ins555";
            string strLVMaxPool = "100";
            string constr = string.Format("User ID=ins;Password={0};Data Source=conf;max pool size={1}",password.ToString().Trim(),strLVMaxPool.ToString().Trim());
            Application["con"] = constr;

        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            Session["appuserid"] = "UIIC";
            Session["usergrpid"] = "UIIC";

        }
    }
}
