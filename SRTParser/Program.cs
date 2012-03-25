using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SRTParser
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream fs = new FileStream("01.srt", FileMode.Open);
            StreamReader reader = new StreamReader(fs);

            SRT srt_parser = new SRT(2, 1);
            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();
                srt_parser.ParseLine(strLine);
            }

            reader.Close();
            fs.Close();
            srt_parser.Close();
        }
    }
}
