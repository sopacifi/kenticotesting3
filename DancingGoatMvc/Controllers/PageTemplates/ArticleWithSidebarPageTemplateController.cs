using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using DancingGoat.Controllers.PageTemplates;
using DancingGoat.Models.PageTemplates;

using Kentico.PageBuilder.Web.Mvc.PageTemplates;

[assembly: RegisterPageTemplate("DancingGoat.ArticleWithSidebar", typeof(ArticleWithSidebarPageTemplateController), "{$dancinggoatmvc.pagetemplate.articlewithsidebar.name$}", Description = "{$dancinggoatmvc.pagetemplate.articlewithsidebar.description$}", IconClass = "icon-l-text-col")]

namespace DancingGoat.Controllers.PageTemplates
{
    public class ArticleWithSidebarPageTemplateController : PageTemplateController<ArticleWithSideBarProperties>
    {
        public ActionResult Index()
        {
            var article = GetPage<Article>();
            if (article == null)
            {
                return HttpNotFound();
            }

            ArticleWithSideBarProperties templateProperties = GetProperties();

            return View("PageTemplates/_ArticleWithSidebar", ArticleWithSideBarViewModel.GetViewModel(article, templateProperties));
        }
    }
}