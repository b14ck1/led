using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Utility
{
    public static class Logger
    {
        public enum LogClass
        {
            Debug,
            Warning,
            Error
        }

        public static bool Debug { get; set; }

        public static bool Warning { get; set; }

        public static bool Error { get; set; }

        public static void Log(object o, string message)
        {
            Console.WriteLine("{0}: {1}", o.ToString(), message);

            //Exception e = new Exception();

            //e.StackTrace;
        }
    }
}
