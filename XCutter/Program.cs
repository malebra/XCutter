using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace XCutter
{
    class Program
    {

        
        static void Main(string[] args)
        {
            Console.WriteLine("Program Started:");
            if (args.Length == 2)
            {
                Console.WriteLine($"Source Folder: {args[0]}");
                Console.WriteLine($"Destination Folder: {args[1]}"); 
            }
            Copier cp = new Copier(args);
            cp.Run();
            while (Console.ReadLine().ToLower() != "exit")
            {

            }
            cp = null;
            GC.Collect();


        }

        private void CopyFiles(object sender, EventArgs e)
        {
            
        }
    }
}
