using System.Web;
using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Search;


namespace DancingGoat.Models.Search
{
    public class SearchResultAboutUsItemModel : SearchResultPageItemModel
    {
        public SearchResultAboutUsItemModel(SearchResultItem resultItem, AboutUs aboutUs) :base(resultItem, aboutUs)
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            Url = urlHelper.Action("Index", "About");
        }
    }
}