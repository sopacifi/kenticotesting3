using System;
using System.Linq;
using System.Web.Mvc;

using CMS.DocumentEngine;

using Kentico.PageBuilder.Web.Mvc;

using DancingGoat.Controllers.Widgets;
using DancingGoat.Models.Widgets;
using DancingGoat.Infrastructure;

[assembly: RegisterWidget("DancingGoat.HomePage.ImageWidget", typeof(ImageWidgetController), "{$dancinggoatmvc.widget.image.name$}", Description = "{$dancinggoatmvc.widget.image.description$}", IconClass = "icon-picture")]

namespace DancingGoat.Controllers.Widgets
{
    public class ImageWidgetController : WidgetController<ImageWidgetProperties>
    {
        private readonly IOutputCacheDependencies outputCacheDependencies;

        /// <summary>
        /// Creates an instance of <see cref="ImageWidgetController"/> class.
        /// </summary>
        /// <param name="outputCacheDependencies">Output cache dependencies.</param>
        public ImageWidgetController(IOutputCacheDependencies outputCacheDependencies)
        {
            this.outputCacheDependencies = outputCacheDependencies;
        }


        /// <summary>
        /// Creates an instance of <see cref="ImageWidgetController"/> class.
        /// </summary>
        /// <param name="propertiesRetriever">Retriever for widget properties.</param>
        /// <param name="currentPageRetriever">Retriever for current page where is the widget used.</param>
        /// <param name="outputCacheDependencies">Output cache dependencies.</param>
        /// <remarks>Use this constructor for tests to handle dependencies.</remarks>
        public ImageWidgetController(IComponentPropertiesRetriever<ImageWidgetProperties> propertiesRetriever, ICurrentPageRetriever currentPageRetriever, IOutputCacheDependencies outputCacheDependencies) 
            : base(propertiesRetriever, currentPageRetriever)
        {
            this.outputCacheDependencies = outputCacheDependencies;
        }


        public ActionResult Index()
        {
            var properties = GetProperties();
            var image = GetImage(properties);

            outputCacheDependencies.AddDependencyOnPageAttachmnent(image?.AttachmentGUID ?? Guid.Empty);

            return PartialView("Widgets/_ImageWidget", new ImageWidgetViewModel
            {
                Image = image
            });
        }


        private DocumentAttachment GetImage(ImageWidgetProperties properties)
        {
            var page = GetPage();
            return page?.AllAttachments.FirstOrDefault(x => x.AttachmentGUID == properties.ImageGuid);
            
        }
    } 
}