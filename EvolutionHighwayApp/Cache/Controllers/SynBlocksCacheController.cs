using System.Collections.Generic;
using EvolutionHighwayApp.Dtos;

namespace EvolutionHighwayApp.Cache.Controllers
{
    public class SynBlocksCacheController
    {
        private readonly IsolatedStorageCacheController<List<SyntenyRegionDto>> _synBlocksCache;
 
        public SynBlocksCacheController()
        {
            _synBlocksCache = new IsolatedStorageCacheController<List<SyntenyRegionDto>>("SynBlocksCache");
        }

        public void Store(string refGenName, string refChrName, string compGenName, List<SyntenyRegionDto> syntenyRegions)
        {
            var cacheFileName = string.Format("{0}##{1}##{2}.xml", refGenName, refChrName, compGenName);
            _synBlocksCache.Store(cacheFileName, syntenyRegions);
        }

        public List<SyntenyRegionDto> Retrieve(string refGenName, string refChrName, string compGenName)
        {
            var cacheFileName = string.Format("{0}##{1}##{2}.xml", refGenName, refChrName, compGenName);
            return _synBlocksCache.Retrieve(cacheFileName);
        }

        public void Clear()
        {
            _synBlocksCache.Clear();
        }
    }
}
