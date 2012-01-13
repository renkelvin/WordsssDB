using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace DerivationParser
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("AHD.xml");
            FileStream fs = new FileStream("result.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);

            XmlNode dictNode = doc.ChildNodes[1];

            int MAX_WORD = 100000;

            WordsssDB.WordsssDBManager manager = new WordsssDB.WordsssDBManager();

            for (int i = 0; i < MAX_WORD && i < dictNode.ChildNodes.Count; i ++ )
            {
                string word_name = "";
                XmlNode ckNode = dictNode.ChildNodes[i];
                XmlNode dcNode = ckNode.SelectSingleNode("单词");
                if (dcNode != null)
                    word_name = dcNode.FirstChild.Value;
                word_name = word_name.Replace("'", "''");
                XmlNode jcNode = ckNode.SelectSingleNode("单词解释块/继承用法");


                if (jcNode != null)
                {
                    XmlNodeList jcList = jcNode.SelectNodes("单词项/单词原型");
                    if (jcList.Count != 0)
                    {
                        writer.WriteLine(word_name + "  " + jcList.Count);
                        foreach (XmlNode jc in jcList)
                        {
                            string str = jc.FirstChild.Value.Replace("&2{”}", "");
                            str = str.Replace("&2{“}", "");
                            str = str.Replace(" 或", "");
                            str = str.Replace("'", "''");
                            writer.WriteLine("  " +str);
                            if (manager.addDerivation(word_name, str) == -1)
                            {
                                writer.WriteLine("FAILED");
                            }
                            
                        }
                        
                    }
                }
                if(i % 1000 == 0)
                {
                    Console.WriteLine(i);
                }
            }
        }
    }
}
