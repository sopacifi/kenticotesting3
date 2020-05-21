using System;
using System.Linq;
using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using DancingGoat.Infrastructure;
using DancingGoat.Models.Articles;
using DancingGoat.Repositories;

using Kentico.PageBuilder.Web.Mvc.PageTemplates;

namespace DancingGoat.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly IArticleRepository mArticleRepository;
        private readonly IOutputCacheDependencies mOutputCacheDependencies;


        public ArticlesController(IArticleRepository repository, IOutputCacheDependencies outputCacheDependencies)
        {
            mArticleRepository = repository;
            mOutputCacheDependencies = outputCacheDependencies;
        }


        // GET: Articles
        [OutputCache(CacheProfile = "Default")]
        public ActionResult Index()
        {
            var articles = mArticleRepository.GetArticles();
            mOutputCacheDependencies.AddDependencyOnPages<Article>();

            return View(articles.Select(ArticleViewModel.GetViewModel));
        }


        // GET: Articles/Show/{guid}
        [OutputCache(CacheProfile = "Default", VaryByParam = "guid")]
        public ActionResult Show(Guid guid, string pageAlias)
        {
            var article = mArticleRepository.GetArticle(guid);

            if (article == null)
            {
                return HttpNotFound();
            }

            // Redirect if page alias does not match
            if (!string.IsNullOrEmpty(pageAlias) && !article.NodeAlias.Equals(pageAlias, StringComparison.InvariantCultureIgnoreCase))
            {
                return RedirectToActionPermanent("Show", new { guid = article.NodeGUID, pageAlias = article.NodeAlias });
            }

            mOutputCacheDependencies.AddDependencyOnPages<Article>();

            return new TemplateResult(article.DocumentID);
        }
    }
}