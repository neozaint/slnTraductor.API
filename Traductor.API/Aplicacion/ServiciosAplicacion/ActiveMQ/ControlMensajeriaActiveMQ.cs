using Apache.NMS;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Traductor.API.Domain.ConstantesDominio;
using Traductor.API.Domain.EntidadesDominio;
using Traductor.API.Domain.ServiciosDominio;
using Traductor.API.Infraestructure.Helpers;

namespace Traductor.API.Aplicacion.ServiciosAplicacion
{
    /// <summary>
    /// Clase orquestadora de metodos de ReceptorNMS, EmisorNMS, ControlMensajeria.
    /// </summary>
    public class ControlMensajeriaActiveMQ
    {
        #region Atributos
        private ReceptorNMS _receptor;
        private EmisorNMS _emisor;
        private Dispositivo _dispositivo;

        private IConfiguration _configuration;
        #endregion

        #region Constructores

        public ControlMensajeriaActiveMQ(IConfiguration configuration)
        {
            _configuration = configuration;

            _receptor = new ReceptorNMS(_configuration);
            _receptor.MensajeRecibidoEvento += new ReceptorNMS.EventHandler(OnMensajeRecibido);

            _emisor = new EmisorNMS(_configuration);
            _dispositivo = new Dispositivo(_configuration);
            
        }

        public ControlMensajeriaActiveMQ()
        { }

        #endregion

        #region Metodos

        public void OnMensajeRecibido(CondicionesMensaje condicionesMensaje)
        {
            try
            {
                EmitirMensajeActiveMQAsync(condicionesMensaje).Wait();

                condicionesMensaje.Message.Acknowledge();///Reconocimiento

            }
            catch (Exception ex)
            {
                Log.Error(ex, Constantes.MsgLog.ERRORMETODO, nameof(OnMensajeRecibido));
                throw ex;
            }

        }


        public async Task<string> ConvertirMensaje(CondicionesMensaje condicionesMensaje)
        {
            string mensaje = string.Empty;
            switch (condicionesMensaje.TipoConversionMensaje)
            {
                case EnumeradosTraductor.TipoConversionMensaje.Cifrar:
                    {
                        mensaje = TraductorMensajeHelper.TraducirMensajeDecodificado(condicionesMensaje.MessageText);
                    }
                    break;

                case EnumeradosTraductor.TipoConversionMensaje.Decifrar:
                    {
                        mensaje = TraductorMensajeHelper.TraducirMensajeCodificado(condicionesMensaje.MessageText);
                    }
                    break;
                case EnumeradosTraductor.TipoConversionMensaje.Nada:
                    {
                        mensaje = condicionesMensaje.MessageText;
                    }
                    break;
            }

            return mensaje;
        }

        private async Task EmitirMensajeActiveMQAsync(CondicionesMensaje condicionesMensaje)
        {
            try
            {
                switch (condicionesMensaje.DireccionMensaje)
                {
                    case EnumeradosTraductor.DireccionMensaje.DispositivosBorde_A_SistemaCentral:
                        {
                            string mensajeConvertido = await ConvertirMensaje(condicionesMensaje);

                            await _emisor.EmitirMensajeActiveMQAsync(
                                mensajeConvertido,
                               _configuration.GetValue<string>("ActiveMQ:QueueName_Cifrado")
                               ,condicionesMensaje.DispositivoId
                                );
                        }
                        break;
                    case EnumeradosTraductor.DireccionMensaje.SistemaCentral_A_DispositivosBorde:
                        {
                            ///Si el mensaje es vacio fue porque la tarea de CrearColasDispositivos creó un mensaje vacio.
                            ///No se debe emitir el mensaje vacio.
                            if (!string.IsNullOrEmpty(condicionesMensaje.MessageText))
                            {
                                string mensajeConvertido = await ConvertirMensaje(condicionesMensaje);
                                string queueNameDispo = string.Format(Constantes.QueueNameDispositivo.QUEUENAMEDISPFORMATO_SINCIFRAR, condicionesMensaje.QueueName);

                                await _emisor.EmitirMensajeActiveMQAsync(
                                    mensajeConvertido,
                                    queueNameDispo
                                    );
                            }
                            
                        }
                        break;

                }

                
            }
            catch (Exception ex)
            {
                Log.Error(ex, Constantes.MsgLog.ERRORMETODO, nameof(EmitirMensajeActiveMQAsync));
            }

            
        }

