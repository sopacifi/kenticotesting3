using System.Collections.Generic;
using System.Linq;

using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Tests;

using Kentico.PageBuilder.Web.Mvc;

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
    [TestFixture]
    public class ArticlesWidgetControllerTests : UnitTests
    {
        private const string PARTIAL_VIEW_NAME = "Widgets/_ArticlesWidget";
        private const int CURRENT_ARTICLES_COUNT = 2;
        private const int REQUIRED_ARTICLES_COUNT = 5;

        private readonly IComponentPropertiesRetriever<ArticlesWidgetProperties> propertiesRetriever = Substitute.For<IComponentPropertiesRetriever<ArticlesWidgetProperties>>();
        private readonly IArticleRepository articleRepository = Substitute.For<IArticleRepository>();
        private readonly IOutputCacheDependencies outputCacheDependencies = Substitute.For<IOutputCacheDependencies>();


        [Test]
        public void Index_ReturnsCorrectModel()
        {
            Fake().DocumentType<Article>(Article.CLASS_NAME);
            var articlesList = new List<Article>
                {
                    new Article
                    {
                        ArticleTitle = "Title 1"
                    },
                    new Article
                    {
                        ArticleTitle = "Title 2"
                    }
                };

            propertiesRetriever.Retrieve().Returns(new ArticlesWidgetProperties { Count = REQUIRED_ARTICLES_COUNT });
            articleRepository.GetArticles(REQUIRED_ARTICLES_COUNT).Returns(articlesList);

            var controller = new ArticlesWidgetController(articleRepository, outputCacheDependencies, propertiesRetriever, Substitute.For<ICurrentPageRetriever>());
            controller.ControllerContext = ControllerContextMock.GetControllerContext(controller);

            controller.WithCallTo(c => c.Index())
                .ShouldRenderPartialView(PARTIAL_VIEW_NAME)
                .WithModel<ArticlesWidgetViewModel>(m => m.Articles.Count() == CURRENT_ARTICLES_COUNT && m.Count == REQUIRED_ARTICLES_COUNT);
        }
    }
}
