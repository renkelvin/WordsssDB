using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace WeiDictParser
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream fs = new FileStream("MWC.txt",FileMode.Open);

            StreamReader reader = new StreamReader(fs);
           
            FileStream fw = new FileStream("result.txt",FileMode.Create);
            StreamWriter writer = new StreamWriter(fw);

            FileStream ferror = new FileStream("error_3_1.txt", FileMode.Create);
            StreamWriter error_writer = new StreamWriter(ferror);

            MWCMatcher matcher = new MWCMatcher();
            string strLine="";
            int currentIndex = 0;
            WordsssDB.WordsssDBManager manager = new WordsssDB.WordsssDBManager();

            
            while (!reader.EndOfStream)
            {
                strLine = reader.ReadLine();
                if (strLine != "")
                    matcher.matchLine(strLine);
                if (matcher.isComplete)
                {
                    writer.WriteLine(matcher.word_name);
                    writer.WriteLine("Function: " + matcher.function);
                    writer.WriteLine("InflectForm: " + matcher.inflectform);
                    writer.WriteLine("Date: " + matcher.date);
                    writer.WriteLine("Etymology: " + matcher.etymology);
                    writer.WriteLine("Definition:");
                    int dict_word_id = -1;
                  
                    if ((dict_word_id = manager.addMWCDictWord(matcher.word_name, matcher.function, matcher.date, matcher.etymology, matcher.inflectform)) == -1)
                    {
                        error_writer.WriteLine(matcher.word_name);
                    }
                    int count = matcher.defList.Count;
                    if (matcher.defList.Count > 1)
                    {
                        for (int i = 1; i <= matcher.defList.Count; i++)
                        {
                            writer.WriteLine(i + " " + matcher.defList[i - 1]);
                            if (manager.addMWCMeaning(dict_word_id, matcher.defList[i - 1]) == -1)
                            {
                                error_writer.WriteLine("<" + matcher.word_name + ">" + matcher.defList[i - 1]);
                            }
                        }
                    }
                    else if (matcher.defList.Count == 1)
                    {
                        writer.WriteLine(matcher.defList[0]);
                        if (manager.addMWCMeaning(dict_word_id, matcher.defList[0]) == -1)
                        {
                            error_writer.WriteLine("<" + matcher.word_name + ">" + matcher.defList[0]);
                        }
                    }
                    currentIndex++;
                    if(currentIndex % 1000 == 0)
                    Console.WriteLine(currentIndex);

                    matcher = new MWCMatcher();
                    if (count > 1)
                        matcher.matchLine(strLine);
                }
                else if (matcher.isTransitive)
                {
                    writer.WriteLine(matcher.word_name);
                    writer.WriteLine("Function: " + matcher.function);
                    writer.WriteLine("InflectForm: " + matcher.inflectform);
                    writer.WriteLine("Date: " + matcher.date);
                    writer.WriteLine("Etymology: " + matcher.etymology);
                    writer.WriteLine("Definition:");
                    int dict_word_id = -1;
                    if ((dict_word_id = manager.addMWCDictWord(matcher.word_name, matcher.function, matcher.date, matcher.etymology, matcher.inflectform)) == -1)
                    {
                        error_writer.WriteLine(matcher.word_name);
                    }
                    if (matcher.defList.Count > 1)
                    {
                        for (int i = 1; i <= matcher.defList.Count; i++)
                        {
                            writer.WriteLine(i + " " + matcher.defList[i - 1]);
                            if (manager.addMWCMeaning(dict_word_id, matcher.defList[i - 1]) == -1)
                              {
                                  error_writer.WriteLine("<" + matcher.word_name + ">" + matcher.defList[i - 1]);
                              }
                        }
                        matcher.defList.Clear();
                        matcher.matchLine(strLine);
                    }
                    else if (matcher.defList.Count == 1)
                    {
                        writer.WriteLine(matcher.defList[0]);
                        if (manager.addMWCMeaning(dict_word_id, matcher.defList[0]) == -1)
                        {
                            error_writer.WriteLine("<" + matcher.word_name + ">" + matcher.defList[0]);
                        }
                        matcher.defList.Clear();
                        if (matcher.isNeedRead)
                            matcher.matchLine(strLine);
                    }
                    currentIndex++;
                    if (currentIndex % 1000 == 0)
                        Console.WriteLine(currentIndex);
                }
                else if (matcher.isIntransitive)
                {
                    writer.WriteLine(matcher.word_name);
                    writer.WriteLine("Function: " + matcher.function);
                    writer.WriteLine("InflectForm: " + matcher.inflectform);
                    writer.WriteLine("Date: " + matcher.date);
                    writer.WriteLine("Etymology: " + matcher.etymology);
                    writer.WriteLine("Definition:");
                    int dict_word_id = -1;
                    if ((dict_word_id = manager.addMWCDictWord(matcher.word_name, matcher.function, matcher.date, matcher.etymology, matcher.inflectform)) == -1)
                    {
                        error_writer.WriteLine(matcher.word_name);
                    }
                    if (matcher.defList.Count > 1)
                    {
                        for (int i = 1; i <= matcher.defList.Count; i++)
                        {
                            writer.WriteLine(i + " " + matcher.defList[i - 1]);
                            if (manager.addMWCMeaning(dict_word_id, matcher.defList[i - 1]) == -1)
                              {
                                  error_writer.WriteLine("<" + matcher.word_name + ">" + matcher.defList[i - 1]);
                              }
                        }
                        matcher.defList.Clear();
                        matcher.matchLine(strLine);
                    }
                    else if (matcher.defList.Count == 1)
                    {
                        writer.WriteLine(matcher.defList[0]);
                        
                        if (manager.addMWCMeaning(dict_word_id, matcher.defList[0]) == -1)
                        {
                            error_writer.WriteLine("<" + matcher.word_name + ">" + matcher.defList[0]);
                        }
                        matcher.defList.Clear();
                        if (matcher.isNeedRead)
                            matcher.matchLine(strLine);
                    }
                    currentIndex++;
                    if (currentIndex % 1000 == 0)
                        Console.WriteLine(currentIndex);
                }
                
            }
            error_writer.Close();
            writer.Close();
            
        }
    }
}
