using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1lab
{
    class FileMerger
    {
        public struct QueueandFileLink
        {
            public Queue<byte> queue;
            public FileStream file;
        }
        private List<FileStream> FileStreams = null;
        private List<QueueandFileLink> FilesQueues = null;
        private FileStream SortedFile = null;
        private int AmountofUnsortedFiles = 0;
        private int MaxBufferSize = 5 * 1024 * 1024, MaxTotalQueueSize = 100 * 1024 * 1024, MaxQueueSize;

        public static FileMerger Create()
        {
            return new FileMerger();
        }

        public void Start()
        {
            InitTools();
            while (AmountofUnsortedFiles > 0 || FilesQueues.Count > 0)
            {
                byte[] sortedData = GetSortedArray();
                SortedFile.Write(sortedData, 0, sortedData.Length);
            }
            Dispose();
        }
        
        private void InitTools()
        {
            try
            {
                SortedFile = new FileStream("SortedFile.txt", FileMode.Create);
                string[] fileNames = Directory.GetFiles("files", "*.txt")
                             .Select(Path.GetFileName)
                             .ToArray();
                AmountofUnsortedFiles = fileNames.Length;
                MaxQueueSize = MaxTotalQueueSize / AmountofUnsortedFiles;
                FileStreams = new List<FileStream>();
                FilesQueues = new List<QueueandFileLink>();
                for (int i = 0; i < fileNames.Length; i++)
                {
                    FileStreams.Add(new FileStream("files/" + fileNames[i], FileMode.Open));
                }
                ReadFromFilesIntoQueues();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void ReadFromFilesIntoQueues()
        {
            foreach (var stream in FileStreams)
            {
                ReplenishQueue(stream);
            }
        }

        private void ReplenishQueue(FileStream stream)
        {
            if (stream.Position < stream.Length)
            {
                long leftBytesinFile;
                byte[] readData;
                if ((leftBytesinFile = stream.Length - stream.Position) < MaxQueueSize)
                {
                    readData = new byte[leftBytesinFile];
                }
                else
                {
                    readData = new byte[MaxQueueSize];
                }
                stream.Read(readData, 0, readData.Length);
                FilesQueues.Add(new QueueandFileLink { queue = new Queue<byte>(readData), file = stream });
            }
            else
            {
                AmountofUnsortedFiles--;
            }
        }

        private byte[] GetSortedArray()
        {
            List<byte> sortedList = new List<byte>();
            while (FilesQueues.Count > 0)
            {
                QueueandFileLink queueWithMinElement = FilesQueues.First();
                byte minValue = byte.MaxValue;
                byte temp = 0;
                foreach (var queue in FilesQueues)
                {
                    if ((temp = queue.queue.Peek()) < minValue)
                    {
                        queueWithMinElement = queue;
                        minValue = temp;
                    }
                }
                sortedList.Add(queueWithMinElement.queue.Dequeue());
                if (queueWithMinElement.queue.Count == 0)
                {
                    ReplenishQueue(queueWithMinElement.file);
                    FilesQueues.Remove(queueWithMinElement);                  
                }
                if(sortedList.Count > MaxBufferSize)
                {
                    break;
                }
            }
            return sortedList.ToArray();
        }
        private void Dispose()
        {
            foreach (var file in FileStreams)
            {
                if (file != null)
                {
                    file.Dispose();
                    file.Close();
                }
            }
            if (SortedFile != null)
            {
                SortedFile.Dispose();
                SortedFile.Close();
            }
        }
    }
}
