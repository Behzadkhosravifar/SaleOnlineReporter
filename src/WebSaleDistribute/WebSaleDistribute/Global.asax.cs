﻿using System;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AdoManager;

namespace WebSaleDistribute
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            SetConnection();

            //Error += Application_Error;
        }

        private void SetConnection()
        {            
#if DEBUG // If(Debugger.IsAttached)
            ConnectionManager.SetToDefaultConnection(Connections.UsersManagements.Connection.Name); // local
#else
            ConnectionManager.SetToDefaultConnection(Connections.UsersManagements.Connection.Name); // server
#endif
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

            // Get the exception object.
            Exception exc = Server.GetLastError();

            if (exc == null)
                return;

            // Clear the error from the server
            Server.ClearError();

            // Handle HTTP errors
            if (exc.GetType() == typeof(HttpException))
            {
                // The Complete Error Handling Example generates
                // some errors using URLs with "NoCatch" in them;
                // ignore these here to simulate what would happen
                // if a global.asax handler were not implemented.
                if (exc.Message.Contains("NoCatch") || exc.Message.Contains("maxUrlLength"))
                    return;

                //Redirect HTTP errors to HttpError page
                //Server.Transfer("~/Views/Errors");

                if(exc.Message.Contains("was not found"))
                {
                    Response.Redirect("~/error/NotFound");
                    return;
                }

                Response.Redirect("~/error/index");

                //Add controller name
                //RouteData routeData = new RouteData();
                //routeData.Values.Add("controller", "Errors");

                ////we will add controller's action name 
                //routeData.Values.Add("action", "e404");

                //// Pass exception details to the target error View.
                //routeData.Values.Add("error", exc.Message);

                //// Clear the error on server.
                //Server.ClearError();

                //// Call target Controller and pass the routeData.
                //IController errorController = new ErrorsController();
                //errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));

                return;
            }

            

            // Redirect to a landing page
            Response.Redirect("~/home");
        }
    }
}
