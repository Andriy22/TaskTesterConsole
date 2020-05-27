using System;
using System.Collections.Generic;
using System.Text;

namespace TaskTesterConsole
{
    public class Config
    {
        public string Source { get; set; }
        public string TestName  { get; set; }
        public string TestType { get; set; }
        public int CountOfTests { get; set; }
        public int TimeLimit { get; set; }
        public string CPPCompiler { get; set; }
        public string PascalCompiler { get; set; }
    }
}
