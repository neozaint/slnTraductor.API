namespace Traductor.API.Domain.ConstantesDominio
{

    public class EnumeradosTraductor
    {
        public enum DireccionMensaje
        {
            DispositivosBorde_A_SistemaCentral,
            SistemaCentral_A_DispositivosBorde
        }

        public enum TipoConversionMensaje
        {
            Cifrar,
            Decifrar,
            Nada

        }
    }

    public class Constantes
    {

        public struct MsgLog
        {
            public const string ERRORMETODO = "Se presentó un error en la ejecución del metodo {0} : {1} ";
            public const string INICIOMETODO = "Inicio del metodo {0} ";
            public const string FINMETODO = "Fin del metodo {0} ";
        }

        public struct MTI
        {
            public const string CODIGODEFAULT = "[{(91001)}]";
        }

        public struct QueueNameDispositivo
        {
            public const string QUEUENAMEDISPFORMATO = "disp_{0}_queue";
            public const string QUEUENAMEDISPFORMATO_SINCIFRAR = "{0}_SinCifrar";
        }

        public struct Tarea
        {
            public const string NOPARAMETROS = "* No hay parametros para la ejecución.";
            public const string EJECUTOTAREA = "* Se ejecutó la tarea.";
            public const string FALLOTAREA = "* La tarea ha lanzado una excepción; - Message: {0}. - StackTrace:{1}.";
        }

        public struct Operacion
        {
            public const string DETENCIONOPERACION = "* Se ejecutó la detención de la operación.";
            public const string INICIOOPERACION = "* Se ejecutó la inicialización de la operación.";
            public const string OBT_EST_OPERACION = "* Se realizó una petición a la obtención de la operación.";
            public const string NO_HAY_TAREAS = "* No hay tareas en ejecución: instancia de _backgroundTask.EstadoTareasEjecucion==null.";
            public const string CREADO_COLAS = " * Se han creado las siguientes colas con mensajes vacios: ";
        }
    }
}
