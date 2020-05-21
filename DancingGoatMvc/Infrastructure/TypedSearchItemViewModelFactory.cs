using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Ecommerce;
using CMS.Search;

using DancingGoat.Models.Search;
using DancingGoat.Services;

namespace DancingGoat.Infrastructure
{
    /// <summary>
    /// Provides methods for conversion from <see cref="SearchResultItem"/> to particular <see cref="SearchResultItemModel"/>.
    /// </summary>
    public class TypedSearchItemViewModelFactory
    {
        private readonly ICalculationService mCalculationService;


        /// <summary>
        /// Creates a new instance of <see cref="TypedSearchItemViewModelFactory"/> class.
        /// </summary>
        /// <param name="calculationService">Service for price calculations.</param>
        public TypedSearchItemViewModelFactory(ICalculationService calculationService)
        {
            mCalculationService = calculationService;
        }


        /// <summary>
        /// Creates a search view model according to the runtime type of <paramref name="searchResultItem"/>.
        /// </summary>
        public SearchResultItemModel GetTypedSearchResultItemModel(SearchResultItem searchResultItem)
        {
            var data = (dynamic) searchResultItem.Data;
            return GetViewModelForSearchItem(searchResultItem, data);
        }


        private SearchResultItemModel GetViewModelForSearchItem(SearchResultItem resultItem, TreeNode treeNode)
        {
            return new SearchResultPageItemModel(resultItem, treeNode);
        }


        private SearchResultItemModel GetViewModelForSearchItem(SearchResultItem resultItem, SKUTreeNode skuTreeNode)
        {
            var price = mCalculationService.CalculatePrice(skuTreeNode.SKU);
            return new SearchResultProductItemModel(resultItem, skuTreeNode, price);
        }


        private SearchResultItemModel GetViewModelForSearchItem(SearchResultItem resultItem, Article article)
        {
            return new SearchResultArticleItemModel(resultItem, article);
        }


        private SearchResultItemModel GetViewModelForSearchItem(SearchResultItem resultItem, AboutUs aboutUs)
        {
            return new SearchResultAboutUsItemModel(resultItem, aboutUs);
        }


        private SearchResultItemModel GetViewModelForSearchItem(SearchResultItem resultItem, AboutUsSection aboutUsSection)
        {
            return new SearchResultAboutUsSectionItemModel(resultItem, aboutUsSection);
        }


        private SearchResultItemModel GetViewModelForSearchItem(SearchResultItem resultItem, Cafe cafe)
        {
            return new SearchResultCafeItemModel(resultItem, cafe);
        }
    }
}