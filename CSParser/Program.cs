using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using WordsssDB;
using System.IO;
using System.Text.RegularExpressions;

namespace CSParser
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("cs.xml");

            XmlNode dictNode = doc.ChildNodes[1];
            Console.WriteLine(dictNode.ChildNodes.Count);

            XmlNodeList csWordList = dictNode.ChildNodes;

            FileStream fs = new FileStream("result.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);

            int count = 0;

            WordsssDBManager manager = new WordsssDBManager();

            foreach (XmlNode csWordNode in csWordList)
            {
                XmlNode word_name = csWordNode.SelectSingleNode("单词");
                XmlNode word_meaning = csWordNode.SelectSingleNode("单词解释块/基本词义/单词项/解释项");
                Regex pattern = new Regex("^[a-z]+$");
                Regex pattern2 = new Regex(",[ ]*[A-Z]+$");
                if (pattern.IsMatch(word_name.FirstChild.Value))
                {
                    string strName = word_name.FirstChild.Value;
                    string strMeaning = word_meaning.FirstChild.Value;
                  
                    manager.addCsListWord(strName, "", strMeaning);

                    writer.WriteLine(word_name.FirstChild.Value);
                    writer.WriteLine(word_meaning.FirstChild.Value);
                    count++;
                    if (count % 100 == 0)
                    {
                        Console.WriteLine(count);
                    }
                }
                else if (pattern2.IsMatch(word_name.FirstChild.Value))
                {
                    string strName = word_name.FirstChild.Value;
                    Regex patterShort = new Regex("[A-Z]+$");
                    string shortName = patterShort.Match(strName).Value;
                    string[] strSplit = strName.Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries);
                    string fullName = strSplit[0];
                    shortName = shortName.Replace(" ", "");
                    fullName = fullName.Replace("'", "''");
                    manager.addCsListWord(shortName, fullName, word_meaning.FirstChild.Value);
                    writer.WriteLine("-"+shortName);
                    writer.WriteLine(fullName);
                    writer.WriteLine(word_meaning.FirstChild.Value);
                    count ++;
                    if (count % 100 == 0)
                    {
                        Console.WriteLine(count);
                    }
                }
            }
            writer.WriteLine(count);
            writer.Close();
            manager.CloseManager();
        }
    }
}
