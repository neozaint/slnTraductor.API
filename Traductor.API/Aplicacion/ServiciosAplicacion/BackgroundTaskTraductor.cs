using Microsoft.Extensions.Configuration;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Traductor.API.Domain.ConstantesDominio;
using Traductor.API.Domain.EntidadesDominio;
using Traductor.API.Infraestructure.BackgroundTask;

namespace Traductor.API.Aplicacion.ServiciosAplicacion
{
    /// <summary>
    /// Start y Stop personalizados para gestion de objetos Task.
    /// </summary>
    public class BackgroundTaskTraductor : IBackgroundTask
    {
        #region Atributos
        private CancellationTokenSource _cancellationToken;
        private List<TareaEjecucion> _tareas;
        private ControlMensajeriaActiveMQ _controlMensajeriaActiveMQ;
        #endregion

        #region Propiedades
        public CancellationTokenSource CancellationToken { get => _cancellationToken; set => _cancellationToken = value; }

        public bool EstadoEjecucion => ObtenerEstadoGeneralTareasEjecucion();

        public List<TareaEjecucion> EstadoTareasEjecucion => _tareas; 
        #endregion

        public void Dispose()
        {
            if (CancellationToken != null)
            {
                CancellationToken.Cancel();
            }
        }

        /// <summary>
        /// Inicia las tareas IniciarTareaDispositivosBorde_A_SistemaCentral, IniciarTareaSistemaCentral_A_DispositivosBorde.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public Task StartAsync(IConfiguration configuration)
        {
            Log.Information(string.Format(Constantes.MsgLog.INICIOMETODO, System.Reflection.MethodInfo.GetCurrentMethod().Name));

            if (EstadoEjecucion == false)
            {
                CancellationToken = new CancellationTokenSource();
                _controlMensajeriaActiveMQ = new ControlMensajeriaActiveMQ(configuration);

                _tareas = new List<TareaEjecucion>();

                //_controlMensajeriaActiveMQ.CrearColasDispositivos();
                _tareas.Add(_controlMensajeriaActiveMQ.IniciarTareaCrearColasDispositivos(CancellationToken));
                _tareas.Add(_controlMensajeriaActiveMQ.IniciarTareaDispositivosBorde_A_SistemaCentral(CancellationToken));
                _tareas.Add(_controlMensajeriaActiveMQ.IniciarTareaSistemaCentral_A_DispositivosBorde(CancellationToken));
            }

            Log.Information(string.Format(Constantes.MsgLog.FINMETODO, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            return Task.CompletedTask;

        }

        /// <summary>
        /// Detiene todas las tareas que esten en ejecución sobre el objeto TareaEjecucion y se hayan definido con CancellationToken.
        /// </summary>
        /// <returns></returns>
        public Task StopAsync()
        {
            Log.Information(string.Format(Constantes.MsgLog.INICIOMETODO, System.Reflection.MethodInfo.GetCurrentMethod().Name));

            foreach (TareaEjecucion tarea in _tareas)
            {
                if (tarea.EstadoEjecucion == EnumeradosEstadoEjecucion.Corriendo.ToString())
                {
                    CancellationToken.Cancel();
                }
            }

            Log.Information(string.Format(Constantes.MsgLog.FINMETODO, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            return Task.CompletedTask;
        }

        private bool ObtenerEstadoGeneralTareasEjecucion()
        {
            Log.Information(string.Format(Constantes.MsgLog.INICIOMETODO, System.Reflection.MethodInfo.GetCurrentMethod().Name));

            bool estado = false;
            if (_tareas != null && _tareas.Count > 0)
            {
                estado = _tareas.All(x => x.EstadoEjecucion == EnumeradosEstadoEjecucion.Corriendo.ToString());
            }
            else
            {
                estado = false;
            }

            Log.Information(string.Format(Constantes.MsgLog.FINMETODO, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            return estado;
        }

    }
}
