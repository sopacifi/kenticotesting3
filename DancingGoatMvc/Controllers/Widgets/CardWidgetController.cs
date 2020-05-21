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

[assembly: RegisterWidget("DancingGoat.LandingPage.CardWidget", typeof(CardWidgetController), "{$dancinggoatmvc.widget.card.name$}", Description = "{$dancinggoatmvc.widget.card.description$}", IconClass = "icon-rectangle-paragraph")]

namespace DancingGoat.Controllers.Widgets
{
    public class CardWidgetController : WidgetController<CardWidgetProperties>
    {
        private readonly IMediaFileRepository mediaFileRepository;
        private readonly IOutputCacheDependencies outputCacheDependencies;


        /// <summary>
        /// Creates an instance of <see cref="CardWidgetController"/> class.
        /// </summary>
        /// <param name="mediaFileRepository">Repository for media files.</param>
        /// <param name="outputCacheDependencies">Output cache dependencies.</param>
        public CardWidgetController(IMediaFileRepository mediaFileRepository, IOutputCacheDependencies outputCacheDependencies)
        {
            this.mediaFileRepository = mediaFileRepository;
            this.outputCacheDependencies = outputCacheDependencies;
        }


        public ActionResult Index()
        {
            var properties = GetProperties();
            var image = GetImage(properties);

            outputCacheDependencies.AddDependencyOnInfoObject<MediaFileInfo>(image?.FileGUID ?? Guid.Empty);
            
            return PartialView("Widgets/_CardWidget", new CardWidgetViewModel
            {
                Image = image,
                Text = properties.Text
            });
        }


        private MediaFileInfo GetImage(CardWidgetProperties properties)
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