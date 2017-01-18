using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Client;


namespace TFSChangedfiles
{
    class Program
    {
        static void Main(string[] args)
        {
            //add args
            //  0: working directory
            //  1: Version? 
            //  2: full username
            //  3: Is HPAS
            //  4: Output file
            tfsHelper TFS = new tfsHelper();
            if (args.Length == 0)
            {
                Console.WriteLine("no args using defaults");
				//init the helper with defaults if want to use defaults
            }
            else {
                Console.WriteLine(args[0]);
                Console.WriteLine(args[1]);
                Console.WriteLine(args[2]);
                Console.WriteLine(args[3]);
                Console.WriteLine(args[4]);
                TFS.Init(args);
            }
        }
    }
}
