using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using DancingGoat.Controllers.PageTemplates;
using DancingGoat.Models.PageTemplates;

using Kentico.PageBuilder.Web.Mvc.PageTemplates;

[assembly: RegisterPageTemplate("DancingGoat.Article", typeof(ArticlePageTemplateController), "{$dancinggoatmvc.pagetemplate.article.name$}", Description = "{$dancinggoatmvc.pagetemplate.article.description$}", IconClass = "icon-l-text")]

namespace DancingGoat.Controllers.PageTemplates
{
    public class ArticlePageTemplateController : PageTemplateController
    {
        public ActionResult Index()
        {
            var article = GetPage<Article>();
            if (article == null)
            {
                return HttpNotFound();
            }

            return View("PageTemplates/_Article", ArticleViewModel.GetViewModel(article));
        }
    }
}