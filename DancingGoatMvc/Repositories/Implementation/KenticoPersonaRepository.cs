using System.Collections.Generic;

using CMS.Personas;

using DancingGoat.Infrastructure;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Provides operations for personas.
    /// </summary>
    public class KenticoPersonaRepository : IPersonaRepository
    {
        /// <summary>
        /// Returns an enumerable collection of all personas.
        /// </summary>
        [CacheDependency("personas.persona|all")]
        public IEnumerable<PersonaInfo> GetAll()
        {
            return PersonaInfoProvider.GetPersonas();
        }
    }
}