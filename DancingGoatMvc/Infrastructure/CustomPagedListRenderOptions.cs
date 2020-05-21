using System.Globalization;

using CMS.Helpers;

using PagedList.Mvc;

namespace DancingGoat.Infrastructure
{
    /// <summary>
    /// Customized render option definitions for X.PagedList.
    /// </summary>
    /// <remarks>
    /// For more customization options see https://github.com/kpi-ua/X.PagedList/blob/master/src/X.PagedList.Mvc/PagedListRenderOptions.cs.
    /// </remarks>
    public class CustomPagedListRenderOptions : PagedListRenderOptions
    {
        /// <summary>
        /// Creates a new instance of <see cref="CustomPagedListRenderOptions"/> class.
        /// </summary>
        public CustomPagedListRenderOptions()
        {
            LinkToPreviousPageFormat = ResHelper.GetString("General.Previous").ToLower(CultureInfo.InvariantCulture);
            LinkToNextPageFormat = ResHelper.GetString("General.Next").ToLower(CultureInfo.InvariantCulture);
        }
    }
}