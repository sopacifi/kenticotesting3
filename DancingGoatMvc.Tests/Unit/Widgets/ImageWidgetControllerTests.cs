using System;

using CMS.DocumentEngine;
using CMS.SiteProvider;
using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Tests;

using Kentico.PageBuilder.Web.Mvc;

using DancingGoat.Controllers.Widgets;
using DancingGoat.Models.Widgets;
using DancingGoat.Infrastructure;

using Tests.DocumentEngine;

using NSubstitute;
using NUnit.Framework;
using TestStack.FluentMVCTesting;

namespace DancingGoat.Tests.Unit
{
    public class ImageWidgetControllerTests : UnitTests
    {
        private const string PARTIAL_VIEW_NAME = "Widgets/_ImageWidget";
        private const int SITE_ID = 1;

        private Article page;
        private ImageWidgetController controller;
        private readonly IComponentPropertiesRetriever<ImageWidgetProperties> propertiesRetriever = Substitute.For<IComponentPropertiesRetriever<ImageWidgetProperties>>();
        private readonly ICurrentPageRetriever currentPageRetriever = Substitute.For<ICurrentPageRetriever>();
        private readonly IOutputCacheDependencies outputCacheDependencies = Substitute.For<IOutputCacheDependencies>();
        private readonly Guid attachmentGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");


        [SetUp]
        public void SetUp()
        {
            Fake().DocumentType<Article>(Article.CLASS_NAME);
            page = new Article
            {
                DocumentName = "Name"
            };
            currentPageRetriever.Retrieve(Arg.Any<IPageBuilderFeature>()).Returns(page);

            Fake<SiteInfo, SiteInfoProvider>().WithData(
                new SiteInfo
                {
                    SiteID = SITE_ID,
                    SiteName = "Site"
                });
            Fake<AttachmentInfo, AttachmentInfoProvider>().WithData(
                new AttachmentInfo
                {
                    AttachmentGUID = attachmentGuid,
                    AttachmentDocumentID = page.DocumentID,
                    AttachmentSiteID = SITE_ID
                });

            controller = new ImageWidgetController(propertiesRetriever, currentPageRetriever, outputCacheDependencies);
            controller.ControllerContext = ControllerContextMock.GetControllerContext(controller);
        }


        [Test]
        public void Index_RetrieveExistingDocumentAttachment_ReturnsCorrectModel()
        {
            propertiesRetriever
                .Retrieve()
                .Returns(new ImageWidgetProperties { ImageGuid = attachmentGuid });

            controller.WithCallTo(c => c.Index())
                .ShouldRenderPartialView(PARTIAL_VIEW_NAME)
                .WithModel<ImageWidgetViewModel>(m => 
                {
                    Assert.That(m.Image.AttachmentGUID, Is.EqualTo(attachmentGuid));
                });
        }


        [Test]
        public void Index_RetrieveNotExistingDocumentAttachment_ReturnsModelWithEmptyImage()
        {
            propertiesRetriever
                .Retrieve()
                .Returns(new ImageWidgetProperties { ImageGuid = Guid.Parse("00000000-0000-0000-0000-000000000002") });

            controller.WithCallTo(c => c.Index())
                      .ShouldRenderPartialView(PARTIAL_VIEW_NAME)
                      .WithModel<ImageWidgetViewModel>(m => m.Image == null);
        }
    }
}
