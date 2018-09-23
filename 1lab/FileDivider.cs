using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace _1lab
{
    class FileDivider
    {
        public string UnsortedFileName { get; set; }
        public long UnsortedFileSize { get; set; }
        private const int MaxFileSize = 50 * 1024 * 1024;
        public void SelectFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true
            })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK && (UnsortedFileName = openFileDialog.FileName) != null)
                {
                    using (var chosenFile = openFileDialog.OpenFile())
                    {
                        UnsortedFileSize = chosenFile.Length;
                    }
                }
            }
        }



        public void ProcessUnsortedFile()
        {
            SelectFile();
            ReadBytes();
        }

        public void ReadBytes()
        {
            try
            {
                using (var fileStream = new FileStream(UnsortedFileName, FileMode.Open))
                {
                    int readBytes = 0;
                    int totalAmountReadBytes = 0;
                    int index = 0;
                    byte[] buffer;
                    do
                    {
                        buffer = new byte[MaxFileSize];
                        if ((readBytes = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            SortAndWriteIntoNewFile(buffer, index);
                        }
                        totalAmountReadBytes += readBytes;
                        index++;
                    } while (readBytes > 0 && totalAmountReadBytes + MaxFileSize < UnsortedFileSize);
                    buffer = new byte[UnsortedFileSize-totalAmountReadBytes];
                    readBytes = fileStream.Read(buffer, 0, buffer.Length);
                    SortAndWriteIntoNewFile(buffer, index);
                }
            }
            catch
            {
                MessageBox.Show("Select file", "File Error", MessageBoxButtons.OK);
                SelectFile();
            }
        }

        public void SortAndWriteIntoNewFile(byte[] readChunk, int fileIndex)
        {
            try
            {
                Array.Sort(readChunk);
                string fileName = fileIndex.ToString() + ".txt";
                using (var stream = File.Create("files/" + fileName))
                {
                    stream.Write(readChunk, 0, readChunk.Length);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }


        }
    }
}
