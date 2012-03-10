using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace AssociationParser
{
    class Program
    {
        public static int getWordType(string wordType)
        {
            switch (wordType)
            {
                case "n.": return 1;
                case "v.": return 2;
                case "adj.": return 3;
                case "adv.": return 4;
                case "prep.": return 5;
                case "conj.": return 6;
                case "vt.": return 7;
                case "vi.": return 9;
                default: return 0;
            }
        }

        static void Main(string[] args)
        {
            FileStream fs = new FileStream("Association - 2.txt",FileMode.Open);
            StreamReader reader = new StreamReader(fs,Encoding.GetEncoding("gb2312"));
            FileStream fsWrite = new FileStream("result.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(fsWrite,Encoding.UTF8);
            
            WordsssDB.WordsssDBManager manager = new WordsssDB.WordsssDBManager();

            int association_type = 1;
            string word_name = "";
            string word_meaning = "";
            int iType = 0;
            string word_type = "";
            string description = "";

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();

                Regex matchType = new Regex("^[adjvitn/\\.]+\\.");
                Regex matchNum = new Regex("^[1-4]$");

                if (strLine.StartsWith("word==>"))
                {
                    word_name = strLine.Split('>')[1];
                }else if(matchType.IsMatch(strLine))
                {
                    word_meaning = strLine;
                }
                else if (matchNum.IsMatch(strLine))
                {
                    association_type = int.Parse(strLine);
                    writer.WriteLine(word_name);
                    writer.WriteLine(word_meaning);
                    writer.WriteLine(description);
                    writer.WriteLine(association_type);
                   manager.addAssociation(word_name, association_type, word_meaning, description);
                }
                else
                {
                    description = strLine;
                }
                //manager.addAssociation(word_name,association_type,word_meaning,association);
            }
            writer.Close();
            reader.Close();
        }
    }
}
