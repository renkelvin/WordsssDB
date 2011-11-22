using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;

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

            Console.WriteLine(dbManager.queryWord(1));

            var result = dbManager.getAllWord();
            foreach (var word in result)
            {
                Console.WriteLine(word);
            }
            dbManager.CloseManager();
        }
    }
}
