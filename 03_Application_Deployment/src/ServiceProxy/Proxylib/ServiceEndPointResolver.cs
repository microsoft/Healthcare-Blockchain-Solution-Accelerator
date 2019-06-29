using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Text;
using System.Threading;

namespace Proxylib
{
    public static class ServiceEndPointResolver
    {
        public static string GetServiceEndPoint(string fabricName, EventSource currentEventSource = null)
        {
            ServicePartitionResolver resolver = ServicePartitionResolver.GetDefault();
            CancellationToken token = new CancellationToken(false);
            ResolvedServicePartition partition = resolver.ResolveAsync(new Uri(fabricName), new ServicePartitionKey(), token).Result;
            string address = partition.GetEndpoint().Address;
            var endpointAddress =
                        JObject.Parse(address)["Endpoints"][""].ToString();

            //if (currentEventSource != null) currentEventSource.Write("ServiceEndPointResolver", $"fabric Name : {fabricName} / resolved url : {endpointAddress}");

            return endpointAddress;
        }
    }

    public struct ServiceEndpoints
    {
        public const string trackerEndPoint = "fabric:/Healthcare.AppHosting/Healthcare.BC.Tracker.API";
        public const string indexerEndPoint = "fabric:/Healthcare.AppHosting/Healthcare.BC.Indexer.API";
        public const string escEndPoint = "fabric:/Healthcare.AppHosting/Healthcare.BC.Chain.API";
        public const string proofsvcEndPoint = "fabric:/Healthcare.AppHosting/Healthcare.Proofing.API";
    }
}
