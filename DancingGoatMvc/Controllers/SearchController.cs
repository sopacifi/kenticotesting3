using System;
using System.Linq;
using System.Web.Mvc;

using CMS.Membership;
using CMS.Search;
using CMS.WebAnalytics;

using DancingGoat.Infrastructure;
using DancingGoat.Models.Search;

using PagedList;

namespace DancingGoat.Controllers
{
    public class SearchController : Controller
    {
        private const string INDEX_NAME = "DancingGoatMvc.Index";
        private const int PAGE_SIZE = 5;
        private const int DEFAULT_PAGE_NUMBER = 1;

        private readonly TypedSearchItemViewModelFactory mSearchItemViewModelFactory;
        private readonly IPagesActivityLogger mPagesActivityLogger;


        public SearchController(TypedSearchItemViewModelFactory searchItemViewModelFactory, IPagesActivityLogger pagesActivityLogger)
        {
            mSearchItemViewModelFactory = searchItemViewModelFactory;
            mPagesActivityLogger = pagesActivityLogger;
        }


        // GET: Search
        [ValidateInput(false)]
        public ActionResult Index(string searchText, int page = DEFAULT_PAGE_NUMBER)
        {
            if (String.IsNullOrWhiteSpace(searchText))
            {
                var empty = new SearchResultsModel
                {
                    Items = new StaticPagedList<SearchResultItemModel>(Enumerable.Empty<SearchResultItemModel>(), page, PAGE_SIZE, 0)
                };
                return View(empty);
            }

            // Validate page number (starting from 1)
            page = Math.Max(page, DEFAULT_PAGE_NUMBER);

            var searchParameters = SearchParameters.PrepareForPages(searchText, new[] { INDEX_NAME }, page, PAGE_SIZE, MembershipContext.AuthenticatedUser);
            var searchResults = SearchHelper.Search(searchParameters);

            mPagesActivityLogger.LogInternalSearch(searchText);

            var searchResultItemModels = searchResults.Items
                .Select(searchResultItem => mSearchItemViewModelFactory.GetTypedSearchResultItemModel(searchResultItem));

            var model = new SearchResultsModel
            {
                Items = new StaticPagedList<SearchResultItemModel>(searchResultItemModels, page, PAGE_SIZE, searchResults.TotalNumberOfResults),
                Query = searchText
            };

            return View(model);
        }
    }
}