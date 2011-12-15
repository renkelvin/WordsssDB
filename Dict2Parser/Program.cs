using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using WordsssDB;
using System.IO;

namespace Dict2Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            int MAX_WORD_COUNT = 140000;
            int BEGIN_WORD = 123472;//未更新
            XmlDocument doc = new XmlDocument();
            doc.Load("1#900.da3");
            FileStream fs = new FileStream("mcec_dict.txt", FileMode.Append);
            StreamWriter writer = new StreamWriter(fs);

            WordsssDBManager manager = new WordsssDBManager();

            XmlNode dictNode = doc.ChildNodes[1];

            for (int i = BEGIN_WORD; i < (BEGIN_WORD + MAX_WORD_COUNT) && i <dictNode.ChildNodes.Count; i++)
            {
                string word_name;
                string word_paraphase;
                string word_type;
                XmlNode ckNode = dictNode.ChildNodes[i];
                XmlNode dcNode = ckNode.SelectSingleNode("DC");
                if (dcNode == null)
                    continue;
                
                word_name = dcNode.FirstChild.Value;
                word_name = word_name.Replace("'", "''");
                XmlNode dxNode = ckNode.SelectSingleNode("JS/CY/CX/DX");
                if (dxNode != null)
                {
                    word_type = dxNode.FirstChild.Value;
                }
                else
                    word_type = "";
                
                XmlNodeList jxNodes = ckNode.SelectNodes("JS/CY/CX/JX");
                writer.WriteLine(word_name);
                if (jxNodes != null)
                {
                    foreach (XmlNode jx in jxNodes)
                    {
                        word_paraphase = jx.FirstChild.Value;
                        word_paraphase = word_paraphase.Replace("'", "''");
                        if(manager.addParaphase("mcec_dict", word_name, word_paraphase, word_type)==-1)
                            writer.WriteLine(i + "==\n" + word_paraphase);
                        if( i % 1000 == 0)
                            Console.WriteLine("  " + i);
                    }
                }
                //writer.WriteLine(doc.ChildNodes[1].FirstChild.SelectSingleNode("//JX").FirstChild.Value);
            }
            writer.Close();
        }
    }
}
