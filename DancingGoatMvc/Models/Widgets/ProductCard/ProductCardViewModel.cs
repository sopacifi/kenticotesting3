using CMS.Ecommerce;

namespace DancingGoat.Models.Widgets
{
    /// <summary>
    /// View model for Product card widget.
    /// </summary>
    public class ProductCardViewModel
    {
        /// <summary>
        /// Card heading.
        /// </summary>
        public string Heading { get; set; } = "Please select a product.";


        /// <summary>
        /// Card background image.
        /// </summary>
        public string Image { get; set; }


        /// <summary>
        /// Card text.
        /// </summary>
        public string Text { get; set; }


        /// <summary>
        /// Gets ViewModel for <paramref name="product"/>.
        /// </summary>
        /// <param name="product">Product.</param>
        /// <returns>Hydrated ViewModel.</returns>
        public static ProductCardViewModel GetViewModel(SKUTreeNode product)
        {
            return product == null
                ? new ProductCardViewModel()
                : new ProductCardViewModel
                {
                    Heading = product.DocumentSKUName,
                    Image = product.SKU.SKUImagePath,
                    Text = product.DocumentSKUShortDescription
                };
        }
    }
}