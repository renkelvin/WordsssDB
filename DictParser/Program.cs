using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using WordsssDB;

namespace DictParser
{
    class Program
    {
        static void Main(string[] args)
        {
            WordsssDBManager myDbManager = new WordsssDBManager();
            var result = myDbManager.getAllWord();

            StreamReader reader = new StreamReader("oxford.txt");
            while (!reader.EndOfStream)
            {
                string s = reader.ReadLine();
                string[] split = s.Split(new char[]{' '},2);
                string word_name = split[0];
                string paraphase = split[1];
                //myDbManager.addParaphase(word_name, 1, paraphase);
                Console.WriteLine("{0} {1}", word_name, paraphase);
            }
            
            foreach (var word in result)
            {
                Console.WriteLine(word);
            }
            myDbManager.CloseManager();
        }
    }
}
