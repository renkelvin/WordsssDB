using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using WordsssDB;

namespace SenseParser
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("GRE_sense.xml");
            XmlNode wordDocument = doc.ChildNodes[2];
            Console.WriteLine(wordDocument.Name);
            string uri = wordDocument.NamespaceURI;
           
            XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
            manager.AddNamespace("w",uri);
            manager.AddNamespace("wsp","http://schemas.microsoft.com/office/word/2003/wordml/sp2");
            XmlNodeList pList = wordDocument.SelectNodes("//w:p[@wsp:rsidR='00E60542']",manager);
            
            FileStream fs = new FileStream("sense2.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);
            Console.WriteLine(pList.Count);

            WordsssDBManager dbManager = new WordsssDBManager();
            int currentLevel = 0;
            Dictionary<int, int> senseParentDict = new Dictionary<int, int>();
            for (int i = 0; i < 6; i++)
            {
                senseParentDict.Add(i,0);
            }
                foreach (XmlNode node in pList)
                {
                    Regex pattern = new Regex("^[0-9.]+");
                    Regex pattern2 = new Regex("^[\u4e00-\u9fa5]");
                    Regex pattern3 = new Regex("^[a-z]+");
                    if (pattern.IsMatch(node.InnerText) || pattern2.IsMatch(node.InnerText))
                    {
                        string num = "";
                        string sense_name = "";
                        int splitCount = 0;
                        int senseId = 0;
                        if (pattern.IsMatch(node.InnerText))
                        {
                            num = pattern.Match(node.InnerText, 0).ToString();
                            splitCount = num.Split(new char[]{'.'},StringSplitOptions.RemoveEmptyEntries).Count();
                            sense_name = node.InnerText.Replace(num,"").Replace(" ","");
                        }
                        else
                        {
                            num = node.InnerText.Split(' ')[0];
                            sense_name = node.InnerText.Split(' ')[1];
                        }
                        while (currentLevel >= splitCount)
                        {
                            currentLevel--;
                        }
                        currentLevel++;
                    
                        if (currentLevel != 0)
                        {
                            int parentId = senseParentDict[currentLevel-1];
                            senseId = dbManager.addSense(sense_name, parentId);
                            writer.WriteLine(sense_name + "--->" + senseId + "," + parentId);
                            // add sense_name.Peek() as parent into current num
                        }
                        else
                        {
                            senseId = dbManager.addSense(sense_name);
                            writer.WriteLine(sense_name + "--->" + senseId);
                        }
                        senseParentDict[currentLevel] = senseId;
                    }
                    else if(pattern3.IsMatch(node.InnerText))
                    {
                        string word_name = pattern3.Match(node.InnerText).ToString();
                        string word_meaning = node.InnerText.Replace(word_name, "").Remove(0,1);
                        int senseId = senseParentDict[currentLevel];
                        dbManager.addWordSense(word_name,senseId,word_meaning);
                        writer.WriteLine(word_name + "--->" + senseId);
                        writer.WriteLine(word_meaning);
                    }
                }
            dbManager.CloseManager();
            writer.Close();
        }
    }
}
