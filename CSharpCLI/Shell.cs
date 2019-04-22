using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace CSharpCLI
{
    public class Shell
    {
        public bool Running { get; private set; }

        protected List<string> Buffer;

        public Shell()
        {
            Buffer = new List<string>();
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
            if (line == "?q")
            {
                Running = false;
                return;
            }

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

        protected void RunScript(string script, params string[] imports)
        {
            try
            {
                ScriptOptions options = ScriptOptions.Default;
                options = options.WithReferences("System");
                options = options.AddImports(imports);

                var result = CSharpScript.EvaluateAsync(script, options);
                if (result?.Result != null)
                    Console.WriteLine(result.Result.ToString());
            }
            catch (CompilationErrorException ex)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(ex.Message + ": " + ex.StackTrace);
                Console.ResetColor();
            }
        }
    }
}
