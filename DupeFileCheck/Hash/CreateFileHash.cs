using System;
using System.Collections.Generic;
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

        private static string GetMd5Hash(MD5 md5Hash, FileStream input)
        {
            byte[] data = md5Hash.ComputeHash(input);
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
