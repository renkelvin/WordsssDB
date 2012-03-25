using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace SoundMarkParser
{
    class Program
    {
        public static string processString(string str)
        {
            string result = str.Replace("&I{", "");
            result = result.Replace("}", "");
            result = result.Replace("&b{", "");
            result = result.Replace("'", "''");
            result = result.Replace("?", "");
            return result;
        }

        public static int getWordType(string wordType)
        {
            if (wordType.Contains("名词"))
                return 1;
            if (wordType.Contains("动词") && wordType.Contains("助动词"))
                return 2;
            if (wordType.Contains("形容词"))
                return 3;
            if (wordType.Contains("副词"))
                return 4;
            if (wordType.Contains("介词"))
                return 5;
            if (wordType.Contains("连接词"))
                return 6;
            if (wordType.Contains("及物动词"))
                return 7;
            if (wordType.Contains("不及物动词"))
                return 8;
            if (wordType.Contains("词组"))
                return 9;
            return 0;
        }

        static void Main(string[] args)
        {
            int MAX_WORD_COUNT = 130000;
            int BEGIN_WORD = 0;//未更新
            XmlDocument doc = new XmlDocument();
            doc.Load("AHD - Copy.xml");
            FileStream fs = new FileStream("sound.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);
            
            WordsssDB.WordsssDBManager manager = new WordsssDB.WordsssDBManager();
           
            XmlNode dictNode = doc.ChildNodes[1];
            Console.WriteLine(dictNode.ChildNodes.Count);

            for (int i = BEGIN_WORD; i < (BEGIN_WORD + MAX_WORD_COUNT) && i < dictNode.ChildNodes.Count; i++)
            {
                string word_name;
                string word_type;
                XmlNode ckNode = dictNode.ChildNodes[i];
                XmlNode dcNode = ckNode.SelectSingleNode("单词");
                if (dcNode == null)
                    continue;
                word_name = dcNode.FirstChild.Value;
                word_name = processString(word_name);
               
                if (i % 1000 == 0)
                    Console.WriteLine(i);
                int word_id = -1;
                if ((word_id = manager.getWordId(word_name)) == -1)
                    continue;
                //Console.WriteLine(word_name);
                
                XmlNodeList jxNodeList = ckNode.SelectNodes("单词解释块");
                foreach (XmlNode jxNode in jxNodeList)
                {
                    XmlNode dxNode = jxNode.SelectSingleNode("基本词义/单词词性");

                    if (dxNode != null)
                    {
                        word_type = dxNode.FirstChild.Value;
                    }
                    else
                        word_type = "";

                    XmlNode ybNode = jxNode.SelectSingleNode("基本词义/单词音标/国际音标");
                    if (ybNode == null)
                        continue;

                    string word_sound = ybNode.FirstChild.Value;
                    Regex pattern = new Regex("{([^}]*)}");
                    if(pattern.IsMatch(word_sound))
                        word_sound = pattern.Match(word_sound).Groups[1].Value;
                    word_sound = word_sound.Replace("'","''");
                    writer.WriteLine(word_name + " " + ybNode.FirstChild.Value + " " + getWordType(word_type));

                    manager.updateAHDSound(word_id, getWordType(word_type), word_sound);
                
                }
                //writer.WriteLine(doc.ChildNodes[1].FirstChild.SelectSingleNode("//JX").FirstChild.Value);
            }
            writer.Close();
        }
    }
}
