using System.Web;
using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Search;

namespace DancingGoat.Models.Search
{
    public class SearchResultAboutUsSectionItemModel : SearchResultPageItemModel
    {
        public SearchResultAboutUsSectionItemModel(SearchResultItem resultItem, AboutUsSection aboutUsSection)
            :base(resultItem, aboutUsSection)
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            Url = urlHelper.Action("Index", "About");
        }
    }
}