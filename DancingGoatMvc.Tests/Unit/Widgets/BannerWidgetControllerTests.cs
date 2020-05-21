using System;
using System.Collections.Generic;

using CMS.SiteProvider;
using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Tests;
using CMS.MediaLibrary;

using Kentico.PageBuilder.Web.Mvc;
using Kentico.Components.Web.Mvc.FormComponents;

using DancingGoat.Controllers.Widgets;
using DancingGoat.Models.Widgets;
using DancingGoat.Repositories;
using DancingGoat.Infrastructure;

using Tests.DocumentEngine;

using NSubstitute;
using NUnit.Framework;
using TestStack.FluentMVCTesting;

namespace DancingGoat.Tests.Unit
{
    public class BannerWidgetControllerTests : UnitTests
    {
        private const string PARTIAL_VIEW_NAME = "Widgets/_BannerWidget";
        private const int SITE_ID = 1;
        private const string BANNER_TEXT = "Banner text";

        private Article page;
        private BannerWidgetController controller;
        private readonly IMediaFileRepository mediaFileRepository = Substitute.For<IMediaFileRepository>();
        private readonly IOutputCacheDependencies outputCacheDependencies = Substitute.For<IOutputCacheDependencies>();
        private readonly IComponentPropertiesRetriever<BannerWidgetProperties> propertiesRetriever = Substitute.For<IComponentPropertiesRetriever<BannerWidgetProperties>>();
        private readonly ICurrentPageRetriever currentPageRetriever = Substitute.For<ICurrentPageRetriever>();
        private readonly Guid fileGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");


        [SetUp]
        public void SetUp()
        {
            Fake<MediaFileInfo>();
            Fake().DocumentType<Article>(Article.CLASS_NAME);
            page = new Article
            {
                DocumentName = "Name"
            };
            currentPageRetriever.Retrieve(Arg.Any<IPageBuilderFeature>()).Returns(page);
            mediaFileRepository.GetMediaFile(fileGuid, Arg.Any<string>()).Returns(new MediaFileInfo
            {
                FileGUID = fileGuid,
                FileSiteID = SITE_ID
            });

            Fake<SiteInfo, SiteInfoProvider>().WithData(
                new SiteInfo
                {
                    SiteID = SITE_ID,
                    SiteName = "Site"
                });

            controller = new BannerWidgetController(mediaFileRepository, propertiesRetriever, currentPageRetriever, outputCacheDependencies);
            controller.ControllerContext = ControllerContextMock.GetControllerContext(controller);
        }


        [Test]
        public void Index_RetrieveExistingFile_ReturnsCorrectModel()
        {
            propertiesRetriever
                .Retrieve()
                .Returns(new BannerWidgetProperties { Image = new List<MediaFilesSelectorItem> { new MediaFilesSelectorItem { FileGuid = fileGuid } }, Text = BANNER_TEXT });

            controller.WithCallTo(c => c.Index())
                .ShouldRenderPartialView(PARTIAL_VIEW_NAME)
                .WithModel<BannerWidgetViewModel>(m => {
                    Assert.Multiple(() =>
                    {
                        Assert.That(m.Image.FileGUID, Is.EqualTo(fileGuid));
                        Assert.That(m.Text, Is.EqualTo(BANNER_TEXT));
                    });
                });
        }


        [Test]
        public void Index_RetrieveNotExistingFile_ReturnsModelWithEmptyImage()
        {
            propertiesRetriever
                .Retrieve()
                .Returns(new BannerWidgetProperties { Image = null, Text = BANNER_TEXT });

            Assert.Multiple(() =>
            {
                controller.WithCallTo(c => c.Index())
                    .ShouldRenderPartialView(PARTIAL_VIEW_NAME)
                    .WithModel<BannerWidgetViewModel>(m => {
                        Assert.Multiple(() =>
                        {
                            Assert.That(m.Image, Is.Null);
                            Assert.That(m.Text, Is.EqualTo(BANNER_TEXT));
                        });
                    });
            });
        }
    }
}
