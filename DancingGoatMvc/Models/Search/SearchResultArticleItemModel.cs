using System.Web;
using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Search;

namespace DancingGoat.Models.Search
{
    public class SearchResultArticleItemModel : SearchResultPageItemModel
    {
        public SearchResultArticleItemModel(SearchResultItem resultItem, Article article)
            : base(resultItem, article)
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            Url = urlHelper.Action("Show", "Articles", new { guid = article.NodeGUID });
        }
    }
}