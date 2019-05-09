using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Traductor.API.Aplicacion.ServiciosAplicacion;
using Traductor.API.Domain.ConstantesDominio;
using Traductor.API.Domain.ServiciosDominio;
using Traductor.API.Infraestructure.BackgroundTask;

namespace Traductor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraductorActiveMQController : ControllerBase
    {
        private ObjectResult _response;
        private ControlMensajeriaActiveMQ _controlMensajeriaActiveMQ;
        private Dispositivo _dispositivo;
        private IConfiguration _configuration;
        private IBackgroundTask _backgroundTask;

        public TraductorActiveMQController(IConfiguration configuration, IBackgroundTask backgroundTask)
        {
            _configuration = configuration;
            _controlMensajeriaActiveMQ = new ControlMensajeriaActiveMQ(configuration);
            _dispositivo = new Dispositivo(configuration);
            _backgroundTask = backgroundTask;
        }

        [HttpGet("Saludar")]
        public IActionResult Saludar() => Ok("Hola soy: " + System.Reflection.Assembly.GetExecutingAssembly().FullName);

        /// <summary>
        /// Inicia la obtención de mensajes y publicación de mensajes de el activeMQ. [No lanza excepcion]
        /// </summary>
        /// <returns></returns>
        [HttpGet("IniciarOperacion")]
        public async Task<IActionResult> IniciarOperacionAsync()
        {
            try
            {
                await _backgroundTask.StartAsync(_configuration);
                Log.Information(Constantes.Operacion.INICIOOPERACION);
                _response = Ok(Constantes.Operacion.INICIOOPERACION);
            }
            catch (Exception ex)
            {
                Log.Error(ex, Constantes.MsgLog.ERRORMETODO, nameof(IniciarOperacionAsync));
                _response = StatusCode(HttpStatusCode.InternalServerError.GetHashCode(),
                    string.Format(Constantes.Tarea.FALLOTAREA, ex.Message, ex.StackTrace));
            }
            return _response;
        }

        /// <summary>
        /// Detiene la operación de obtención y publicación de mensajes de el mom. [No lanza excepcion]
        /// </summary>
        /// <returns></returns>
        [HttpGet("DetenerOperacion")]
        public async Task<IActionResult> DetenerOperacionAsync()
        {
            try
            {
                await _backgroundTask.StopAsync();
                Log.Information(Constantes.Operacion.DETENCIONOPERACION);
                _response = Ok(Constantes.Operacion.DETENCIONOPERACION);
            }
            catch (Exception ex)
            {
                Log.Error(ex, Constantes.MsgLog.ERRORMETODO, nameof(DetenerOperacionAsync));
                _response = StatusCode(HttpStatusCode.InternalServerError.GetHashCode(),
                    string.Format(Constantes.Tarea.FALLOTAREA, ex.Message, ex.StackTrace));
            }
            return _response;
        }

        /// <summary>
        /// Método que retorna el estado de la tarea de publicación.
        /// </summary>
        /// <returns></returns>
        [HttpGet("ObtenerEstadoOperacion")]
        public IActionResult ObtenerEstadoOperacionAsync()
        {
            try
            {
                string _resLog,_resPet = string.Empty;
                _resLog = Constantes.Operacion.OBT_EST_OPERACION;
                

                if (_backgroundTask.EstadoTareasEjecucion == null)
                {
                    _resPet = _resPet + "\n "+ Constantes.Operacion.NO_HAY_TAREAS;
                }
                else
                {
                    _resPet = _resPet + "\n " + JsonConvert.SerializeObject(_backgroundTask.EstadoTareasEjecucion);
                }

                Log.Information(_resLog+" "+_resPet);
                _response = Ok(_resPet);
            }
            catch (Exception ex)
            {
                Log.Error(ex, Constantes.MsgLog.ERRORMETODO, nameof(ObtenerEstadoOperacionAsync));
                _response = StatusCode(HttpStatusCode.InternalServerError.GetHashCode(),
                    string.Format(Constantes.Tarea.FALLOTAREA, ex.Message, ex.StackTrace));
            }
            return _response;
        }

        //[HttpGet("EnviarMensajePrueba")]
        public async Task EnviarMensajePrueba()
        {
            string sMensaje = ObtenerArchivo();
            await _controlMensajeriaActiveMQ.EmitirMensajeActiveMQAsync(sMensaje,"QueueName_Prueba");
        }

        public string ObtenerArchivo()
        {
            return @"checkInternet: -iw wlan0 link
                reloadWpaSupplicant: -killall wpa_supplicant
                                     - killall udhcpc
                                      - wpa_supplicant - i wlan0 - B - C / var / run / wpa_supplicant


                scan: -wpa_cli SCAN
                scanResults: -wpa_cli SCAN_RESULTS
                listNetworks: -wpa_cli LIST_NETWORKS
                addNetwork: -wpa_cli ADD_NETWORK
                setNetwork: -wpa_cli SET_NETWORK 0 ssid \Fabiolo\ 
								                 0 psk \123456789\
                enableNetwork: -wpa_cli ENABLE_NETWORK 0
                selectNetwork: -wpa_cli SELECT_NETWORK 0
                connect: -udhcpc - i wlan0 - b
                status: -wpa_cli STATUS

                ";
            return System.IO.File.ReadAllText(@"C:\Users\852038\Desktop\MensajeUsos_Nativo.txt");
        }

        [HttpGet("ListarDispositivos")]
        public async Task<IActionResult> ListarDispositivosAsync()
        {
            try
            {
                List<string> _dispositivosId =_dispositivo.ListarColasDispositivosRegistrados();
                string json=JsonConvert.SerializeObject(_dispositivosId);

                _response = Ok(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex, Constantes.MsgLog.ERRORMETODO, nameof(ListarDispositivosAsync));
                _response = StatusCode(HttpStatusCode.InternalServerError.GetHashCode(),
                    string.Format(Constantes.Tarea.FALLOTAREA, ex.Message, ex.StackTrace));
            }

            return _response;
            
        }

        [HttpGet("CrearColasDispositivos")]
        public async Task<IActionResult> CrearColasDispositivosAsync()
        {
            try
            {
                List<string> _dispositivosId = await _controlMensajeriaActiveMQ.CrearColasDispositivos();
                string json = JsonConvert.SerializeObject(_dispositivosId);
                _response = Ok(Constantes.Operacion.CREADO_COLAS +json);
            }
            catch (Exception ex)
            {
                Log.Error(ex, Constantes.MsgLog.ERRORMETODO, nameof(CrearColasDispositivosAsync));
                _response = StatusCode(HttpStatusCode.InternalServerError.GetHashCode(),
                    string.Format(Constantes.Tarea.FALLOTAREA, ex.Message, ex.StackTrace));
            }

            return _response;

        }
    }
}