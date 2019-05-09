using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traductor.API.Infraestructure.Helpers
{
    public class EncoderHelper
    {

        /// <summary>
        /// Codifica de UTF8 a iso-8859-1.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] CodificarUTF8_ISO(string msjNativo)
        {
            byte[] data;
            Encoding iso = Encoding.GetEncoding("iso-8859-1");
            Encoding utf8 = Encoding.UTF8;

            data = utf8.GetBytes(msjNativo);

            byte[] isoBytes = Encoding.Convert(utf8, iso, data);


            return isoBytes;
        }

        /// <summary>
        /// Convierte iso-8859-1 mensaje a byte.
        /// </summary>
        /// <param name="msjISO"></param>
        /// <returns></returns>
        public static byte[] ConvertirISO(string msjISO)
        {
            Encoding iso = Encoding.GetEncoding("iso-8859-1");
            byte[] utfBytes = iso.GetBytes(msjISO);
            return utfBytes;
        }

        /// <summary>
        /// Decodifica iso-8859-1 a UTF8.
        /// </summary>
        /// <param name="msjISO"></param>
        /// <returns></returns>
        public static byte[] DecodificarUTF(byte[] msjISO)
        {
            Encoding iso = Encoding.GetEncoding("iso-8859-1");
            Encoding utf8 = Encoding.UTF8;
            byte[] isoBytes = Encoding.Convert(iso, utf8, msjISO);
            return isoBytes;
        }

        internal static byte[] ConvertirISO(Task<string> msg)
        {
            throw new NotImplementedException();
        }
    }
}
