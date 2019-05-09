using Apache.NMS;
using System;
using Traductor.API.Domain.ConstantesDominio;

namespace Traductor.API.Domain.EntidadesDominio
{
    public class CondicionesMensaje
    {
        #region Atributos
        private string _dispositivoId;

        private string _queueName;

        private string _messageText;

        private IMessage _message;

        private EnumeradosTraductor.DireccionMensaje _direccionMensaje;

        private EnumeradosTraductor.TipoConversionMensaje _tipoConversionMensaje;
        #endregion

        #region Propiedades
        public string MessageText { get => _messageText; set => _messageText = value; }
        public IMessage Message { get => _message; set => _message = value; }

        public EnumeradosTraductor.DireccionMensaje DireccionMensaje { get => _direccionMensaje; set => _direccionMensaje = value; }

        public EnumeradosTraductor.TipoConversionMensaje TipoConversionMensaje { get => _tipoConversionMensaje; set => _tipoConversionMensaje = value; }

        public string QueueName { get => _queueName; set => _queueName = value; }
        public string DispositivoId { get => _dispositivoId; set => _dispositivoId = value; }
        #endregion

        #region Constructores
        public CondicionesMensaje(IMessage message, EnumeradosTraductor.DireccionMensaje direccion,
            EnumeradosTraductor.TipoConversionMensaje tipo, string queueNameDestino)
        {
            if (EnumeradosTraductor.DireccionMensaje.SistemaCentral_A_DispositivosBorde == direccion
                && string.IsNullOrEmpty(queueNameDestino))
            {
                throw new ArgumentNullException("El campo " + nameof(queueNameDestino) + " es requerido. ");
            }

            _queueName = queueNameDestino;
            _message = message ?? throw new ArgumentNullException("El campo " + nameof(message) + " es requerido. ");
            ITextMessage textMessage = _message as ITextMessage;

            if (EnumeradosTraductor.DireccionMensaje.DispositivosBorde_A_SistemaCentral == direccion)
            {
                _dispositivoId = textMessage.Properties.GetString("dispositivoId");
                if (string.IsNullOrEmpty(DispositivoId))
                {
                    throw new ArgumentNullException("El campo " + nameof(DispositivoId) + " es requerido. ");
                }
            }
            
            _messageText = textMessage.Text;
            _direccionMensaje = direccion;
            _tipoConversionMensaje = tipo;

        } 
        #endregion
    }

    public class CustomTextMessage : ITextMessage
    {
        public CustomTextMessage(string mensaje)
        {
            _mensaje = mensaje;
        }

        private string _mensaje;

        string ITextMessage.Text { get => _mensaje; set => _mensaje=value; }

        IPrimitiveMap IMessage.Properties => throw new NotImplementedException();

        string IMessage.NMSCorrelationID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IDestination IMessage.NMSDestination { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        TimeSpan IMessage.NMSTimeToLive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string IMessage.NMSMessageId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        MsgDeliveryMode IMessage.NMSDeliveryMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        MsgPriority IMessage.NMSPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool IMessage.NMSRedelivered { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IDestination IMessage.NMSReplyTo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        DateTime IMessage.NMSTimestamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string IMessage.NMSType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        void IMessage.Acknowledge()
        {
            throw new NotImplementedException();
        }

        void IMessage.ClearBody()
        {
            throw new NotImplementedException();
        }

        void IMessage.ClearProperties()
        {
            throw new NotImplementedException();
        }
    }
}
