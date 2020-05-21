using CMS.DocumentEngine.Types.DancingGoatMvc;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a landing page.
    /// </summary>
    public interface ILandingPageRepository : IRepository
    {
        /// <summary>
        /// Returns an object representing the landing page.
        /// <param name="pageAlias">Landing page node alias.</param>
        /// </summary>
        LandingPage GetLandingPage(string pageAlias);
    }
}
