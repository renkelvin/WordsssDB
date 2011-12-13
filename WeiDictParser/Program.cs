using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HtmlAgilityPack;
using System.IO;

namespace WeiDictParser
{
    class Program
    {
        public static string ConvertString(string str)
        {
            string result = str;
            result = result.Replace("&nbsp", "\r\n");
            result = result.Replace("&gt;", ">");
            result = result.Replace("&lt;", "<");
           /* result = result.Replace("\r\n", " ");
            result = result.Replace("1 :","\n1:");
            result = result.Replace("2 :", "\n2:");
            result = result.Replace("3 :", "\n3:");
            result = result.Replace("4 :", "\n4:");
            result = result.Replace("5 :", "\n5:");
            result = result.Replace("6 :", "\n6:");
            result = result.Replace("7 :", "\n7:");
            result = result.Replace("8 :", "\n8:");
            result = result.Replace("9 :", "\n9:");
         */   return result;
        }
        static void Main(string[] args)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load("weitest.htm",Encoding.GetEncoding("gb2312"));
            
            HtmlNodeCollection pNodeList = doc.DocumentNode.SelectNodes("//body/div/p/b/span");
            
            HtmlNodeCollection meanNodeList = 
               doc.DocumentNode.SelectNodes("//p[@style='text-align:left;line-height:14.4pt']|//p[@style='line-height:14.4pt']");
            
            FileStream fs = new FileStream("text.txt",FileMode.Create);

            StreamWriter writer = new StreamWriter(fs, Encoding.GetEncoding("gb2312"));
            
            
            WordsssDB.WordsssDBManager manager = new WordsssDB.WordsssDBManager();
            
            int i = 0;
            foreach (HtmlNode node in pNodeList)
            {
                if (node.InnerText == "'" || node.InnerText == ": ")
                {
                    continue;
                }
                string word_name = node.InnerText;
                word_name = word_name.Replace("·", "");
                word_name = word_name.Replace("'", "''");

              
                if (i < meanNodeList.Count())
                {
                    string meaning = meanNodeList[i++].InnerText;
                    meaning = meaning.Replace("'", "''");
                    meaning = ConvertString(meaning);

                    Console.WriteLine(manager.addParaphase("mwc_dict", word_name, meaning, null));
                    writer.WriteLine("{0}==>{1}", word_name,i);
                    writer.WriteLine(meaning);
    
                }

           
            }
            Console.WriteLine(pNodeList.Count(node=>node.InnerText != "'" && node.InnerText != ": "));
            Console.WriteLine(meanNodeList.Count());
            writer.Close();
            
        }
    }
}
