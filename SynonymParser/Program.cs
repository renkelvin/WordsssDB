using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace SynonymParser
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("PWREE.da3");

            XmlNode dict = doc.ChildNodes[1];

            XmlNodeList dcList = dict.SelectNodes("单词块");

            FileStream fs = new FileStream("result.txt",FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);

            WordsssDB.WordsssDBManager manager = new WordsssDB.WordsssDBManager();

            int i = 0;
            foreach (XmlNode dcNode in dcList)
            {
                XmlNode xgNode = dcNode.SelectSingleNode("单词解释块/基本词义/单词项/相关词");
      //          if(xgNode != null)
                Regex patter1 = new Regex("l{[a-z.A-Z]+}");
                MatchCollection matches = patter1.Matches(xgNode.InnerText);

                XmlNode wordNode = dcNode.SelectSingleNode("单词");

                string base_word = wordNode.InnerText;
                writer.WriteLine(wordNode.InnerText);
                if(matches.Count != 0)
                    writer.WriteLine("<1>");
                foreach(Match word in matches)
                {
                    string rep = word.ToString().Replace("l{", "");
                    rep = rep.Replace("}", "");
                    writer.WriteLine("  " +　rep);
                    if (manager.addAntonym(base_word, rep) == -1)
                    {
                        writer.WriteLine("FAILED");
                    }
                }

                Regex pattern2 = new Regex("L{[a-z.A-Z]+}");
                MatchCollection matches2 = pattern2.Matches(xgNode.InnerText);
                if(matches2.Count != 0)
                    writer.WriteLine("<2>");
                foreach (Match word in matches2)
                {
                    string rep = word.ToString().Replace("L{", "");
                    rep = rep.Replace("}", "");
                    writer.WriteLine("  " + rep);
                    if (manager.addSynonym(base_word, rep) == -1)
                    {
                        writer.WriteLine("FAILED");
                    }
                }
                if(i++ % 100 == 0)
                Console.WriteLine(i);
            }
            writer.Close();
        }
    }
}
