using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Traductor.API.Domain.EntidadesDominio
{
    public class TareaEjecucion
    {
        #region Atributos
        private string _estadoEjecucionAsString;
        #endregion

        #region Propiedades
        [JsonIgnore]
        public Task Tarea { get; set; }


        [JsonProperty("NombreTarea")]
        public string NombreTarea { get; set; }

        
        [JsonProperty("EstadoEjecucion")]
        public string EstadoEjecucion
        {
            get
            {
                if (Tarea != null && 
                    (Tarea.Status == TaskStatus.Running || 
                    Tarea.Status == TaskStatus.WaitingToRun || 
                    Tarea.Status == TaskStatus.WaitingForActivation))
                {
                    _estadoEjecucionAsString = EnumeradosEstadoEjecucion.Corriendo.ToString();
                }
                else
                {
                    _estadoEjecucionAsString = EnumeradosEstadoEjecucion.Detenida.ToString();
                }

                return _estadoEjecucionAsString;
            }
            set
            {
                _estadoEjecucionAsString = value;
            }
        }

        [JsonProperty("FechaSolicitud")]
        public DateTime FechaSolicitud { get => DateTime.Now;}
        #endregion

    }

    public enum EnumeradosEstadoEjecucion
    {
        Corriendo,
        Detenida,
        CorriendoConFallas,
        Ejecutada
    }
}
