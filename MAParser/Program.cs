using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using WordsssDB;
using System.IO;
using System.Text.RegularExpressions;

namespace MAParser
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("ma.xml");

            XmlNode dictNode = doc.ChildNodes[1];
            Console.WriteLine(dictNode.ChildNodes.Count);

            XmlNodeList wlWordList = dictNode.ChildNodes;

            FileStream fs = new FileStream("result.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);

            int count = 0;

            WordsssDBManager manager = new WordsssDBManager();

            foreach (XmlNode wordNode in wlWordList)
            {
                string word_name;
                string word_meaning;
                word_name = wordNode.SelectSingleNode("单词").FirstChild.Value;
                word_meaning = wordNode.SelectSingleNode("单词解释块/基本词义/单词项/解释项").FirstChild.Value;
                Regex matchWord = new Regex("^[a-z]+$");
                if (matchWord.IsMatch(word_name))
                {
                    manager.addMaListWord(word_name, word_meaning);
                    writer.WriteLine(word_name);
                    writer.WriteLine(word_meaning);
                    count++;
                    if (count % 100 == 0)
                        Console.WriteLine(count);
                }
            }


            writer.WriteLine(count);
            writer.Close();
            manager.CloseManager();
        }
    }
}
