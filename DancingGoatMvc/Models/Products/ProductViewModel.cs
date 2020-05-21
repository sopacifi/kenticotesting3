using System;
using System.Collections.Generic;

using CMS.Ecommerce;

namespace DancingGoat.Models.Products
{
    public class ProductViewModel
    {
        public ITypedProductViewModel TypedProduct { get; }


        public ProductCatalogPrices PriceDetail { get; }


        public IEnumerable<ProductOptionCategoryViewModel> ProductOptionCategories { get; }


        public int SelectedVariantID { get; set; }


        public bool IsInStock { get; }


        public string Name { get; }


        public string Description { get; }


        public string ShortDescription { get; }


        public int SKUID { get; }


        public string ImagePath { get; }


        public ProductViewModel(SKUTreeNode productPage, ProductCatalogPrices priceDetail,
            ITypedProductViewModel typedProductViewModel = null)
        {
            // Set page information
            Name = productPage.DocumentName;
            Description = productPage.DocumentSKUDescription;
            ShortDescription = productPage.DocumentSKUShortDescription;

            // Set SKU information
            var sku = productPage.SKU;
            SKUID = sku.SKUID;
            ImagePath = sku.SKUImagePath;
            IsInStock = sku.SKUTrackInventory == TrackInventoryTypeEnum.Disabled ||
                        sku.SKUAvailableItems > 0;

            // Set additional info
            TypedProduct = typedProductViewModel;
            PriceDetail = priceDetail;
        }


        public ProductViewModel(SKUTreeNode productPage, ProductCatalogPrices price,
            ITypedProductViewModel typedProductViewModel, ProductVariant defaultVariant,
            IEnumerable<ProductOptionCategoryViewModel> categories)
            : this(productPage, price, typedProductViewModel)
        {
            if (defaultVariant == null)
            {
                throw new ArgumentNullException(nameof(defaultVariant));
            }

            var variant = defaultVariant.Variant;

            IsInStock = ((variant.SKUTrackInventory == TrackInventoryTypeEnum.Disabled) || (variant.SKUAvailableItems > 0));
            SelectedVariantID = variant.SKUID;

            // Variant categories which will be rendered
            ProductOptionCategories = categories;
        }
    }
}