using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Pipes
{
    public class PipesDeom
    {
        public void Test()
        {
            Process process = new Process();
            process.StartInfo.FileName = "child.exe";
            using (AnonymousPipeServerStream anonymousPipe = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
            {
                process.StartInfo.Arguments = anonymousPipe.GetClientHandleAsString();
                process.StartInfo.UseShellExecute = false;
                process.Start();


            } 
        }
    }
}
