using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using EvolutionHighwayApp.Dtos;
using EvolutionHighwayApp.Infrastructure.WebServices;

namespace EvolutionHighwayApp.ServiceLayer
{
    [ServiceContract]
    // ReSharper disable InconsistentNaming
    public interface IEHDataService
    // ReSharper restore InconsistentNaming
    {
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/genomes")]
        [OperationContract(Name = "ListRefGenomes", AsyncPattern = true)]
        IAsyncResult BeginListRefGenomes(AsyncCallback callback, object state = null);
        IEnumerable<GenomeDto> EndListRefGenomes(IAsyncResult asyncResult);

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/genomes/{genome}/chromosomes")]
        [OperationContract(Name = "ListRefChromosomes", AsyncPattern = true)]
        IAsyncResult BeginListRefChromosomes(string genome, AsyncCallback callback, object state = null);
        IEnumerable<ChromosomeDto> EndListRefChromosomes(IAsyncResult asyncResult);

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/genomes/{genome}/chromosomes/{chromosome}/species")]
        [OperationContract(Name = "ListCompGenomes", AsyncPattern = true)]
        IAsyncResult BeginListCompGenomes(string genome, string chromosome, AsyncCallback callback, object state = null);
        IEnumerable<CompGenomeDto> EndListCompGenomes(IAsyncResult asyncResult);

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/genomes/{genome}/chromosomes/{chromosome}/centromere")]
        [OperationContract(Name = "ListCentromereRegions", AsyncPattern = true)]
        IAsyncResult BeginListCentromereRegions(string genome, string chromosome, AsyncCallback callback, object state = null);
        IEnumerable<CentromereRegionDto> EndListCentromereRegions(IAsyncResult asyncResult);

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/genomes/{genome}/chromosomes/{chromosome}/heterochromatin")]
        [OperationContract(Name = "ListHeterochromatinRegions", AsyncPattern = true)]
        IAsyncResult BeginListHeterochromatinRegions(string genome, string chromosome, AsyncCallback callback, object state = null);
        IEnumerable<HeterochromatinRegionDto> EndListHeterochromatinRegions(IAsyncResult asyncResult);

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/genomes/{genome}/chromosomes/{chromosome}/species/{species}/synblocks")]
        [OperationContract(Name = "ListSyntenyRegions", AsyncPattern = true)]
        IAsyncResult BeginListSyntenyRegions(string genome, string chromosome, string species, AsyncCallback callback, object state = null);
        IEnumerable<SyntenyRegionDto> EndListSyntenyRegions(IAsyncResult asyncResult);

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/species/{species}/chromosomesLength")]
        [OperationContract(Name = "GetCompChrLengths", AsyncPattern = true)]
        IAsyncResult BeginGetCompChrLengths(string species, AsyncCallback callback, object state = null);
        IEnumerable<CompChrLengthDto> EndGetCompChrLengths(IAsyncResult asyncResult);

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/features/density/{featureName}/{refGen}/{refChr}")]
        [OperationContract(Name = "GetFeatureData", AsyncPattern = true)]
        IAsyncResult BeginGetFeatureData(string featureName, string refGen, string refChr, AsyncCallback callback, object state = null);
        IEnumerable<FeatureDensityDto> EndGetFeatureData(IAsyncResult asyncResult);

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/features/density")]
        [OperationContract(Name = "ListDensityFeatures", AsyncPattern = true)]
        IAsyncResult BeginListDensityFeatures(AsyncCallback callback, object state = null);
        IEnumerable<FeatureDto> EndListDensityFeatures(IAsyncResult asyncResult);
    }

    // ReSharper disable InconsistentNaming
    public class EHDataService
    // ReSharper restore InconsistentNaming
    {
        public static IEHDataService CreateServiceProxy(EndpointAddress address)
        {
            var binding = new CustomBinding(
                new WebMessageEncodingBindingElement(),
                new HttpTransportBindingElement { ManualAddressing = true, MaxReceivedMessageSize = 1024000 }
            );

            var factory = new ChannelFactory<IEHDataService>(binding, address);
            factory.Endpoint.Behaviors.Add(new WebHttpBehaviorWithJson());

            var serviceProxy = factory.CreateChannel();
            ((IClientChannel)serviceProxy).Closed += delegate { factory.Close(); };

            return serviceProxy;
        }
    }
}
