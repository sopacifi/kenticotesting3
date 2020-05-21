using System.Web;
using System.Web.Mvc;

using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Search;

namespace DancingGoat.Models.Search
{
    public class SearchResultProductItemModel : SearchResultPageItemModel
    {
        public string Description { get; set; }


        public string ShortDescription { get; set; }


        public ProductCatalogPrices PriceDetail { get; set; }


        public SearchResultProductItemModel(SearchResultItem resultItem, SKUTreeNode skuTreeNode, ProductCatalogPrices priceDetail)
            : base(resultItem, skuTreeNode)
        {
            Description = skuTreeNode.DocumentSKUDescription;
            ShortDescription = HTMLHelper.StripTags(skuTreeNode.DocumentSKUShortDescription, false);
            PriceDetail = priceDetail;

            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            Url = urlHelper.Action("Detail", "Product",
                new {guid = skuTreeNode.NodeGUID, productAlias = skuTreeNode.NodeAlias});
        }
    }
}