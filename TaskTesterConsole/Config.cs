using System;
using System.Collections.Generic;
using System.Text;

namespace TaskTesterConsole
{
    public class Config
    {
        public bool SkipWelcome  { get; set; }
        public string Source { get; set; }
        
        public string TestInputName  { get; set; }
        public string TestInputType { get; set; }

        public string TestOutputName { get; set; }
        public string TestOutputType { get; set; }

        public string OutputFile { get; set; }
        public string InputFile { get; set; }


        public int CountOfTests { get; set; }
        public int TimeLimit { get; set; }


        
        public string CPPCompiler { get; set; }
        public string PascalCompiler { get; set; }
    }
}
