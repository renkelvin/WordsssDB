using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace RootAfixParser2
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream fs = new FileStream("rootafix.txt", FileMode.Open);
            StreamReader reader = new StreamReader(fs);

            FileStream fsWrite = new FileStream("result.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(fsWrite);

            FileStream fsWrite2 = new FileStream("error.txt",FileMode.Create);
            StreamWriter writer2 = new StreamWriter(fsWrite2);
            
            Regex patternRootafix = new Regex("^[0-9]");
            Regex patternRootMeaning = new Regex("^[(]");
            Regex patternWordRoot = new Regex("^[a-zA-Z]");

            string currentRootafix = "";
            string currentDeformation = "";
            string currentMeaningCn = "";
            string currentMeaningEn = "";
            int RootaffixId = -1;
            WordsssDB.WordsssDBManager manager = new WordsssDB.WordsssDBManager();

            bool hasMeaning = false;

            int k = 0;

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();
                if (patternRootafix.IsMatch(strLine))
                {
                    Regex pattern = new Regex("[a-z， ')(=-]+");

                    Match matchRootAfix = pattern.Match(strLine);

                    string strRootafix = matchRootAfix.Value;

                    if (strRootafix.Contains("="))
                    {
                        string[] strSplit = matchRootAfix.Value.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                        strRootafix = strSplit[0];
                        if (strSplit.Count() > 1)
                        {
                            //Console.WriteLine(matchRootAfix.Value);
                            //Console.WriteLine(strSplit.Count());
                            currentMeaningEn = strSplit[1];
                            currentMeaningEn = currentMeaningEn.Replace("，",",");
                            if (currentMeaningEn.ElementAt(currentMeaningEn.Length - 1) == ',')
                            {
                                currentMeaningEn = currentMeaningEn.Remove(currentMeaningEn.Length - 1);
                            }
                            currentMeaningEn = currentMeaningEn.Replace("'", "''");
                            writer.WriteLine("MeaningEn:    " + currentMeaningEn);
                        }
                    }
                    string[] meaningList = strRootafix.Split('，');
                    currentRootafix = meaningList[0];
                    currentRootafix = currentRootafix.Replace("'", "''");
                    writer.WriteLine("Rootaffix:    " + currentRootafix);

                    if (meaningList.Count() > 1)
                        currentDeformation = meaningList[1];
                    else
                        currentDeformation = "";
                    currentDeformation = currentDeformation.Replace("'", "''");
                    for (int i = 2; i < meaningList.Count(); i++)
                    {
                        currentDeformation += (" " + meaningList[i]);
                    }

                    writer.WriteLine("Deformation:  " + currentDeformation);

                    if (matchRootAfix.Index + matchRootAfix.Length != strLine.Length)
                    {
                        currentMeaningCn = strLine.Substring(matchRootAfix.Index + matchRootAfix.Length);
                        writer.WriteLine("MeaningCn:    " +currentMeaningCn);
                        writer.WriteLine("");
                        hasMeaning = true;
                        }
                    else
                    {
                        hasMeaning = false;
                    }

                    RootaffixId = manager.addRootaffix(currentRootafix,currentDeformation,currentMeaningCn,currentMeaningEn);
                }
                else if (patternWordRoot.IsMatch(strLine))
                {
                    Regex patternWordName = new Regex("[a-zA-Z]+");
                    Match matchWordName = patternWordName.Match(strLine);
                    string strWordName = matchWordName.Value;
                    int i = strWordName.Length - 1;
                    while (strWordName.ElementAt(i--) == ' ') ;
                    if(i+2 < strWordName.Length)
                    strWordName = strWordName.Remove(i+2);
                    string strWordType = "";
                    writer.WriteLine(strWordName);

                    int meaningStart = matchWordName.Index + matchWordName.Length;

                    Regex patternWordEuqal = new Regex("[(][a-z]");
                    string strWordMeaning = "";
                    string strWordEqual = "";
                    if (strLine.ElementAt(strLine.Length-1) == ')')
                    {
                        int meaningEnd = strLine.Length - 1;
                        int top = 1;
                        while (top > 0)
                        {
                            meaningEnd--;
                            if (strLine.ElementAt(meaningEnd) == '(')
                            {
                                top--;
                            }
                            else if (strLine.ElementAt(meaningEnd) == ')')
                            {
                                top++;
                            }
                        }
                        strWordMeaning = strLine.Substring(meaningStart, meaningEnd - meaningStart);
                        strWordEqual = strLine.Substring(meaningEnd + 1, strLine.Length - meaningEnd - 2);
                        strWordMeaning = strWordMeaning.Replace("<", "");
                        strWordMeaning = strWordMeaning.Replace(">", "");
                        writer.WriteLine("Meaning:  " + strWordMeaning);
                        strWordEqual = strWordEqual.Replace("→", "->");
                        strWordEqual = strWordEqual.Replace(" ", "");
                        writer.WriteLine("Equation:    " + strWordEqual);
                    }
                    else
                    {
                        strWordMeaning = strLine.Substring(meaningStart);
                        strWordMeaning = strWordMeaning.Replace("<", "");
                        strWordMeaning = strWordMeaning.Replace(">", "");
                        writer.WriteLine("Meaning:  " + strWordMeaning);
                    }
                    if (RootaffixId != -1)
                    {
                        if (manager.addWordRootaffix(strWordName, strWordMeaning, strWordEqual, RootaffixId) == -1)
                        {
                            writer2.WriteLine(strWordName);
                            Console.WriteLine(strWordName+"'");
                        }
                    }
                }
                else
                {
                    writer2.WriteLine(strLine);
                }
                
            }
            writer.Close();
            writer2.Close();
        }
    }
}
