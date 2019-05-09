using Apache.NMS;
using Apache.NMS.ActiveMQ;

namespace Traductor.API.Aplicacion.ServiciosAplicacion
{
    public class ActiveMQ
    {
        #region Atributos
        private IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private ISession _session;

        #endregion

        protected void CrearConexion(string uri, AcknowledgementMode ackMode)
        {
            if (_connectionFactory == null)
            {
                _connectionFactory = new ConnectionFactory(uri);
                if (Connection == null)
                {
                    Connection _connection = (Connection)_connectionFactory.CreateConnection(UserName, Password);
                    _connection.PrefetchPolicy.All = 10;

                    Connection = _connection;
                    Connection.Start();
                    Session = Connection.CreateSession(ackMode);
                }
            }
        }

        protected IConnection CrearSessionAtomica(string uri, AcknowledgementMode ackMode, string username, string password, string queueName)
        {
            ConnectionFactory conexionFactoryAto = new ConnectionFactory(uri);
            IConnection conexionAtomica = conexionFactoryAto.CreateConnection(username, password);
            conexionAtomica.Start();
            //ISession session = conexionAtomica.CreateSession(ackMode);
            return conexionAtomica;
                //conexionAtomica.Close();
        }

        #region Propiedades
        /// <summary>
        /// Ruta servidor broker.
        /// </summary>
        public string ServidorBrokerUri { get; set; }

        /// <summary>
        /// Credencial usuario de acceso al servidor.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Credencial password de acceso al servidor.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Nombre de la cola para el ActiveMQ.
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Tiempo espera Para recbir mensajes nuevos desde el mom
        /// </summary>
        public int TiempoEsperaRecepcionMensaje { get; set; }

        /// <summary>
        /// Modo de encolamiento.
        /// </summary>
        public MQMode MQMode { get; set; }
        public IConnectionFactory ConnectionFactory { get => _connectionFactory; set => _connectionFactory = value; }
        public IConnection Connection { get => _connection; set => _connection = value; }
        public ISession Session { get => _session; set => _session = value; }
        #endregion
    }


    public enum MQMode
    {
        Queue,
        Topic
    }
}
