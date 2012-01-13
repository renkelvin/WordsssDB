using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace RootafixParser
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("rootafix.xml");
            

            FileStream fs = new FileStream("reault.txt", FileMode.Create);
            FileStream fs2 = new FileStream("result2.txt", FileMode.Create);
            StreamWriter writer2 = new StreamWriter(fs2);
            StreamWriter writer = new StreamWriter(fs);
            XmlNode wordDocument = doc.ChildNodes[2];
            Console.WriteLine(wordDocument.Name);
            string uri = wordDocument.NamespaceURI;

            XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
            manager.AddNamespace("w", uri);
            manager.AddNamespace("wsp", "http://schemas.microsoft.com/office/word/2003/wordml/sp2");
            manager.AddNamespace("wx", "http://schemas.microsoft.com/office/word/2003/auxHint");
            XmlNode wordSect = wordDocument.SelectSingleNode("w:body/wx:sect",manager);
            Console.WriteLine(wordSect.ChildNodes.Count);
            XmlNodeList pList = wordSect.SelectNodes("w:p",manager);
            Regex pattern = new Regex("^[0-9]");
            Regex pattern2 = new Regex("-[a-z，\u0020)(=]+|[a-z-)(，\u0020=]+");
            Regex patternWordRoot = new Regex("^a-zA-Z]");
            bool hasRootafixMeaning = false;
           
            foreach (XmlNode pNode in pList)
            {
                if (pattern.IsMatch(pNode.InnerText))
                {
                    writer.WriteLine(pNode.InnerText);
                    int i = 0;
                    foreach (Match m in pattern2.Matches(pNode.InnerText))
                    {
                        if (m.ToString() != "，" && m.Value != " " && m.Value != "(" && m.Value != ")")
                        {
                            writer.WriteLine(m.ToString());
                            if (i++ == 0 && pNode.InnerText.Length != m.Index + m.Length)
                            {
                                writer.WriteLine(pNode.InnerText.Substring(m.Index + m.Length));
                                hasRootafixMeaning = true;
                                break;
                            }
                            else if (m.Index == m.Length)
                            {
                                hasRootafixMeaning = false;
                            }

                            writer.WriteLine(m.Index);
                        }
                    }
                }
                else if (!hasRootafixMeaning && !patternWordRoot.IsMatch(pNode.InnerText)) 
                {
                    writer.WriteLine(pNode.InnerText);
                    }
                writer2.WriteLine(pNode.InnerText);
            }
            writer.Close();
            writer2.Close();
        }
    }
}
