using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;

namespace BioProject.Helper
{
    public class Utils
    {
        public static string GenerateDigits(int length)
        {
            var rndDigits = new System.Text.StringBuilder().Insert(0, "0123456789", length).ToString().ToCharArray();
            return string.Join("", rndDigits.OrderBy(o => Guid.NewGuid()).Take(length));
        }

        public static string DecompressToBase64(string gzipInputString)
        {
            try
            {
                byte[] compressed = Convert.FromBase64String(gzipInputString);

                MemoryStream inputStream = new MemoryStream(compressed);
                MemoryStream outputStream = new MemoryStream(10024);
                using (var csStream = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(inputStream))
                {
                    byte[] buffer = new byte[10024];
                    int nRead;
                    while ((nRead = csStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outputStream.Write(buffer, 0, nRead);
                    }
                    //return Convert.ToBase64String(outputStream.ToArray());
                    return Encoding.UTF8.GetString(outputStream.ToArray());
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error occured: " + e.Message);
                return null;
            }

            return null;

        }

        public static string DecompressToBase64FromDesktop(string gzipInputString)
        {
            try
            {
                byte[] compressed = Convert.FromBase64String(gzipInputString);

                MemoryStream inputStream = new MemoryStream(compressed);
                MemoryStream outputStream = new MemoryStream(10024);
                using (var csStream = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(inputStream))
                {
                    byte[] buffer = new byte[10024];
                    int nRead;
                    while ((nRead = csStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outputStream.Write(buffer, 0, nRead);
                    }
                    return Convert.ToBase64String(outputStream.ToArray());
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error occured: " + e.Message);
                return null;
            }

            return null;

        }
        public static string CompressByteToGzipBase64(byte[] input)
        {
            try
            {
                var outputStream = new MemoryStream();
                var csStream = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(outputStream);
                csStream.Write(input, 0, input.Length);
                csStream.Close();
                string base64Image = Convert.ToBase64String(outputStream.ToArray());
                outputStream.Close();
                csStream = null;
                outputStream = null;

                return base64Image;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error occured: " + e.Message);
                return null;
            }
            return null;
        }

        public static string CompressBase64ToGzipBase64(String fromBase64Image)
        {
            try
            {
                var outputStream = new MemoryStream();
                byte[] input = Convert.FromBase64String(fromBase64Image);

                var csStream = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(outputStream);
                csStream.Write(input, 0, input.Length);
                csStream.Close();
                string base64Image = Convert.ToBase64String(outputStream.ToArray());

                outputStream.Close();
                csStream = null;
                outputStream = null;

                return base64Image;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error occured: " + e.Message);
                return null;
            }
            return null;
        }


        public static byte[] Decompress(byte[] input)
        {
            using (var source = new MemoryStream(input))
            {
                byte[] lengthBytes = new byte[4];
                source.Read(lengthBytes, 0, 4);

                var length = BitConverter.ToInt32(lengthBytes, 0);
                using (var decompressionStream = new GZipStream(source,
                    CompressionMode.Decompress))
                {
                    System.Diagnostics.Debug.WriteLine("I am here.. 2");
                    var result = new byte[length];
                    decompressionStream.Read(result, 0, length);
                    System.Diagnostics.Debug.WriteLine("I am here.. 10");
                    System.Diagnostics.Debug.WriteLine("DEcomp " + Encoding.UTF8.GetString(result));
                    return result;
                }
            }
        }

        public static byte[] Compress(byte[] input)
        {
            using (var result = new MemoryStream())
            {
                var lengthBytes = BitConverter.GetBytes(input.Length);
                result.Write(lengthBytes, 0, 4);

                using (var compressionStream = new GZipStream(result,
                    CompressionMode.Compress))
                {
                    compressionStream.Write(input, 0, input.Length);
                    compressionStream.Flush();

                }
                return result.ToArray();
            }
        }
    }
}