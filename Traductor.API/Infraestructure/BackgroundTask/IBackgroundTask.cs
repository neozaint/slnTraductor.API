using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Traductor.API.Domain.EntidadesDominio;

namespace Traductor.API.Infraestructure.BackgroundTask
{
    public interface IBackgroundTask : IDisposable
    {
        CancellationTokenSource CancellationToken { get; set; }

        [JsonProperty("EstadoTareasEjecucion")]
        List<TareaEjecucion> EstadoTareasEjecucion { get; }


        //
        // Summary:
        //     Triggered when the application host is ready to start the service.
        Task StartAsync(IConfiguration configuration);
        //
        // Summary:
        //     Triggered when the application host is performing a graceful shutdown.
        Task StopAsync();

    }
}