        /// <summary>
        /// Encapsula el método Emisor.EmitirMensajeActiveMQAsync.
        /// </summary>
        /// <param name="mensaje"></param>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public async Task EmitirMensajeActiveMQAsync(string mensaje, string queueName)
        {
            await _emisor.EmitirMensajeActiveMQAsync(mensaje, queueName);
        }

        /// <summary>
        /// Define las condiciones del mensaje e inicia la tarea para traducir mensajes en dirección DispositivosBorde_A_SistemaCentral.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public TareaEjecucion IniciarTareaDispositivosBorde_A_SistemaCentral(CancellationTokenSource cancellationToken)
        {
            Log.Information(Constantes.MsgLog.INICIOMETODO,System.Reflection.MethodInfo.GetCurrentMethod().Name);

            ///Definir la cola origen
            List<string> queueNameOrigenes_1 = new List<string>();
            queueNameOrigenes_1.Add(_configuration.GetValue<string>("ActiveMQ:QueueName_SinCifrar"));

            TareaEjecucion _tareaEjecucion = new TareaEjecucion();
            _tareaEjecucion.NombreTarea = "Tarea DispositivosBorde_A_SistemaCentral";
            _tareaEjecucion.EstadoEjecucion = EnumeradosEstadoEjecucion.Corriendo.ToString();
            _tareaEjecucion.Tarea
                = Task.Run(
                    () => (
                    _receptor.TareaRecibirMensajesActiveMQAsync(
                        cancellationToken.Token,
                        queueNameOrigenes_1,
                        EnumeradosTraductor.DireccionMensaje.DispositivosBorde_A_SistemaCentral,
                        EnumeradosTraductor.TipoConversionMensaje.Cifrar)
                        )
                        );

            Log.Information(Constantes.MsgLog.FINMETODO, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            return _tareaEjecucion;
        }

        /// <summary>
        /// Define las condiciones del mensaje e inicia la tarea para traducir mensajes en dirección SistemaCentral_A_DispositivosBorde.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public TareaEjecucion IniciarTareaSistemaCentral_A_DispositivosBorde(CancellationTokenSource cancellationToken)
        {
            Log.Information(Constantes.MsgLog.INICIOMETODO, System.Reflection.MethodInfo.GetCurrentMethod().Name);

            ///Definir la cola origen
            List<string> queueNameOrigenes_2 = _dispositivo.ListarColasDispositivosRegistrados();

            TareaEjecucion _tareaEjecucion = new TareaEjecucion();
            _tareaEjecucion.NombreTarea = "Tarea SistemaCentral_A_DispositivosBorde";
            _tareaEjecucion.EstadoEjecucion = EnumeradosEstadoEjecucion.Corriendo.ToString();
            _tareaEjecucion.Tarea
                = Task.Run(
                    () => (
                    _receptor.TareaRecibirMensajesActiveMQAsync(
                    cancellationToken.Token,
                    queueNameOrigenes_2,
                    EnumeradosTraductor.DireccionMensaje.SistemaCentral_A_DispositivosBorde,
                    EnumeradosTraductor.TipoConversionMensaje.Decifrar)
                        )
                        );

            Log.Information(Constantes.MsgLog.FINMETODO, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            return _tareaEjecucion;
        }

        /// <summary>
        /// Crea colas con la lista de dispositivos.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Obsolete("Su estado siempre queda en detenida.")]
        public TareaEjecucion IniciarTareaCrearColasDispositivos(CancellationTokenSource cancellationToken)
        {
            Log.Information(Constantes.MsgLog.INICIOMETODO, System.Reflection.MethodInfo.GetCurrentMethod().Name);

            TareaEjecucion _tareaEjecucion = new TareaEjecucion();
            _tareaEjecucion.NombreTarea = "Tarea CrearColasDispositivos";
            _tareaEjecucion.EstadoEjecucion = EnumeradosEstadoEjecucion.Ejecutada.ToString();
            _tareaEjecucion.Tarea = Task.Run(()=>CrearColasDispositivos());

            Log.Information(Constantes.MsgLog.FINMETODO, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            return _tareaEjecucion;
        }

        /// <summary>
        /// Consulta los dispositivosId del appsettings.json, le aplica formato y envia un mensaje vacío con la cola del formato aplicado.
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> CrearColasDispositivos()
        {
            List<string> queueNames = new List<string>();
            try
            {
                queueNames.AddRange(_dispositivo.ListarColasDispositivosRegistrados());

                foreach (string item in queueNames)
                {
                    _emisor.CrearSessionAtomica(item);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, Constantes.MsgLog.ERRORMETODO, nameof(EmitirMensajeActiveMQAsync));
            }
            return queueNames;
        }

        #endregion
    }
}
