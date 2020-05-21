using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatMvc;

namespace DancingGoat.Models.Widgets
{
    /// <summary>
    /// View model for Cafe card widget.
    /// </summary>
    public class CafeCardViewModel
    {
        /// <summary>
        /// Cafe name.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Cafe background image.
        /// </summary>
        public DocumentAttachment Photo { get; set; }


        /// <summary>
        /// Gets ViewModel for <paramref name="cafe"/>.
        /// </summary>
        /// <param name="cafe">Cafe.</param>
        /// <returns>Hydrated view model.</returns>
        public static CafeCardViewModel GetViewModel(Cafe cafe)
        {
            return cafe == null
                ? new CafeCardViewModel()
                : new CafeCardViewModel
                {
                    Name = cafe.Fields.Name,
                    Photo = cafe.Fields.Photo
                };
        }
    }
}