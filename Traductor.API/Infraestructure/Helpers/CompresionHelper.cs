using System.IO;
using System.IO.Compression;

namespace Traductor.API.Infraestructure.Helpers
{
    public static class CompresionHelper
    {
        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        /// <summary>
        /// Comprime usando GZipStream.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] Zip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        /// <summary>
        /// Descomprime usando GZipStream.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] UnZip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }
                return mso.ToArray();
            }
        }

    }
}
