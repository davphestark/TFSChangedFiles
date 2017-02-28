using System;
using System.Collections.Generic;
using System.IO;

namespace TFSChangedfiles
{
    public class WriteOutput
    {
        public int fileCount { get; set; }
        public string resultsFile { get; set; }
        public List<TaskInfo> Tasks { get; set; }

        public WriteOutput()
        {
            Tasks = new List<TaskInfo>();
        }
        public void Init(string fileName, List<TaskInfo> tasks)
        {
            fileCount = 0;
            resultsFile = fileName;
            Tasks = tasks;
        }
        public void WriteOutTasksWithChangesets()
        {
            StreamWriter oWrite = new StreamWriter(new FileStream(resultsFile, FileMode.OpenOrCreate, FileAccess.Write));
            foreach (TaskInfo task in Tasks)
            {
                WriteTask(task, oWrite);                
            }
            if (fileCount > 0)
            {
                WriteFileSummery(oWrite, "Total Files: ", fileCount);
                WriteOutFilesPerTask(oWrite);
            }
            oWrite.Close();
        }

        private void WriteTask(TaskInfo task, StreamWriter sw)
        {
            sw.WriteLine(task.ID);
            foreach (ChangeSet cs in task.changeSet)
            {
                WriteChangeSet(cs, sw);                
            }
        }

        private void WriteChangeSet(ChangeSet cs, StreamWriter sw)
        {
            sw.WriteLine("     ChangeSet:" + cs.ID + " by: " + cs.user + " on:" + cs.checkIn.ToString("MM/dd/yyyy HH:mm"));
            foreach (string file in cs.fileNames)
            {
                WriteFileAndIncrement(file, sw);                
            }
        }

        private void WriteFileAndIncrement(string file, StreamWriter sw)
        {
            sw.WriteLine("       " + file);
            fileCount++;
        }

        private void WriteFileSummery(StreamWriter sw, string desc, int count)
        {
            sw.WriteLine("--------------------------");
            sw.WriteLine("Total Files: " + fileCount);
            sw.WriteLine("");
        }
        private void WriteOutFilesPerTask(StreamWriter oWrite)
        {
            foreach (TaskInfo task in Tasks)
            {
                oWrite.WriteLine(task.ID + ":");
                oWrite.WriteLine("");
                List<string> files = new List<string>();
                BuildFileListForTask(task.changeSet, files);
                
                foreach (string file in files)
                {
                    oWrite.WriteLine(file);
                }
                WriteFileSummery(oWrite, "Files: ", files.Count);
            }
        }

        private void BuildFileListForTask(List<ChangeSet> changeSet, List<string> files)
        {
            foreach (ChangeSet cs in changeSet)
            {
                foreach (string file in cs.fileNames)
                {
                    AddUniqueFiles(file, files);
                }
            }
            files.Sort();
        }

        private void AddUniqueFiles(string file, List<string> files)
        {
            string filename = files.Find(f => String.Equals(f, file));
            if (filename == null)
            {
                files.Add(file);
            }
        }
    }
    
}
