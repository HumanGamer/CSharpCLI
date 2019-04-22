using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpCLI
{
    public class ScriptGlobals
    {
        private readonly Shell _shell;

        internal ScriptGlobals(Shell shell)
        {
            _shell = shell;
        }

        // Global Methods and Variables Go Here
        public void Clear()
        {
            Console.Clear();
        }

        public void Print(object o)
        {
            Console.WriteLine(o);
        }

        public void Quit()
        {
            Environment.Exit(0);
        }

        public void Reset()
        {
            _shell.Reset();
        }
    }
}
