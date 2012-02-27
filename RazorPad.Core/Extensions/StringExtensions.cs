using System.IO;
using System.Text;
using System.Web.Mvc;
using System;
using System.Web;

namespace RazorPad.Core
{
    public static class StringExtensions
    {
        public static TextReader ToTextReader(this string source)
        {
            var stream = new MemoryStream(Encoding.Default.GetBytes(source ?? string.Empty));
            return new StreamReader(stream);
        }

        public static string AbsoluteAction(this UrlHelper url, string actionName, string controllerName, object routeValues)
        {
            return url.Action(actionName, controllerName, routeValues, "http");
        }

        public static string AbsoluteContent(this UrlHelper url, string path)
        {
            Uri uri = new Uri(path, UriKind.RelativeOrAbsolute);

            //If the URI is not already absolute, rebuild it based on the current request.
            if (!uri.IsAbsoluteUri)
            {
                UriBuilder builder = new UriBuilder(url.RequestContext.HttpContext.Request.Url);

                //All that needs to change is the path portion.
                builder.Path = VirtualPathUtility.ToAbsolute(path);

                uri = builder.Uri;
            }

            return uri.ToString();
        }
    }
}