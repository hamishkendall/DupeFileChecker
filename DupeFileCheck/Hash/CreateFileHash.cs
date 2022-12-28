using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DupeFileCheck.DirHash
{
    public class CreateFileHash
    {
        public static string GetHash(string filePath)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(filePath))
                {
                    return GetMd5Hash(md5Hash, stream);
                }
            }
        }

        //Converts file to md5 hash
        private static string GetMd5Hash(MD5 md5Hash, FileStream file)
        {
            byte[] data = md5Hash.ComputeHash(file);
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                strBuilder.Append(data[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }
    }
}
