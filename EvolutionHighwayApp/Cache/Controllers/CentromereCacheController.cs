using System.Collections.Generic;
using EvolutionHighwayApp.Dtos;

namespace EvolutionHighwayApp.Cache.Controllers
{
    public class CentromereCacheController
    {
        private readonly IsolatedStorageCacheController<List<CentromereRegionDto>> _centromereRegionCache;
 
        public CentromereCacheController()
        {
            _centromereRegionCache = new IsolatedStorageCacheController<List<CentromereRegionDto>>("CentromereRegionCache");
        }

        public void Store(string refGenName, string refChrName, List<CentromereRegionDto> centromereRegions)
        {
            var cacheFileName = string.Format("{0}##{1}.xml", refGenName, refChrName);
            _centromereRegionCache.Store(cacheFileName, centromereRegions);
        }

        public List<CentromereRegionDto> Retrieve(string refGenName, string refChrName)
        {
            var cacheFileName = string.Format("{0}##{1}.xml", refGenName, refChrName);
            return _centromereRegionCache.Retrieve(cacheFileName);
        }

        public void Clear()
        {
            _centromereRegionCache.Clear();
        }
    }
}
