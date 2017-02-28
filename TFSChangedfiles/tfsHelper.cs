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
            //connect to TFS
            TfsTeamProjectCollection tfs = null;
            tfs = new TfsTeamProjectCollection(new Uri("###ADD URI TO YOU TFS PROJECT COLLECTION HERE###"));
            VersionControlServer vcs = tfs.GetService<VersionControlServer>();
            try {
                var changesetItems = vcs.QueryHistory(localPath, VersionSpec.ParseSingleSpec(versionFromString, null), 0, RecursionType.Full, null, VersionSpec.ParseSingleSpec(versionFromString, null), null, Int32.MaxValue, true, false);
                
                foreach (Changeset csItem in changesetItems) {
                    ProcessChangeSet(csItem, args[2]);                        
                }
                var writeOutput = new WriteOutput();
                writeOutput.Init(resultsFile, Tasks);
                writeOutput.WriteOutTasksWithChangesets();                
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
                taskName = GetTaskNameFromChangeSet(csItem);
            }
            var task = FindOrCreateTask(taskName, ref isNewTask);
            
            ChangeSet cSet = new ChangeSet();
            cSet.Init(csItem.ChangesetId, csItem.CreationDate, csItem.Committer);
                       
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

        private TaskInfo FindOrCreateTask(string taskName, ref bool isNewTask)
        {
            TaskInfo task = Tasks.Find(t => String.Equals(t.ID, taskName));
            if (task == null)
            {
                task = new TaskInfo();
                task.ID = taskName;
                isNewTask = true;
            }
            return task;
        }
        private string GetTaskNameFromChangeSet(Changeset csItem)
        {
            return csItem.AssociatedWorkItems[0].Id + " " + csItem.AssociatedWorkItems[0].WorkItemType;
        }

    }
}
