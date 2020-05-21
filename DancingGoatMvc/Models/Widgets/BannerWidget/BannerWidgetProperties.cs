using System;
using System.Collections.Generic;

using CMS.SiteProvider;

using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

namespace DancingGoat.Models.Widgets
{
    /// <summary>
    /// Banner widget properties.
    /// </summary>
    public class BannerWidgetProperties : IWidgetProperties
    {
        private string mLinkUrl;


        /// <summary>
        /// Guid of an image to be displayed.
        /// </summary>
        public IList<MediaFilesSelectorItem> Image { get; set; }


        /// <summary>
        /// Text to be displayed.
        /// </summary>
        public string Text { get; set; }


        /// <summary>
        /// Gets or sets URL to which a visitor is redirected after clicking on the <see cref="Text"/>.
        /// </summary>
        [EditingComponent(TextInputComponent.IDENTIFIER, Order = 0, Label = "{$DancingGoatMVC.Widget.Banner.LinkUrl$}")]
        [EditingComponentProperty(nameof(TextInputProperties.Placeholder), "https://www.example.com")]
        public string LinkUrl
        {
            get
            {
                return mLinkUrl;
            }
            set
            {
                mLinkUrl = GetNormalizedUrl(value, SiteContext.CurrentSite);
            }
        }


        /// <summary>
        /// Gets or sets a title for a link defined by <see cref="LinkUrl"/>.
        /// </summary>
        /// <remarks>
        /// If URL targets a page in the site then URL is stored in a given format '~/en-us/article'.
        /// </remarks>
        [EditingComponent(TextInputComponent.IDENTIFIER, Order = 1, Label = "{$DancingGoatMVC.Widget.Banner.LinkTitle$}")]
        public string LinkTitle { get; set; } = String.Empty;


        private string GetNormalizedUrl(string url, SiteInfo site)
        {
            if (String.IsNullOrEmpty(url))
            {
                return url;
            }
            
            if (!site.SiteIsContentOnly || String.IsNullOrEmpty(site.SitePresentationURL) || !url.StartsWith(site.SitePresentationURL, StringComparison.OrdinalIgnoreCase) || !Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                return url;
            }

            return $"~{url.Substring(site.SitePresentationURL.Length)}";
        }
    }
}