using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.IO;

namespace WordsssDB
{
    class Program
    {
        static void Main(string[] args)
        {
            WordsssDBManager dbManager = new WordsssDBManager();

            //if (dbManager.addWord("test"))
             //   Console.WriteLine("添加成功");
            
            //if (dbManager.deleteWord(3))
            //    Console.WriteLine("delete successfully");

            //if (dbManager.updateWord(2,"updateword"))
            //    Console.WriteLine("update successfully");

            /*Console.WriteLine(dbManager.queryWord(1));

            var result = dbManager.getAllWord();
            foreach (var word in result)
            {
                Console.WriteLine(word);
            }
             */
            //Console.Write("word_name:");
            //string word_name = Console.ReadLine();
            //Console.Write("word_paraphase:");
            //string word_paraphase = Console.ReadLine();
            //dbManager.addParaphase(word_name, 0, word_paraphase);

            /*dbManager.updateParaphase(1,0,"update关于"); 

            var paraphaseList = dbManager.getParaphase("about", 0);
            foreach (string paraphase in paraphaseList)
            {
                Console.WriteLine(paraphase);
            }

            dbManager.deleteParagraph(2, 0);

            var paraphaseList2 = dbManager.getParaphase("about", 0);
            foreach (string paraphase in paraphaseList2)
            {
                Console.WriteLine(paraphase);
            }*/

            
            dbManager.CloseManager();
        }
    }
}
