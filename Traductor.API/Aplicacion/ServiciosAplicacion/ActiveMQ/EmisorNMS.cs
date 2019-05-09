using Apache.NMS;
using Apache.NMS.Util;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Traductor.API.Domain.ConstantesDominio;

namespace Traductor.API.Aplicacion.ServiciosAplicacion
{
    public class EmisorNMS : ActiveMQ
    {
        #region Atributos
        private IConfiguration _configuration;

        #endregion

        #region Constructores

        public EmisorNMS(IConfiguration configuration)
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
        /// Envia mensaje al ActiveMQ y lo traduce si es requerido. [lanza excepcion]
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task EmitirMensajeActiveMQAsync(string message, string queueNameDestino)
        {
            try
            {
                CrearConexion(ServidorBrokerUri, AcknowledgementMode.DupsOkAcknowledge);

                IDestination destination = SessionUtil.GetDestination(Session, queueNameDestino);
                using (IMessageProducer producer = Session.CreateProducer(destination))
                {
                    ITextMessage textMessage = producer.CreateTextMessage(message);
                    producer.Send(textMessage);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, Constantes.MsgLog.ERRORMETODO, nameof(EmitirMensajeActiveMQAsync));
                throw ex;
            }
            finally
            {
                //if (Connection != null)
                //{
                //    Connection.Close();
                //}
            }
        }


        /// <summary>
        /// Envia mensaje al ActiveMQ y lo traduce si es requerido. [lanza excepcion]
        /// </summary>
        /// <param name="message"></param>
        /// <param name="queueNameDestino"></param>
        /// <param name="propertyValue">Valor de la propiedad adicional del objeto ITextMessage (dispositivoId)</param>
        /// <returns></returns>
        public async Task EmitirMensajeActiveMQAsync(string message, string queueNameDestino, string propertyValue)
        {
            try
            {
                CrearConexion(ServidorBrokerUri, AcknowledgementMode.AutoAcknowledge);

                IDestination destination = SessionUtil.GetDestination(Session, queueNameDestino);
                using (IMessageProducer producer = Session.CreateProducer(destination))
                {
                    ITextMessage textMessage = producer.CreateTextMessage(message);
                    textMessage.Properties.SetString("dispositivoId", propertyValue);
                    producer.Send(textMessage);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, Constantes.MsgLog.ERRORMETODO, nameof(EmitirMensajeActiveMQAsync));
                throw ex;
            }
            finally
            {
                //if (Connection != null)
                //{
                //    Connection.Close();
                //}
            }
        }

        /// <summary>
        /// Envia un mensaje vacío a la cola especificada.
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public void CrearSessionAtomica(string queueName)
        {
            try
            {

                IConnection connection = CrearSessionAtomica(ServidorBrokerUri,
                    AcknowledgementMode.AutoAcknowledge,
                    UserName,
                    Password,
                    queueName);

                using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
                {
                    IDestination destination = SessionUtil.GetDestination(session, queueName);
                    using (IMessageProducer producer = session.CreateProducer(destination))
                    {
                        ITextMessage textMessage = producer.CreateTextMessage();
                        producer.DeliveryMode = MsgDeliveryMode.NonPersistent;
                        producer.DisableMessageID = true;
                        producer.DisableMessageTimestamp = true;
                        producer.Send(textMessage);
                        producer.Priority = MsgPriority.Lowest;
                        producer.TimeToLive = TimeSpan.MinValue;
                        producer.Close();
                        //connection.PurgeTempDestinations();
                    }

                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex, Constantes.MsgLog.ERRORMETODO, nameof(CrearSessionAtomica));
                throw ex;
            }
        }

        #endregion
    }
}
