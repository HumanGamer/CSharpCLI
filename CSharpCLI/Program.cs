﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpCLI
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            Shell shell = new Shell();
            shell.Run();
        }
    }
}
