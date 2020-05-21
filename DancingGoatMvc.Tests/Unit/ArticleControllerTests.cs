using System;
using System.Net;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Tests;

using DancingGoat.Controllers;
using DancingGoat.Infrastructure;
using DancingGoat.Repositories;

using Kentico.PageBuilder.Web.Mvc.PageTemplates;

using NSubstitute;
using NUnit.Framework;
using Tests.DocumentEngine;
using TestStack.FluentMVCTesting;

namespace DancingGoat.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class ArticlesControllerTests : UnitTests
    {
        private ArticlesController mController;
        private Article mArticle;
        private IOutputCacheDependencies mDependencies;
        private const string ARTICLE_TITLE = "Article1";
        private const int DOCUMENT_ID = 1;

        private readonly Guid nodeGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");


        [SetUp]
        public void SetUp()
        {
            Fake().DocumentType<Article>(Article.CLASS_NAME);
            mArticle = TreeNode.New<Article>()
                                .With(a => a.Fields.Title = ARTICLE_TITLE)
                                .With(a => a.SetValue("DocumentID", DOCUMENT_ID));
            mDependencies = Substitute.For<IOutputCacheDependencies>();
            
            var repository = Substitute.For<IArticleRepository>();
            repository.GetArticle(nodeGuid).Returns(mArticle);
            
            mController = new ArticlesController(repository, mDependencies);
        }


        [Test]
        public void Index_RendersDefaultView()
        {
            mController.WithCallTo(c => c.Index())
                .ShouldRenderDefaultView();
        }


        [Test]
        public void Show_WithExistingArticle_RendersDefaultViewWithCorrectModel()
        {
            mController.WithCallTo(c => c.Show(nodeGuid, null))
                .ValidateActionReturnType<TemplateResult>();
        }


        [Test]
        public void Show_WithoutExistingArticle_ReturnsHttpNotFoundStatusCode()
        {
            mController.WithCallTo(c => c.Show(Guid.Parse("00000000-0000-0000-0000-000000000002"), null))
                .ShouldGiveHttpStatus(HttpStatusCode.NotFound);
        }
    }
}