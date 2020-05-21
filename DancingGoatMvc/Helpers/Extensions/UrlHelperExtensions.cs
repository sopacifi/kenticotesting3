using System;
using System.Web.Mvc;

using Kentico.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;

namespace DancingGoat.Helpers
{
    /// <summary>
    /// Extension methods for <see cref="UrlHelper"/> class.
    /// </summary>
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Generates a fully qualified URL to the action method handling the detail of the given article.
        /// </summary>
        /// <param name="urlHelper">URL Helper</param>
        /// <param name="article">Article model to generate URL for.</param>
        public static string ForArticle(this UrlHelper urlHelper, Guid nodeGuid, string nodeAlias)
        {
            return urlHelper.Action("Show", "Articles", new
            {
                guid = nodeGuid,
                pageAlias = nodeAlias
            });
        }


        /// <summary>
        /// Returns canonical URL of current page.
        /// </summary>
        /// <param name="urlHelper">URL Helper</param>
        public static string CanonicalUrl(this UrlHelper urlHelper)
        {
            var pageMainUrl = urlHelper.Kentico().PageMainUrl();
            var currentUrl = urlHelper.RequestContext.HttpContext.Request.Url;

            if (String.IsNullOrEmpty(pageMainUrl))
            {
                return currentUrl.GetLeftPart(UriPartial.Path).TrimEnd('/');
            }

            return new Uri(
                new Uri(currentUrl.GetLeftPart(UriPartial.Authority)),
                urlHelper.Content(pageMainUrl)
                ).AbsoluteUri.TrimEnd('/');
        }
    }
}