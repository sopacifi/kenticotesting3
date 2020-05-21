using System;
using System.Linq;
using System.Web.Mvc;

using CMS.MediaLibrary;
using CMS.SiteProvider;

using DancingGoat.Controllers.Widgets;
using DancingGoat.Infrastructure;
using DancingGoat.Models.Widgets;
using DancingGoat.Repositories;

using Kentico.PageBuilder.Web.Mvc;

[assembly: RegisterWidget("DancingGoat.LandingPage.HeroImage", typeof(HeroImageWidgetController), "{$dancinggoatmvc.widget.heroimage.name$}", Description = "{$dancinggoatmvc.widget.heroimage.description$}", IconClass = "icon-badge")]

namespace DancingGoat.Controllers.Widgets
{
    public class HeroImageWidgetController : WidgetController<HeroImageWidgetProperties>
    {
        private readonly IMediaFileRepository mediaFileRepository;
        private readonly IOutputCacheDependencies outputCacheDependencies;


        /// <summary>
        /// Creates an instance of <see cref="BannerWidgetController"/> class.
        /// </summary>
        /// <param name="mediaFileRepository">Repository for media files.</param>
        /// <param name="outputCacheDependencies">Output cache dependencies.</param>
        public HeroImageWidgetController(IMediaFileRepository mediaFileRepository, IOutputCacheDependencies outputCacheDependencies)
        {
            this.mediaFileRepository = mediaFileRepository;
            this.outputCacheDependencies = outputCacheDependencies;
        }


        public ActionResult Index()
        {
            var properties = GetProperties();
            var image = GetImage(properties);

            outputCacheDependencies.AddDependencyOnInfoObject<MediaFileInfo>(image?.FileGUID ?? Guid.Empty);

            return PartialView("Widgets/_HeroImageWidget", new HeroImageWidgetViewModel
            {
                Image = image,
                Text = properties.Text,
                ButtonText = properties.ButtonText,
                ButtonTarget = properties.ButtonTarget,
                Theme = properties.Theme
            });
        }


        private MediaFileInfo GetImage(HeroImageWidgetProperties properties)
        {
            var imageGuid = properties.Image?.FirstOrDefault()?.FileGuid ?? Guid.Empty;
            if (imageGuid == Guid.Empty)
            {
                return null;
            }

            return mediaFileRepository.GetMediaFile(imageGuid, SiteContext.CurrentSiteName);
        }
    }
}