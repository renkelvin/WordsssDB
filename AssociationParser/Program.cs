using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AssociationParser
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream fs = new FileStream("Association - 1",FileMode.Open);
            StreamReader reader = new StreamReader(fs);
            FileStream fsWrite = new FileStream("result.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(fsWrite);
            
            int association_type = 1;
            WordsssDB.WordsssDBManager manager = new WordsssDB.WordsssDBManager();
            while (!reader.EndOfStream)
            {
                string word_name = reader.ReadLine();
                if (word_name.Count() == 0)
                    word_name = reader.ReadLine();
                string word_meaning = reader.ReadLine();
                string association = reader.ReadLine();
                writer.WriteLine("word==>" + word_name);
                writer.WriteLine(word_meaning);
                writer.WriteLine(association);
                manager.addAssociation(word_name,association_type,word_meaning,association);
            }
            writer.Close();
            reader.Close();
        }
    }
}
