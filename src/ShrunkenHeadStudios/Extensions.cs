using ShrunkenHeadStudios.Core.Objects;
using System;
using System.Configuration;
using System.Web.Mvc;

namespace ShrunkenHeadStudios
{
    public static class Extensions
    {
        public static string Href(this Post post, UrlHelper helper)
        {
            return helper.RouteUrl(new { controller = "Blog", action = "Post", year = post.PostedOn.Year, month = post.PostedOn.Month, title = post.UrlSlug });
        }
    }
}