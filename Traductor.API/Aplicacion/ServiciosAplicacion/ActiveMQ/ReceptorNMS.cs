using Apache.NMS;
using Apache.NMS.Util;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Traductor.API.Domain.ConstantesDominio;
using Traductor.API.Domain.EntidadesDominio;

namespace Traductor.API.Aplicacion.ServiciosAplicacion
{
    public class ReceptorNMS : ActiveMQ
    {
        public delegate void EventHandler(CondicionesMensaje c);

        public event EventHandler MensajeRecibidoEvento;

        public void OnMensajeRecibido(CondicionesMensaje c)
        {
            if (MensajeRecibidoEvento != null)
                MensajeRecibidoEvento(c);
        }

        #region Atributos
        private IConfiguration _configuration;

        #endregion


        #region Constructor

        public ReceptorNMS(IConfiguration configuration)
        {
            _configuration = configuration;

            ServidorBrokerUri = _configuration.GetValue<string>("ActiveMQ:Servidor");
            UserName = _configuration.GetValue<string>("ActiveMQ:Usuario");
            Password = _configuration.GetValue<string>("ActiveMQ:Password");
            TiempoEsperaRecepcionMensaje = _configuration.GetValue<int>("ActiveMQ:TiempoEsperaRecepcionMensaje");
        }


        #endregion

        #region Metodos

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <param name="queueNameOrigenes"></param>
        /// <param name="direccion"></param>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public async Task TareaRecibirMensajesActiveMQAsync(CancellationToken stoppingToken,
            List<string> queueNameOrigenes,
            EnumeradosTraductor.DireccionMensaje direccion,
            EnumeradosTraductor.TipoConversionMensaje tipo)
        {
            try
            {
                string _queueName = string.Empty;
                CrearConexion(ServidorBrokerUri, AcknowledgementMode.DupsOkAcknowledge);

                int _contadorDispositivos = 0;
                IMessage message;
                while (!stoppingToken.IsCancellationRequested)///Determinar rompimiento del ciclo
                {
                    try
                    {
                        if (queueNameOrigenes.Count <= _contadorDispositivos)
                            _contadorDispositivos = 0;

                        _queueName = queueNameOrigenes[_contadorDispositivos];

                        IDestination dest = SessionUtil.GetDestination(Session, _queueName);
                        using (IMessageConsumer consumer = Session.CreateConsumer(dest))
                        {
                            message = consumer.Receive(TimeSpan.FromMilliseconds(TiempoEsperaRecepcionMensaje));
                            if (message != null)
                            {
                                CondicionesMensaje condicionesMensaje = new CondicionesMensaje(
                                    message,
                                    direccion,
                                    tipo,
                                    _queueName
                                    );

                               OnMensajeRecibido(condicionesMensaje);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, string.Format(Constantes.MsgLog.ERRORMETODO, nameof(RecibirMensajeActiveMQAsync),_queueName));
                    }
                    _queueName = string.Empty;
                    _contadorDispositivos++;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, Constantes.MsgLog.ERRORMETODO, nameof(RecibirMensajeActiveMQAsync));
                throw ex;
            }
            finally
            {
                if (Connection != null)
                {
                    Connection.Close();
                }
            }
        }

        /// <summary>
        /// Obtiene el ultimo mensaje encolado del MOM.
        /// </summary>
        /// <returns>Mensaje en cadena.</returns>
        public async Task RecibirMensajeActiveMQAsync(List<string> queueNameOrigenes,
            EnumeradosTraductor.DireccionMensaje direccion,
            EnumeradosTraductor.TipoConversionMensaje tipo)
        {
            try
            {
                string _queueName = string.Empty;
                CrearConexion(ServidorBrokerUri, AcknowledgementMode.ClientAcknowledge);
                _queueName = queueNameOrigenes[0];

                IDestination dest = SessionUtil.GetDestination(Session, QueueName);
                using (IMessageConsumer consumer = Session.CreateConsumer(dest))
                {
                    IMessage message;
                    message = consumer.Receive(TimeSpan.FromSeconds(TiempoEsperaRecepcionMensaje));
                    if (message != null)
                    {
                        CondicionesMensaje condicionesMensaje = new CondicionesMensaje(
                                    message,
                                    direccion,
                                    tipo,
                                    _queueName);

                        OnMensajeRecibido(condicionesMensaje);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, Constantes.MsgLog.ERRORMETODO, nameof(RecibirMensajeActiveMQAsync));
                throw ex;
            }
            finally
            {
                if (Connection != null)
                {
                    Connection.Close();
                }
            }
        }

        #endregion

    }
}
