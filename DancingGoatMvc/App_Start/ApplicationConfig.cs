using Kentico.Web.Mvc;
using Kentico.Activities.Web.Mvc;
using Kentico.CampaignLogging.Web.Mvc;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.Newsletters.Web.Mvc;
using Kentico.OnlineMarketing.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

using DancingGoat.PageTemplatesFilters;

namespace DancingGoat
{
    /// <summary>
    /// Class for application configuration.
    /// </summary>
    public class ApplicationConfig
    {
        /// <summary>
        /// Registers features into given <paramref name="builder"/>.
        /// </summary>
        public static void RegisterFeatures(IApplicationBuilder builder)
        {
            builder.UsePreview();

            builder.UsePageBuilder(new PageBuilderOptions
            {
                DefaultSectionIdentifier = "DancingGoat.SingleColumnSection",
                RegisterDefaultSection = false
            });

            builder.UseDataAnnotationsLocalization();
            builder.UseResourceSharingWithAdministration();
            builder.UseCampaignLogger();
            builder.UseActivityTracking();
            builder.UseABTesting();
            builder.UseEmailTracking(new EmailTrackingOptions());
            builder.UsePageRouting(new PageRoutingOptions
            {
                EnableAlternativeUrls = true
            });


            // Register page template filers into the global page template filter collection.
            PageBuilderFilters.PageTemplates.Add(new LandingPageTemplatesFilter());
            PageBuilderFilters.PageTemplates.Add(new ArticlePageTemplatesFilter());
        }
    }
}