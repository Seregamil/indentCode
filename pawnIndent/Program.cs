using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

/*

http://pawno.su/showthread.php?t=106474
// -- Total lines. Before: 1012, After: 975
// -- Elapsed time: 00:00:00.0310409
 * 
 * 
 * 
http://pawno.su/showthread.php?t=112123
// -- Total lines. Before: 60, After: 58
// -- Elapsed time: 00:00:00.0060103
 */

namespace pawnIndent
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[0] == string.Empty)
                return;

            string path = args[0];
            string newPath = args[0] + ".pwn";

            List<string> fileData = File.ReadAllLines(path, Encoding.GetEncoding("windows-1251")).ToList(); // get file data
            List<string> data = pawn.transform(fileData);

            using (StreamWriter sWrite = new StreamWriter(newPath, false, Encoding.GetEncoding("windows-1251")))
            {
                foreach (string line in data)
                {
                    sWrite.WriteLine(line);
                }
            }

            Console.WriteLine("Job done!");
            System.Diagnostics.Process.Start(newPath);
        }
    }
}
/*
 

makeMyWork(")");

makeMyWork() {
	{ {} {} {{{   }}} }
}

lol(userid){sendclientmessage('lo{l}ka');}else{ lol{if(lolka){ }} }

//makeMyWork(")");

/*makeMyWork() {
	{ {} {} {{{   }}} }
}* /

lol(userid){sendclientmessage("lo{l'(()}ka");}else{ lol{if(lolka){ }} }
 */