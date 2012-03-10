using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using WordsssDB;

namespace GREParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Regex matchWord = new Regex("^\\s([a-z]+)\\s*$");
            //Regex matchWord = new Regex("^ ");
            
            Regex matchMeaning = new Regex("^[^\\s\\[]\\s*");
            Regex matchMemory = new Regex("^\\[(.*)\\](.*)\\s*$");

            FileStream fs = new FileStream("GRE.txt", FileMode.Open);
            StreamReader reader = new StreamReader(fs, Encoding.UTF8);

            FileStream fout = new FileStream("result.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(fout);

            int pos = 0;

            WordsssDBManager manager = new WordsssDBManager();
            string word_name = "";
            string word_meaning = "";
            int gre_list_word_id = -1;
            int count = 0;
            bool begin = true;
            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();
                if (pos == 0 && matchWord.IsMatch(strLine))
                {
                    word_name = matchWord.Match(strLine).Groups[1].Value;
                    writer.WriteLine("word==>" + word_name);
                    pos ++;
                }
                else if (pos == 1 && matchMeaning.IsMatch(strLine))
                {
                    word_meaning = strLine;
                    writer.WriteLine("meaning==>" + strLine);
                    pos++;
                    if(begin)
                    gre_list_word_id = manager.addGREListWord(word_name, word_meaning);
                    if (count++ % 100 == 0)
                        Console.WriteLine(count);
                }
                else if (pos == 2 && matchMemory.IsMatch(strLine))
                {
                    string type;
                    string description;
                    if (word_name == "capitulate")
                        begin = true;
                    type = matchMemory.Match(strLine).Groups[1].Value;
                    description = matchMemory.Match(strLine).Groups[2].Value;
                    writer.WriteLine("["+type+"]"+description);
                    if(begin)
                    manager.addGREListMemory(type, description, gre_list_word_id);
                }
                else if (pos == 2 && strLine == "")
                {
                    pos = 0;
                }

            }
            manager.CloseManager();
            writer.Close();
        }
    }
}
