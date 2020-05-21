using System.Collections.Generic;
using System.Linq;
using CMS.Ecommerce;


namespace DancingGoat.Models.Products
{
    public class ProductOptionCategoryViewModel
    {
        private readonly OptionCategoryInfo Category;


        public string Name => string.IsNullOrEmpty(Category.CategoryLiveSiteDisplayName) ?
            Category.CategoryDisplayName :
            Category.CategoryLiveSiteDisplayName;


        public OptionCategorySelectionTypeEnum SelectionType => Category.CategorySelectionType;


        public IEnumerable<SKUInfo> Options { get; }


        public int SelectedOptionId { get; set; }


        public ProductOptionCategoryViewModel(int skuID, int selectedOptionID, OptionCategoryInfo category)
        {
            SelectedOptionId = selectedOptionID;
            Category = category;
            Options = GetOptions(skuID, category.CategoryID);
        }


        private IEnumerable<SKUInfo> GetOptions(int skuID, int categoryID)
        {
            // Get all variant's options
            var variantOptionIDs = VariantOptionInfoProvider.GetVariantOptions()
                                                            .WhereIn("VariantSKUID", VariantHelper.GetVariants(skuID).Column("SKUID"))
                                                            .Column("OptionSKUID");

            var variantOptionsList = SKUInfoProvider.GetSKUs()
                                                    .WhereIn("SKUID", variantOptionIDs)
                                                    .OrderBy("SKUOrder")
                                                    .ToList();

            // Create option categories with selectable variant options
            return variantOptionsList.Where(o => o.SKUOptionCategoryID == categoryID);
        }
    }
}