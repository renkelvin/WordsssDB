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
        public static string processString(string str)
        {
            string result = str.Replace("&I{", "");
            result = result.Replace("}", "");
            result = result.Replace("&b{","");
            result = result.Replace("'", "''");
            result = result.Replace("?", "");
            return result;
        }

        public static int getWordType(string wordType)
        {
            if (wordType.Contains("名词"))
                return 1;
            if (wordType.Contains("动词")&&wordType.Contains("助动词"))
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
            int MAX_WORD_COUNT = 100000;
            int BEGIN_WORD = 10;//未更新
            XmlDocument doc = new XmlDocument();
            doc.Load("AHD.xml");
            FileStream fs = new FileStream("AHD2.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);
            WordsssDBManager manager = new WordsssDBManager();

            XmlNode dictNode = doc.ChildNodes[1];
            Console.WriteLine(dictNode.ChildNodes.Count);

            for (int i = BEGIN_WORD; i < (BEGIN_WORD + MAX_WORD_COUNT) && i <dictNode.ChildNodes.Count; i++)
            {
                string word_name;
                string word_paraphase;
                string word_type;
                XmlNode ckNode = dictNode.ChildNodes[i];
                XmlNode dcNode = ckNode.SelectSingleNode("单词");
                if (dcNode == null)
                    continue;
                word_name = dcNode.FirstChild.Value;
                word_name = processString(word_name);
                writer.WriteLine(word_name);
                Console.WriteLine(i);

                XmlNodeList jxNodeList = ckNode.SelectNodes("单词解释块");
                foreach (XmlNode jxNode in jxNodeList)
                {
                    XmlNode dxNode = jxNode.SelectSingleNode("基本词义/单词项/单词词性");
                    
                    if (dxNode != null)
                    {
                        word_type = dxNode.FirstChild.Value;
                    }
                    else
                        word_type = "";

                    XmlNodeList jxNodes = jxNode.SelectNodes("基本词义/单词项/跟随注释");
                    
                    XmlNode yxNode = jxNode.SelectSingleNode("基本词义/单词项/单词原型");

                    int dict_word_id = manager.addDictWord2(word_name,getWordType(word_type));

                    if(yxNode != null && yxNode.FirstChild.Value != word_name)
                        writer.WriteLine(processString(yxNode.FirstChild.Value));
                    bool isParent = true;
                    if (jxNodes != null)
                    {
                        foreach (XmlNode jx in jxNodes)
                        {
                            if (jx.NextSibling != null && jx.NextSibling.Name == "子解释项" && isParent == true)
                            {
                                Console.WriteLine(jx.NextSibling.FirstChild.Name);
                               // Console.WriteLine(jx.NextSibling.FirstChild.Value);
                                isParent = false;
                                continue;
                            }
                            else if (jx.NextSibling != null && jx.NextSibling.Name != "子解释项")
                            {
                                isParent = true;
                            }
                            int dict_meaning_id = -1;
                            word_paraphase = jx.FirstChild.Value;
                            if (word_paraphase == " ")
                                continue;
                            //if(manager.addParaphase("mcec_dict", word_name, word_paraphase, word_type)==-1)
                            writer.WriteLine(getWordType(word_type));
                            string meaning_en = processString(jx.PreviousSibling.FirstChild.Value);
                            string meaning_cn = processString(word_paraphase);
                            writer.WriteLine(processString(jx.PreviousSibling.FirstChild.Value));
                            writer.WriteLine(processString(word_paraphase));


                            dict_meaning_id = manager.addParaphaseAhd(dict_word_id,meaning_cn,meaning_en);
                            if (dict_meaning_id == -1)
                            {
                                writer.WriteLine("MEANING_FAILED");
                                continue;
                            }
                            if (i % 1000 == 0)
                                Console.WriteLine("  " + i);
                            XmlNode ljNode = jx.NextSibling;
                            if (ljNode != null && ljNode.Name == "例句")
                            {
                                XmlNodeList ljNodeList = ljNode.SelectNodes("例句原型");
                                foreach (XmlNode lj in ljNodeList)
                                {
                                    string sentence_en = processString(lj.FirstChild.Value);
                                    string sentence_cn = processString(lj.NextSibling.FirstChild.Value);
                                    if (manager.addDictSentence(dict_meaning_id, sentence_en, sentence_cn) == -1)
                                        writer.WriteLine("SENTENCE_FAILED");
                                    writer.WriteLine(processString(lj.FirstChild.Value));
                                    writer.WriteLine(processString(lj.NextSibling.FirstChild.Value));
                                }
                            }
                        }
                    }
                }
                //writer.WriteLine(doc.ChildNodes[1].FirstChild.SelectSingleNode("//JX").FirstChild.Value);
            }
            writer.Close();
        }
    }
}
