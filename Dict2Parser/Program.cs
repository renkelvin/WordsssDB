using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using WordsssDB;

namespace Dict2Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            int MAX_WORD_COUNT = 2000;
            int BEGIN_WORD = 12543;//未添加
            XmlDocument doc = new XmlDocument();
            doc.Load("1#900.da3");

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
                if (jxNodes != null)
                {
                    foreach (XmlNode jx in jxNodes)
                    {
                        word_paraphase = jx.FirstChild.Value;
                        word_paraphase = word_paraphase.Replace("'", "''");
                        manager.addParaphase(word_name, 2, word_paraphase, word_type);
                        Console.WriteLine(i);
                    }
                }
                //writer.WriteLine(doc.ChildNodes[1].FirstChild.SelectSingleNode("//JX").FirstChild.Value);
            }
        }
    }
}
