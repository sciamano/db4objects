using System;
using System.Diagnostics;
using System.IO;

namespace com.db4o.test
{
	class IOServices
	{
		public static string FindParentDirectory(string path)
		{
			string parent = Path.GetFullPath("..");
			while (true)
			{
				if (Directory.Exists(Path.Combine(parent, path))) return parent;
				string oldParent = parent;
				parent = Path.GetDirectoryName(parent);
				if (parent == oldParent || parent == null) break;
			}
			return null;
		}

		public static void WriteFile(String fname, String contents)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(fname));
			using (StreamWriter writer = new StreamWriter(fname))
			{
				writer.Write(contents);
			}
		}

		public static String Exec(String program, params String[] arguments)
		{
			ProcessStartInfo psi = new ProcessStartInfo(program);
			psi.UseShellExecute = false;
			psi.Arguments = string.Join(" ", arguments);
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError = true;
			psi.WorkingDirectory = Path.GetTempPath();
			psi.CreateNoWindow = true;

			Process p = Process.Start(psi);
			string stdout = p.StandardOutput.ReadToEnd();
			string stderr = p.StandardError.ReadToEnd();
			p.WaitForExit();
			return stdout + stderr;
		}

		public static String BuildTempPath(String fname)
		{
			return Path.Combine(Path.GetTempPath(), fname);
		}
	}
}
