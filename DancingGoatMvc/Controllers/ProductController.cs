using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using CMS.Ecommerce;
using CMS.Helpers;

using DancingGoat.Infrastructure;
using DancingGoat.Models.Products;
using DancingGoat.Repositories;
using DancingGoat.Services;

namespace DancingGoat.Controllers
{
    public class ProductController : Controller
    {
        private readonly ICalculationService mCalculationService;
        private readonly IProductRepository mProductRepository;
        private readonly IVariantRepository mVariantRepository;
        private readonly TypedProductViewModelFactory mTypedProductViewModelFactory;
        private readonly IShoppingService mShoppingService;


        public ProductController(ICalculationService calculationService, IProductRepository productRepository,
            IVariantRepository variantRepository, TypedProductViewModelFactory typedProductViewModelFactory, IShoppingService shoppingService)
        {
            mCalculationService = calculationService;
            mProductRepository = productRepository;
            mVariantRepository = variantRepository;
            mTypedProductViewModelFactory = typedProductViewModelFactory;
            mShoppingService = shoppingService;
        }


        public ActionResult Detail(Guid guid, string productAlias)
        {
            var product = mProductRepository.GetProduct(guid);
            var sku = product?.SKU;

            // If a product is not found or not allowed for sale, redirect to 404
            if ((product == null) || (sku == null) || !sku.SKUEnabled)
            {
                return HttpNotFound();
            }

            // Redirect if the page alias does not match
            if (!string.IsNullOrEmpty(productAlias) &&
                !product.NodeAlias.Equals(productAlias, StringComparison.InvariantCultureIgnoreCase))
            {
                return RedirectToActionPermanent("Detail", new { guid = product.NodeGUID, productAlias = product.NodeAlias });
            }

            var viewModel = PrepareProductDetailViewModel(product);

            return View(viewModel);
        }


        [HttpPost]
        public JsonResult Variant(List<int> options, int parentProductID)
        {
            var variant = mVariantRepository.GetByProductIdAndOptions(parentProductID, options);

            if (variant == null)
            {
                return Json(new
                {
                    stockMessage = ResHelper.GetString("DancingGoatMvc.Product.NotAvailable"),
                    totalPrice ="-"
                });
            }

            var cart = mShoppingService.GetCurrentShoppingCart();
            var variantPrice = mCalculationService.CalculatePrice(variant.Variant);
            var isInStock = ((variant.Variant.SKUTrackInventory == TrackInventoryTypeEnum.Disabled) || (variant.Variant.SKUAvailableItems > 0));

            return GetVariantResponse(variantPrice,
                                      isInStock,
                                      isInStock ? "DancingGoatMvc.Product.InStock" : "DancingGoatMvc.Product.OutOfStock",
                                      variant.Variant.SKUID, cart.Currency);

        }


        private JsonResult GetVariantResponse(ProductCatalogPrices priceDetail, bool inStock, string stockMessageResourceString, int variantSKUID, CurrencyInfo currency)
        {
            string priceSavings = string.Empty;

            var discount = priceDetail.StandardPrice - priceDetail.Price;
            var beforeDiscount = priceDetail.Price + discount;

            if (discount > 0)
            {
                var discountPercentage = Math.Round(discount * 100 / beforeDiscount);
                var formattedDiscount = String.Format(currency.CurrencyFormatString, discount);
                priceSavings = $"{formattedDiscount} ({discountPercentage}%)";
            }

            var response = new
            {
                totalPrice = String.Format(currency.CurrencyFormatString, priceDetail.Price),
                beforeDiscount = discount > 0 ? String.Format(currency.CurrencyFormatString, beforeDiscount) : string.Empty,
                savings = priceSavings,
                stockMessage = ResHelper.GetString(stockMessageResourceString),
                inStock,
                variantSKUID
            };

            return Json(response);
        }


        private ProductViewModel PrepareProductDetailViewModel(SKUTreeNode product)
        {
            var sku = product.SKU;

            var cheapestVariant = GetCheapestVariant(product);
            var variantCategories = PrepareProductOptionCategoryViewModels(sku, cheapestVariant);

            // Calculate the price of the product or selected variant
            var cheapestProduct = cheapestVariant != null ? cheapestVariant.Variant : sku;
            var price = mCalculationService.CalculatePrice(cheapestProduct);

            // Create a strongly typed view model with product type specific information
            var typedProduct = mTypedProductViewModelFactory.GetViewModel(product);

            var viewModel = (cheapestVariant != null)
                ? new ProductViewModel(product, price, typedProduct, cheapestVariant, variantCategories)
                : new ProductViewModel(product, price, typedProduct);
            return viewModel;
        }


        private IEnumerable<ProductOptionCategoryViewModel> PrepareProductOptionCategoryViewModels(SKUInfo sku, ProductVariant cheapestVariant)
        {
            var categories = mVariantRepository.GetVariantOptionCategories(sku.SKUID);

            // Set the selected options in the variant categories which represents the cheapest variant
            var variantCategories =
                cheapestVariant?.ProductAttributes.Select(
                    option =>
                        new ProductOptionCategoryViewModel(sku.SKUID, option.SKUID,
                            categories.FirstOrDefault(c => c.CategoryID == option.SKUOptionCategoryID)));

            return variantCategories;
        }


        private ProductVariant GetCheapestVariant(SKUTreeNode product)
        {
            var variants = mVariantRepository.GetByProductId(product.NodeSKUID).OrderBy(v => v.Variant.SKUPrice).ToList();
            var cheapestVariant = variants.FirstOrDefault();

            return cheapestVariant;
        }
    }
}