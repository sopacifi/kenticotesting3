using System.Web;
using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Search;

namespace DancingGoat.Models.Search
{
    public class SearchResultCafeItemModel : SearchResultPageItemModel
    {
        public SearchResultCafeItemModel(SearchResultItem resultItem, Cafe cafe)
            :base(resultItem, cafe)
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            Url = urlHelper.Action("Index", "Cafes");
        }
    }
}