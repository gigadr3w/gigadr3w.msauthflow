using System.IO.Compression;
using System.Text;

namespace gigadr3w.msauthflow.common.Handlers
{
    public static class CompressionHandler
    {
        public static byte[] Compress(string value)
        {
            byte[] stringBuffer = Encoding.UTF8.GetBytes(value);

            using (MemoryStream ms = new())
            {
                using (GZipStream zip = new(ms, CompressionMode.Compress))
                    zip.Write(stringBuffer, 0, stringBuffer.Length);
                return ms.ToArray();
            }   
        }

        public static string Decompress(byte[] value)
        {
            using(MemoryStream input = new(value))
            {
                using (MemoryStream output = new())
                {
                    using (GZipStream zip = new(input, CompressionMode.Decompress))
                    {
                        zip.CopyTo(output);
                        return Encoding.UTF8.GetString(output.ToArray());                        
                    }    
                }
            }

            
        }
    }
}
