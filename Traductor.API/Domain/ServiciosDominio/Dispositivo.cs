using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using Traductor.API.Domain.ConstantesDominio;

namespace Traductor.API.Domain.ServiciosDominio
{
    public class Dispositivo
    {
        IConfiguration _configuration;

        public Dispositivo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Obtiene del appsettings.json la lista de dispositivos id y aplica el formato: disp_{0}_queue
        /// </summary>
        /// <returns></returns>
        public List<string> ListarColasDispositivosRegistrados()
        {
            try
            {
                List<string> _dispositivos = _configuration.GetSection("DispositivosId").Get<List<string>>();

                ///Formatear dispositivosID
                List<string> _queueNamesDispositivos = new List<string>();
                foreach (string item in _dispositivos)
                {
                    _queueNamesDispositivos.Add(string.Format(Constantes.QueueNameDispositivo.QUEUENAMEDISPFORMATO, item));
                }

                return _queueNamesDispositivos;
            }
            catch (Exception ex)
            {
                Log.Error(ex, Constantes.MsgLog.ERRORMETODO, nameof(ListarColasDispositivosRegistrados));
                throw ex;
            }
        }
    }
}
