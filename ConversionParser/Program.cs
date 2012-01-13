using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace ConversionParser
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream fs = new FileStream("1_1_all_fullalpha.txt", FileMode.Open);
            StreamReader reader = new StreamReader(fs);

            FileStream writerFile = new FileStream("result.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(writerFile);

            bool hasConversion = false;
            Regex pattern = new Regex("^[a-zA-Z]");
            int count = 0;
            int i = 0;
            string current_word = "";
            HashSet<string> wordHash = new HashSet<string>();
            Dictionary<string, HashSet<string>> conversionDic = new Dictionary<string, HashSet<string>>();
            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();
            
                string[] strSplit = strLine.Split(new char[]{'\t'},StringSplitOptions.RemoveEmptyEntries);
                int frequency = int.Parse(strSplit[3]);

                if(i++ % 10000 == 0)
                Console.WriteLine(i);
                if(strSplit[2]== "%" && frequency != 0)
                {
                    if (!pattern.IsMatch(strSplit[0]))
                        continue;
                    hasConversion = true;
                    current_word = strSplit[0];
                    wordHash.Add(current_word);
                  //  count++;
                    continue;
                }
                else if (strSplit[2] == ":")
                {
                    hasConversion = false;
                    if (frequency != 0 && pattern.IsMatch(strSplit[0])) 
                    {
                        wordHash.Add(strSplit[0]);
                        count++;
                    }
                    continue;
                }
                else if (strSplit[2] == "%" && frequency == 0)
                {
                    hasConversion = false;
                    continue;
                }

                if (hasConversion == true)
                {
                    count++;
                    wordHash.Add(strSplit[2]);
                    if (conversionDic.Keys.Contains(current_word)&& current_word != strSplit[2])
                        conversionDic[current_word].Add(strSplit[2]);
                    else if(current_word  != strSplit[2]){
                        conversionDic.Add(current_word, new HashSet<string> { strSplit[2] });
                    }
                }
            }
            Console.WriteLine(count);
            Console.WriteLine(wordHash.Count());
            Console.WriteLine(conversionDic.Count());
            int countDict = 0;

            WordsssDB.WordsssDBManager manager = new WordsssDB.WordsssDBManager();
            foreach (string strConversion in wordHash)
            {

               /// writer.WriteLine(strConversion);
                if (conversionDic.Keys.Contains(strConversion))
                {
                    countDict++;
                    string word_base = strConversion.Replace("'", "''");
                    writer.WriteLine(countDict + " " + word_base);
                    foreach (string word in conversionDic[strConversion])
                    {
                        string word_conversion = word.Replace("'", "''");
                        writer.WriteLine("  " + word_conversion);
                        if (manager.addConversion(word_base,word_conversion) == -1)
                        {
                            writer.WriteLine("FAILED");
                        }
                    }
                }
            }
            writer.WriteLine(countDict);
            writer.WriteLine(conversionDic.Keys.Count());
            writer.WriteLine(wordHash.Count());
         
            writer.Close();
        }
    }
}
