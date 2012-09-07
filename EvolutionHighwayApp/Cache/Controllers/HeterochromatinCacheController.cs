using System.Collections.Generic;
using EvolutionHighwayApp.Dtos;

namespace EvolutionHighwayApp.Cache.Controllers
{
    public class HeterochromatinCacheController
    {
        private readonly IsolatedStorageCacheController<List<HeterochromatinRegionDto>> _heterochromatinRegionCache;
 
        public HeterochromatinCacheController()
        {
            _heterochromatinRegionCache = new IsolatedStorageCacheController<List<HeterochromatinRegionDto>>("HeterochromatinRegionCache");
        }

        public void Store(string refGenName, string refChrName, List<HeterochromatinRegionDto> heterochromatinRegions)
        {
            var cacheFileName = string.Format("{0}##{1}.xml", refGenName, refChrName);
            _heterochromatinRegionCache.Store(cacheFileName, heterochromatinRegions);
        }

        public List<HeterochromatinRegionDto> Retrieve(string refGenName, string refChrName)
        {
            var cacheFileName = string.Format("{0}##{1}.xml", refGenName, refChrName);
            return _heterochromatinRegionCache.Retrieve(cacheFileName);
        }

        public void Clear()
        {
            _heterochromatinRegionCache.Clear();
        }
    }
}
