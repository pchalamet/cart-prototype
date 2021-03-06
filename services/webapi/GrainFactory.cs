﻿using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapi
{
    public interface IGrainFactory
    {
        T GetGrain<T>(long id) where T : IGrainWithIntegerKey;
    }

    public class GrainFactory : IGrainFactory
    {
        private Lazy<IClusterClient> _clusterClient = new Lazy<IClusterClient>(ClusterClientBuilder);

        private static IClusterClient ClusterClientBuilder()
        {
            // connect to orleans cluster
            var client = new ClientBuilder()
                            .UseLocalhostClustering()
                            .Configure<ClusterOptions>(options =>
                            {
                                options.ClusterId = "dev";
                                options.ServiceId = "CartService";
                            })
                            .ConfigureLogging(logging => logging.AddConsole())
                            .Build();
            client.Connect().Wait();
            return client;
        }

        public T GetGrain<T>(long id) where T : IGrainWithIntegerKey
        {
            return _clusterClient.Value.GetGrain<T>(id);
        }
    }
}
