using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace FrequencyParser
{
    public class Frequency
    {
        public int frequency1 { get; set; }
        public int frequency2 {get;set;}
        public double frequency3{get;set;}
        public Frequency(int f1, int f2, double f3)
        {
            frequency1 = f1;
            frequency2 = f2;
            frequency3 = f3;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            FileStream fs = new FileStream("1_1_all_fullalpha.txt",FileMode.Open);
            StreamReader reader = new StreamReader(fs);
            FileStream outFile = new FileStream("out2.txt",FileMode.Create);
            StreamWriter writer = new StreamWriter(outFile);

            WordsssDB.WordsssDBManager manager = new WordsssDB.WordsssDBManager();

            int i = 0;
            bool bAddWord = false;
            Regex pattern = new Regex("^[a-z]");
            HashSet<string> strHash = new HashSet<string>();
            Dictionary<string, Frequency> strDict = new Dictionary<string, Frequency>();
            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();
                string[] splitLine = strLine.Split(new char[]{'\t'},StringSplitOptions.RemoveEmptyEntries);
       
                int frequency = int.Parse(splitLine[3]);
                int frequency2 = int.Parse(splitLine[4]);
                double frequency3 = double.Parse(splitLine[5]);
                if (frequency == 0 && bAddWord == false)
                    continue;
                if (frequency == 0 && splitLine[2] == ":")
                {
                    bAddWord = false;
                    continue;
                }
                string str;
                if (splitLine[2] == "%" && frequency != 0)
                {
                    bAddWord = true;
                    continue;
                }
                else if (splitLine[2] == ":")
                {
                    bAddWord = false;
                }
                if (splitLine[0] == "@")
                    str = splitLine[2];
                else
                    str = splitLine[0];

                if (!pattern.IsMatch(str))
                    continue;

                if (i ++ > 0 )
                {
                    if (!strHash.Contains(str))
                    {
                        strHash.Add(str);
                        strDict.Add(str, new Frequency(frequency, frequency2, frequency3));
                    }
                    else
                    {
                        strDict[str].frequency1 += frequency;
                    }
                }
                //      Console.WriteLine(manager.addFrequency(current, currentFrequency, currentFrequency2, frequency3));
            }
            int j = 0;
            Console.WriteLine(strHash.Count);
            foreach (string str in strHash)
            {
                //int j = manager.addFrequency(str, strDict[str].frequency1, strDict[str].frequency2, strDict[str].frequency3);
                if (j  % 100 == 0)
                    Console.WriteLine(j);
                //if (str.Contains('\''))
                //{
                    string strRep = str.Replace("'", "''");
                    if (strDict[str].frequency1 == 0)
                    {    //  manager.addFrequency(strRep, strDict[str].frequency1, strDict[str].frequency2, strDict[str].frequency3);

                        if (manager.addFrequency(strRep, strDict[str].frequency1, strDict[str].frequency2, strDict[str].frequency3) == -1)
                        {
                            Console.WriteLine(str);
                            Console.Read();
                        }
                        writer.WriteLine(str + " " + strDict[str].frequency1 + "," + strDict[str].frequency2 + "," + strDict[str].frequency3);
                        //}
                        j++;
                    }
            }
            writer.WriteLine(i);
            writer.WriteLine(strHash.Count());
            writer.WriteLine(j);
            manager.CloseManager();
            writer.Close();
        }
    }
}
