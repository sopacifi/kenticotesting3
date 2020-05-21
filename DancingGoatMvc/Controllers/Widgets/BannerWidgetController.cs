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

[assembly: RegisterWidget("DancingGoat.HomePage.BannerWidget", typeof(BannerWidgetController), "{$dancinggoatmvc.widget.banner.name$}", Description = "{$dancinggoatmvc.widget.banner.description$}", IconClass = "icon-ribbon")]

namespace DancingGoat.Controllers.Widgets
{
    public class BannerWidgetController : WidgetController<BannerWidgetProperties>
    {
        private readonly IMediaFileRepository mediaFileRepository;
        private readonly IOutputCacheDependencies outputCacheDependencies;


        /// <summary>
        /// Creates an instance of <see cref="BannerWidgetController"/> class.
        /// </summary>
        /// <param name="mediaFileRepository">Repository for media files.</param>
        public BannerWidgetController(IMediaFileRepository mediaFileRepository, IOutputCacheDependencies outputCacheDependencies)
        {
            this.mediaFileRepository = mediaFileRepository;
            this.outputCacheDependencies = outputCacheDependencies;
        }


        /// <summary>
        /// Creates an instance of <see cref="BannerWidgetController"/> class.
        /// </summary>
        /// <param name="mediaFileRepository">Repository for media files.</param>
        /// <param name="propertiesRetriever">Retriever for widget properties.</param>
        /// <param name="currentPageRetriever">Retriever for current page where is the widget used.</param>
        /// <remarks>Use this constructor for tests to handle dependencies.</remarks>
        public BannerWidgetController(IMediaFileRepository mediaFileRepository, IComponentPropertiesRetriever<BannerWidgetProperties> propertiesRetriever, ICurrentPageRetriever currentPageRetriever, IOutputCacheDependencies outputCacheDependencies) 
            : base(propertiesRetriever, currentPageRetriever)
        {
            this.mediaFileRepository = mediaFileRepository;
            this.outputCacheDependencies = outputCacheDependencies;
        }


        public ActionResult Index()
        {
            var properties = GetProperties();
            var image = GetImage(properties);
            
            return PartialView("Widgets/_BannerWidget", new BannerWidgetViewModel
            {
                Image = image,
                Text = properties.Text,
                LinkUrl = properties.LinkUrl,
                LinkTitle = properties.LinkTitle
            });
        }


        private MediaFileInfo GetImage(BannerWidgetProperties properties)
        {
            var imageGuid = properties.Image?.FirstOrDefault()?.FileGuid ?? Guid.Empty;
            if (imageGuid == Guid.Empty)
            {
                return null;
            }

            outputCacheDependencies.AddDependencyOnInfoObject<MediaFileInfo>(imageGuid);

            return mediaFileRepository.GetMediaFile(imageGuid, SiteContext.CurrentSiteName);
        }
    }
}