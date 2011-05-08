using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using EvolutionHighwayModel;

namespace EvolutionHighwayApp.Infrastructure.WebServices
{
    [ServiceContract]
    public interface IEHDataService
    {
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/genomes")]
        [OperationContract(Name = "ListGenomes", AsyncPattern = true)]
        IAsyncResult BeginListGenomes(AsyncCallback callback, object state = null);
        List<Genome> EndListGenomes(IAsyncResult asyncResult);

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/genomes/{genome}/chromosomes")]
        [OperationContract(Name = "ListChromosomesForGenome", AsyncPattern = true)]
        IAsyncResult BeginListChromosomes(string genome, AsyncCallback callback, object state = null);
        List<Chromosome> EndListChromosomes(IAsyncResult asyncResult);

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/genomes/{genome}/chromosomes/{chromosome}/species")]
        [OperationContract(Name = "ListSpeciesForChromosome", AsyncPattern = true)]
        IAsyncResult BeginListSpecies(string genome, string chromosome, AsyncCallback callback, object state = null);
        List<ComparativeSpecies> EndListSpecies(IAsyncResult asyncResult);

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/genomes/{genome}/chromosomes/{chromosome}/centromere")]
        [OperationContract(Name = "ListCentromereForChromosome", AsyncPattern = true)]
        IAsyncResult BeginListCentromere(string genome, string chromosome, AsyncCallback callback, object state = null);
        List<CentromereRegion> EndListCentromere(IAsyncResult asyncResult);

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/genomes/{genome}/chromosomes/{chromosome}/heterochromatin")]
        [OperationContract(Name = "ListHeterochromatinForChromosome", AsyncPattern = true)]
        IAsyncResult BeginListHeterochromatin(string genome, string chromosome, AsyncCallback callback, object state = null);
        List<HeterochromatinRegion> EndListHeterochromatin(IAsyncResult asyncResult);

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/genomes/{genome}/chromosomes/{chromosome}/species/{species}/synblocks")]
        [OperationContract(Name = "ListSynBlocksForSpecies", AsyncPattern = true)]
        IAsyncResult BeginListSynblocks(string genome, string chromosome, string species, AsyncCallback callback, object state = null);
        List<AncestorRegion> EndListSynblocks(IAsyncResult asyncResult);

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/species/{species}/chromosomesLength")]
        [OperationContract(Name = "GetSpeciesChromosomesLength", AsyncPattern = true)]
        IAsyncResult BeginGetSpeciesChromosomesLength(string species, AsyncCallback callback, object state = null);
        List<SpeciesChromosomeLengths> EndGetSpeciesChromosomesLength(IAsyncResult asyncResult);
    }

    public class EHDataService
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
