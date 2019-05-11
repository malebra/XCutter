using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Lame;
using System.Runtime.InteropServices;

namespace XCutter
{
    public class Copier
    {
        private FileSystemWatcher watcher = new FileSystemWatcher();

        string locationFrom;
        string locationTo;

        object locker = new Object();

        bool donNotRun = false;
            
        public Copier(string[] _args)
        {
            if (_args.Length == 2)
            {
                locationFrom = _args[0];
                locationTo = _args[1];
                if (((File.GetAttributes(locationFrom) & File.GetAttributes(locationTo) & FileAttributes.Directory) != FileAttributes.Directory) || locationTo == String.Empty || locationFrom == String.Empty)
                {
                    Console.WriteLine("One or both of the given locations isn't a folder.");
                    Console.Read();
                    return;
                }

                watcher = new FileSystemWatcher(locationFrom);
                watcher.EnableRaisingEvents = true;
                watcher.Changed += CopyFiles;
                watcher.Created += CopyFiles;
                watcher.Deleted += CopyFiles;
                watcher.Error += (a, b) => Console.WriteLine($"{b.GetException().Message}");
                

                foreach (var item in Directory.GetFiles(locationFrom))
                {
                    if (!File.Exists($"{locationTo}\\{Path.GetFileNameWithoutExtension(item)}.wav"))
                    {
                        CopyFiles(item);
                    }
                }

            }
            else
            {
                Console.WriteLine("The number of arguments is incorrect. Pass the folder locations correctly. ");
                Console.Read();
                //donNotRun = true;
            }
        }

        public void Run()
        {

        }

        private void CopyFiles(object sender, FileSystemEventArgs e)
        {
            bool x = true;
            while (x)
            {
                try
                {
                    using (Stream s = File.Open(e.FullPath, FileMode.Open))
                    {

                    }
                    x = false;
                }
                catch (Exception)
                {
                    Thread.Sleep(500);
                }
            }
            Thread.Sleep(250);
            lock (locker)
            {
                string name = Path.GetFileNameWithoutExtension(e.FullPath);
                string dest = $"{locationTo}\\{name}.wav";
                if (e.ChangeType == WatcherChangeTypes.Created)
                {
                    if (!File.Exists(dest))
                    {
                        Console.WriteLine($"Editing file {e.FullPath}...");
                        WaveFileUtils.TrimAudioFile(e.FullPath, dest, new TimeSpan(0, 0, 0, 5, 600), new TimeSpan(0, 0, 0, 6, 470));
                    }
                }
                if (e.ChangeType == WatcherChangeTypes.Deleted)
                {
                    if (File.Exists(dest))
                    {
                        try
                        {
                            Console.Write($"Editing file {dest}......");
                            File.Delete(dest);
                            Console.WriteLine("Done.");
                        }
                        catch (Exception ee)
                        {
                            Console.WriteLine($"Error: {ee.Message}");
                        }
                    }
                } 
            }
        }

        private void CopyFiles(string fullPath)
        {
            string name = Path.GetFileNameWithoutExtension(fullPath);
            string dest = $"{locationTo}\\{name}.wav";
            if (!File.Exists(dest))
            {
                WaveFileUtils.TrimAudioFile(fullPath, dest, new TimeSpan(0, 0, 0, 5, 600), new TimeSpan(0, 0, 0, 6, 470));
            }
        }
    }
}
