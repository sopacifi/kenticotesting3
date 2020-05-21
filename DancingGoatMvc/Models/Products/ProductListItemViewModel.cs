using System;

using CMS.Ecommerce;

namespace DancingGoat.Models.Products
{
    public class ProductListItemViewModel
    {
        public ProductCatalogPrices PriceDetail { get; }


        public string Name { get; }


        public string ImagePath { get; }


        public string PublicStatusName { get; }


        public bool Available { get; }


        public Guid ProductPageGUID { get; }


        public string ProductPageAlias { get; }


        public ProductListItemViewModel(SKUTreeNode productPage, ProductCatalogPrices priceDetail, string publicStatusName)
        {
            // Set page information
            Name = productPage.DocumentName;
            ProductPageGUID = productPage.NodeGUID;
            ProductPageAlias = productPage.NodeAlias;

            // Set SKU information
            ImagePath = productPage.SKU.SKUImagePath;
            Available = !productPage.SKU.SKUSellOnlyAvailable || productPage.SKU.SKUAvailableItems > 0;
            PublicStatusName = publicStatusName;

            // Set additional info
            PriceDetail = priceDetail;
        }
    }
}