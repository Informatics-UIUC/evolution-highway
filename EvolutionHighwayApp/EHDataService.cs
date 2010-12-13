using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using EvolutionHighwayModel;

namespace EvolutionHighwayApp
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

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/genomes/{genome}/chromosomes/{chromosome}/species/{species}/synblocks")]
        [OperationContract(Name = "ListSynBlocksForSpecies", AsyncPattern = true)]
        IAsyncResult BeginListSynblocks(string genome, string chromosome, string species, AsyncCallback callback, object state = null);
        List<AncestorRegion> EndListSynblocks(IAsyncResult asyncResult);

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/speciesChromosomeLengths")]
        [OperationContract(Name = "GetSpeciesChromosomeLengths", AsyncPattern = true)]
        IAsyncResult BeginGetChromosomeLengths(AsyncCallback callback, object state = null);
        List<SpeciesChromosomeLengths> EndGetChromosomeLengths(IAsyncResult asyncResult);
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
