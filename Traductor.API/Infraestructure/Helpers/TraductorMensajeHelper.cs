using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traductor.API.Infraestructure.Helpers
{
    public static class TraductorMensajeHelper
    {

        private static byte[] _bDecodificado;
        private static byte[] _bDescomprimido;

        private static byte[] _bCodificado;
        private static byte[] _bComprimido;

        /// <summary>
        /// Decodifica el mensaje, lo descomprime y lo convierte a un formato legible.
        /// </summary>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        public static string TraducirMensajeCodificado(string mensaje)
        {
            _bDecodificado = null;
            _bDescomprimido = null;

            _bDecodificado = EncoderHelper.ConvertirISO(mensaje);
            _bDescomprimido = CompresionHelper.UnZip(_bDecodificado);
            return Encoding.UTF8.GetString(_bDescomprimido);
        }

        /// <summary>
        /// Codifica el mensaje, lo comprime y lo convierte a un formato ISO latin.
        /// </summary>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        public static string TraducirMensajeDecodificado(string mensaje)
        {
            _bCodificado = null;
            _bComprimido = null;

            _bCodificado = EncoderHelper.CodificarUTF8_ISO(mensaje);
            _bComprimido = CompresionHelper.Zip(_bCodificado);

            Encoding iso = Encoding.GetEncoding("iso-8859-1");
            string msg = iso.GetString(_bComprimido);

            return msg;
        }
    }
}