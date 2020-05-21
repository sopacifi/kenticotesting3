using System;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.SiteProvider;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Encapsulates access to landing page.
    /// </summary>
    public class KenticoLandingPageRepository : ILandingPageRepository
    {
        private readonly string mCultureName;
        private readonly bool mLatestVersionEnabled;


        /// <summary>
        /// Creates a new instance of the <see cref="KenticoLandingPageRepository"/> class.
        /// </summary>
        /// <param name="cultureName">The name of a culture.</param>
        /// <param name="latestVersionEnabled">Indicates whether the repository will provide the most recent version of pages.</param>
        public KenticoLandingPageRepository(string cultureName, bool latestVersionEnabled)
        {
            mCultureName = cultureName;
            mLatestVersionEnabled = latestVersionEnabled;
        }


        /// <summary>
        /// Returns an object representing the landing page.
        /// </summary>
        /// <param name="pageAlias">Landing page node alias.</param>
        public LandingPage GetLandingPage(string pageAlias)
        {
            return LandingPageProvider.GetLandingPages()
                               .LatestVersion(mLatestVersionEnabled)
                               .Published(!mLatestVersionEnabled)
                               .OnSite(SiteContext.CurrentSiteName)
                               .Culture(mCultureName)
                               .CombineWithDefaultCulture()
                               .Path("/Landing-pages", PathTypeEnum.Children)
                               .WhereEquals("NodeAlias", pageAlias)                               
                               .TopN(1);
        }
    }
}