using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ShrunkenHeadStudios
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //Route for list of posts
            routes.MapRoute("Posts", "Blog/{action}", new { controller = "Blog", action = "Posts" });

            //Route for list of categories
            routes.MapRoute("Category", "Category/{category}", new { controller = "Blog", action = "Category" });

            //Route for list of tags
            routes.MapRoute("Tag", "Tag/{tag}", defaults: new { controller = "Blog", action = "Tag" });

            //Route for a single post
            routes.MapRoute("Post", "Archive/{year}/{month}/{title}", new { controller = "Blog", action = "Post" });

            //Login route
            routes.MapRoute("Login", "Login", new { controller = "Admin", action = "Login" });

            //Logout route
            routes.MapRoute("Logout", "Logout", new { controller = "Admin", action = "Logout" });

            //Admin control panel route
            routes.MapRoute("Manage", "Manage", new { controller = "Admin", action = "Manage" });

            //Route for managing posts
            routes.MapRoute("AdminAction", "Admin/{action}", new { controller = "Admin", action = "Login" });

            //Default route
            routes.MapRoute("Default", "{controller}/{action}/{id}", defaults: new { controller = "Blog", action = "Index", id = UrlParameter.Optional });
        }
    }
}