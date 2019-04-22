using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace CSharpCLI
{
    internal class Shell
    {
        public bool Running { get; private set; }

        protected List<string> Buffer;

        internal ScriptGlobals Globals;

        protected Script Script;
        protected ScriptState State;

        public Shell()
        {
            Buffer = new List<string>();
            Globals = new ScriptGlobals(this);
        }

        public void Run()
        {
            Running = true;
            while (Running)
            {
                Console.Write("> ");

                string line = Console.ReadLine();
                ProcessLine(line);
            }
        }

        protected void ProcessLine(string line)
        {
            if (line.EndsWith("\\"))
            {
                Buffer.Add(line.Substring(0, line.Length - 1));
                return;
            }

            if (Buffer.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var ln in Buffer)
                {
                    sb.AppendLine(ln);
                }

                sb.Append(line);
                RunScript(sb.ToString());
                Buffer.Clear();
            }
            else
            {
                RunScript(line);
            }
        }

        protected void RunScriptFile(string file)
        {
            RunScript(File.ReadAllText(file));
        }

        protected void RunScript(string script)
        {
            RunScript(script, "System", "System.IO", "System.Linq", "System.Collections.Generic", "System.Text");
        }

        protected void RunScript(string code, params string[] imports)
        {
            try
            {
                ScriptOptions options = ScriptOptions.Default;
                options = options.WithReferences("System");
                options = options.WithReferences(Assembly.GetAssembly(this.GetType()));
                options = options.AddImports(imports);

                if (Script == null)
                    Script = CSharpScript.Create("", options, Globals.GetType());
                
                Script script = Script.ContinueWith(code, options);
                ScriptState state = State == null ? script.RunAsync(Globals, ExceptionHandler).Result : script.RunFromAsync(State, ExceptionHandler).Result;

                if (state.Exception != null)
                {
                    ExceptionHandler(state.Exception);
                    return;
                }

                if (state.ReturnValue != null)
                {
                    Console.WriteLine("Returned: " + state.ReturnValue);
                }

                Script = script;
                State = state;
            }
            catch (CompilationErrorException ex)
            {
                ExceptionHandler(ex);
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex);
            }
        }

        private bool ExceptionHandler(Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            if (ex.InnerException != null)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Caused by: " + ex.InnerException.Message + "\n" + ex.StackTrace);
            }

            Console.ResetColor();

            // Is Exception Caught?
            return false;
        }
    }
}
