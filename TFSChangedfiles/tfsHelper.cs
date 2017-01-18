using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.IO;

namespace TFSChangedfiles
{
    public class tfsHelper
    {
        List<TaskInfo> Tasks = new List<TaskInfo>();
        string startString = "";
        string resultsFile = "";

        public void Init(params string[] args)
        {
            string localPath = args[0];
            string versionFromString = args[1];
            resultsFile = args[4];
            bool isHPAS = Convert.ToBoolean(args[3]);
            
            if (isHPAS)
            {
                startString = "src/";
            }
            else {
                startString = "stingray/";
            }

            TfsTeamProjectCollection tfs = null;
            tfs = new TfsTeamProjectCollection(new Uri("###ADD URI TO YOU TFS PROJECT COLLECTION HERE###"));
            VersionControlServer vcs = tfs.GetService<VersionControlServer>();
            try {
                var changesetItems = vcs.QueryHistory(localPath, VersionSpec.ParseSingleSpec(versionFromString, null), 0, RecursionType.Full, null, VersionSpec.ParseSingleSpec(versionFromString, null), null, Int32.MaxValue, true, false);
                
                foreach (Changeset csItem in changesetItems) {
                    ProcessChangeSet(csItem, args[2]);                        
                }
                
                WriteOutInfo();
                
            }
            catch (Exception e) { 
                Console.WriteLine("Error: " +e.Message);
                Console.ReadKey();
            }

        }
        public void ProcessChangeSet(Changeset csItem, string user)
        {            
            string taskName = "No Associated Work Item";
            bool isNewTask = false;

            if (csItem.AssociatedWorkItems.Length > 0)
            {
                taskName = csItem.AssociatedWorkItems[0].Id + " " + csItem.AssociatedWorkItems[0].WorkItemType;
            }
            TaskInfo task = Tasks.Find(t => String.Equals(t.ID, taskName));
            if (task == null)
            {
                task = new TaskInfo();
                isNewTask = true;
                task.ID = taskName;
            }

            ChangeSet cSet = new ChangeSet();
            cSet.checkIn = csItem.CreationDate;
            cSet.ID = csItem.ChangesetId;
            cSet.user = csItem.Committer;
           
            if (String.Equals(user, cSet.user, StringComparison.CurrentCultureIgnoreCase))
            {
                foreach (Change changedItem in csItem.Changes)
                {
                    cSet.fileNames.Add(changedItem.Item.ServerItem.Substring(changedItem.Item.ServerItem.IndexOf(startString) + startString.Length));
                }
                task.changeSet.Add(cSet);
                if (isNewTask) { Tasks.Add(task); }
            }
        }
        public void WriteOutInfo()
        {
            int fileCount = 0;
            StreamWriter oWrite = new StreamWriter(new FileStream(resultsFile, FileMode.Create, FileAccess.Write));
            foreach (TaskInfo task in Tasks)
            {
                oWrite.WriteLine(task.ID);
                foreach (ChangeSet cs in task.changeSet)
                {
                    oWrite.WriteLine("     ChangeSet:" + cs.ID + " by: " + cs.user + " on:" + cs.checkIn.ToString("MM/dd/yyyy HH:mm"));
                    foreach (string file in cs.fileNames)
                    {
                        oWrite.WriteLine("       " + file);
                        fileCount++;
                    }
                }
            }
            if (fileCount > 0)
            {
                oWrite.WriteLine("--------------------------");
                oWrite.WriteLine("Total Files: " + fileCount);
                oWrite.WriteLine("");
                WriteFilesPerTask(oWrite);                
            }
            oWrite.Close();
        }
        public void WriteFilesPerTask(StreamWriter oWrite)
        {
            foreach (TaskInfo task in Tasks)
            {
                oWrite.WriteLine(task.ID + ":");
                oWrite.WriteLine("");
                List<string> files = new List<string>(); 
                foreach (ChangeSet cs in task.changeSet)
                {
                    foreach (string file in cs.fileNames)
                    {
                        string filename = files.Find(f => String.Equals(f, file));
                        if (filename == null)
                        {
                            files.Add(file);
                        }
                    }
                }
                files.Sort();
                foreach (string file in files)
                {
                    oWrite.WriteLine(file);
                }
                oWrite.WriteLine("--------------------------");
                oWrite.WriteLine("Files: " + files.Count);
                oWrite.WriteLine("");
            }
 
        }
    }
}
