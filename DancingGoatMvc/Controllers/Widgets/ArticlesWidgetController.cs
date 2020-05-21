using System.Linq;
using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using DancingGoat.Controllers.Widgets;
using DancingGoat.Infrastructure;
using DancingGoat.Models.Articles;
using DancingGoat.Models.Widgets;
using DancingGoat.Repositories;

using Kentico.PageBuilder.Web.Mvc;

[assembly: RegisterWidget("DancingGoat.HomePage.ArticlesWidget", typeof(ArticlesWidgetController), "{$dancinggoatmvc.widget.articles.name$}", Description = "{$dancinggoatmvc.widget.articles.description$}", IconClass = "icon-l-list-article")]

namespace DancingGoat.Controllers.Widgets
{
    /// <summary>
    /// Controller for article widgets.
    /// </summary>
    public class ArticlesWidgetController : WidgetController<ArticlesWidgetProperties>
    {
        private readonly IArticleRepository mArticleRepository;
        private readonly IOutputCacheDependencies mOutputCacheDependencies;


        /// <summary>
        /// Creates an instance of <see cref="ArticlesWidgetController"/> class.
        /// </summary>
        /// <param name="repository">Repository for retrieving articles.</param>
        /// <param name="outputCacheDependencies">Output cache dependency handling.</param>
        public ArticlesWidgetController(IArticleRepository repository, IOutputCacheDependencies outputCacheDependencies)
        {
            mArticleRepository = repository;
            mOutputCacheDependencies = outputCacheDependencies;
        }


        /// <summary>
        /// Creates an instance of <see cref="ArticlesWidgetController"/> class.
        /// </summary>
        /// <param name="repository">Repository for retrieving articles.</param>
        /// <param name="outputCacheDependencies">Output cache dependency handling.</param>
        /// <param name="propertiesRetriever">Retriever for widget properties.</param>
        /// <param name="currentPageRetriever">Retriever for current page where is the widget used.</param>
        /// <remarks>Use this constructor for tests to handle dependencies.</remarks>
        public ArticlesWidgetController(IArticleRepository repository, IOutputCacheDependencies outputCacheDependencies,
                                        IComponentPropertiesRetriever<ArticlesWidgetProperties> propertiesRetriever,
                                        ICurrentPageRetriever currentPageRetriever) : base(propertiesRetriever, currentPageRetriever)
        {
            mArticleRepository = repository;
            mOutputCacheDependencies = outputCacheDependencies;
        }


        public ActionResult Index()
        {
            var properties = GetProperties();
            var widgetModels = mArticleRepository.GetArticles(properties.Count)
                                                 .Select(ArticleViewModel.GetViewModel);

            mOutputCacheDependencies.AddDependencyOnPages<Article>();

            return PartialView("Widgets/_ArticlesWidget", new ArticlesWidgetViewModel { Articles = widgetModels, Count = properties.Count });
        }
    }
}