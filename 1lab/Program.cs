using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1lab
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //FileCreator.CreateFile();
            //FileDivider fileDivider = new FileDivider();
            //fileDivider.ProcessUnsortedFile();
            FileMerger merger = FileMerger.Create();
            merger.Start();
        }
    }
}
