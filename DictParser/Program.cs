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

            StreamReader reader = new StreamReader("test.txt");
            while (!reader.EndOfStream)
            {
                string s1 = reader.ReadLine();
                string paraphase = reader.ReadLine();
                string word_name = s1.Split(' ')[1];
                myDbManager.addParaphase(word_name, 0, paraphase);
            }
 
            foreach (var word in result)
            {
                Console.WriteLine(word);
            }
            myDbManager.CloseManager();
        }
    }
}
