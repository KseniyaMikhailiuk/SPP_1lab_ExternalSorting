using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1lab
{
    public static class FileCreator
    {
        
        public static void CreateFile()
        {
            using (var fileStream = new FileStream("myFile.txt", FileMode.OpenOrCreate, FileAccess.Write))
            {
                for (int i = 0; i <= 90000; i++)
                {
                    byte[] generatedString = Encoding.ASCII.GetBytes(GetRandomString(10000));
                    fileStream.Write(generatedString, 0, generatedString.Length);
                }
            }
        }

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static string GetRandomString(int size)
        {
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, size)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

}
