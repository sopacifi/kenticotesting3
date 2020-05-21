using System.Collections.Generic;

using CMS.DocumentEngine.Types.DancingGoatMvc;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of home page sections.
    /// </summary>
    public interface IHomeRepository : IRepository
    {
        /// <summary>
        /// Returns an object representing the home page.
        /// </summary>
        Home GetHomePage();

        /// <summary>
        /// Returns an enumerable collection of home page sections ordered by a position in the content tree.
        /// </summary>
        IEnumerable<HomeSection> GetHomeSections();
    }
}