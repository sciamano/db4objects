using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Db4oTool;

namespace Db4oTool.MSBuild
{
    public class Db4oEnhancerMSBuildTask : Task
    {
        private ITaskItem[] assemblies;

        [Required]
        public ITaskItem[] Assemblies
        {
            get { return assemblies; }
            set { assemblies = value; }
        }

        private ITaskItem projectDir;

        public ITaskItem ProjectDir
        {
            get { return projectDir; }
            set { projectDir = value; }
        }

        private string commandLine;

        public string CommandLine
        {
            get { return commandLine; }
            set { commandLine = value; }
        }

        public override bool Execute()
        {
            List<string> list = new List<string>();
            list.Add("-v");
			list.Add("-ta");
            if (commandLine != null)
            {
                list.Add(commandLine);
            }
            foreach (ITaskItem assembly in assemblies)
            {
                string assemblyFile = projectDir + assembly.ItemSpec;
                Log.LogWarning(string.Format("Enhancing assembly: {0}", assemblyFile));
                list.Add(assemblyFile);

                int ret = Enhance(list.ToArray());
                if (ret != 0)
                {
                	Log.LogError(string.Format("Fail to enhance assembly: {0} with return value {1}", assemblyFile, ret));
                    return false;
                }
                string message = string.Format("Assembly {0} is enhanced successfully.", assemblyFile);
                Log.LogWarning(message);

                list.Remove(assemblyFile);
            }
            return true;
        }

        private int Enhance(string[] options)
        {
        	int ret;
			StringWriter consoleOut = new StringWriter();
			try
			{
				Console.SetOut(consoleOut);

				ret = Program.Main(options);
				if (ret != 0)
				{
					Log.LogError(consoleOut.ToString());
				}
			}
			finally
			{
				StreamWriter originalOut = new StreamWriter(Console.OpenStandardOutput());
				originalOut.AutoFlush = true;
				Console.SetOut(originalOut);
			}

        	return ret;
        }
    }
}
