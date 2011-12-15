using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;

namespace WordsssDB
{
    public class WordsssDBManager
    {
        string strConn = "Driver={MySQL ODBC 5.1 Driver};Server=localhost;Database=word_test;User=root;Password=ohluyaomysql";
        OdbcConnection myConnection;

        private string getDictName(int dict_type)
        {
            switch( dict_type)
            {
                case 0: return "test_dic";
                case 1: return "oxford_dict";
                case 2: return "mcec_dict";
                default: return "null";
            }
        }
        public WordsssDBManager()
        {
            myConnection = new OdbcConnection(strConn);
            myConnection.Open();
        }

        public void CloseManager()
        {
            myConnection.Close();
        }

        public int addWord(string word_name)
        {
            string queryMaxId = "select Max(word_id) from word";
            OdbcCommand myCommand = new OdbcCommand(queryMaxId, myConnection);
            OdbcDataReader reader = myCommand.ExecuteReader();
            reader.Read();
            int word_id = reader.GetInt32(0) + 1;

            string addWord = String.Format("insert into word values({0},'{1}')",word_id,word_name);
            myCommand = new OdbcCommand(addWord, myConnection);
            try
            {
                if (myCommand.ExecuteNonQuery() == 1)
                    return word_id;
                else
                    return -1;
            }
            catch (OdbcException e)
            {
                //throw (e);
                return -1;
            }
       }

        public bool deleteWord(int word_id)
        {
            string deleteSql = String.Format("delete from word where word_id = {0}", word_id);
            OdbcCommand delCommand = new OdbcCommand(deleteSql, myConnection);
            if(delCommand.ExecuteNonQuery() == 1)
                return true;
            return false;
        }

        public bool updateWord(int word_id, string word_name)
        {
            string updateSql = String.Format(
                            "update word set word_name = '{0}' where word_id = {1}", word_name, word_id);
            OdbcCommand updateCommand = new OdbcCommand(updateSql, myConnection);

            if (updateCommand.ExecuteNonQuery() == 1)
                return true;
            return false;
        }

        public string queryWord(int word_id)
        {
            string querySql = String.Format("select * from word where word_id = {0}", word_id);
            OdbcCommand queryCmd = new OdbcCommand(querySql, myConnection);
            OdbcDataReader reader = queryCmd.ExecuteReader();
            if (reader.Read())
            {
                return String.Format("{0},{1}", reader[0], reader[1]);
            }
            return "can't find word";
        }

        private int getWordId(string word_name)
        {
            string queryWord = String.Format("select word_id from word where binary word_name = '{0}'",word_name);
            OdbcCommand myCommand = new OdbcCommand(queryWord, myConnection);
            try
            {
                OdbcDataReader reader = myCommand.ExecuteReader();
                if (!reader.Read())
                {
                    return -1;
                }
                return reader.GetInt32(0);
            }
            catch (OdbcException e)
            {
                Console.WriteLine("get word_id error!");
                Console.WriteLine(e.Message);
                return -1;
            }
        }

        private int getInsertId(string table_name)
        {
            string strQuery = String.Format("select max({0}_id) from {0}",table_name);
            OdbcCommand command = new OdbcCommand(strQuery, myConnection);
            OdbcDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetInt32(0) + 1;
            }
            else
            {
                return 1;
            }
        }

        private int getInsertDictWordId(string dict_name,int word_id)
        {
            string strQuery = String.Format("select {0}_word_id from word_dict where word_id = {1}", dict_name, word_id);
            OdbcCommand command = new OdbcCommand(strQuery, myConnection);
            OdbcDataReader reader = command.ExecuteReader();
            int dict_word_id;
            if (reader.Read())
            {
                dict_word_id = reader.GetInt32(0);
            }
            else 
            {
                dict_word_id = addDictWord(dict_name);
                if (addWordDictParaphase(word_id, dict_name + "_word_id", dict_word_id) == -1)
                {
                    return -1; 
                }
            }
            return dict_word_id;
        }
        
        public IEnumerable<string> getAllWord()
        {
            string querySql = String.Format("select * from word");
            OdbcCommand queryCmd = new OdbcCommand(querySql, myConnection);
            OdbcDataReader reader = queryCmd.ExecuteReader();
            
            List<string> resultList = new List<string>();
            while (reader.Read())
            {
                string result = String.Format("{0},{1}", reader[0], reader[1]);
                resultList.Add(result);
            }
            return resultList;
        }

        private int addWordDictParaphase(int word_id, string insertColumn, int dict_id)
        {
            // GET MAX WORD_DICT_ID TO INSERT NEW ROW
            string queryMaxId = String.Format("select max(word_dict_id) from word_dict");
            OdbcCommand myCommand = new OdbcCommand(queryMaxId,myConnection);
            OdbcDataReader reader = myCommand.ExecuteReader();
            int word_dict_id = -1;
            reader.Read();
            if (reader.IsDBNull(0))
            {
                word_dict_id = 1;   
            }
            else
                word_dict_id = reader.GetInt32(0) + 1;

            // INSERT TNTO WORD_DICT A NEW ROW
            string insertParaphase = String.Format("insert into word_dict (word_dict_id,word_id,{3})values({0},{1},{2})",word_dict_id,word_id,dict_id,insertColumn);
            myCommand = new OdbcCommand(insertParaphase,myConnection);
            try
            {
                if (myCommand.ExecuteNonQuery() == 1)
                {
                    return word_dict_id;
                }
            }
            catch (OdbcException e)
            {
                //throw (e);
                Console.WriteLine("insert into word_dict error");
            }
            return -1;
        }

        private IEnumerable<int> getWordDictId(string word_name, int dict_type)
        {
            int word_id = getWordId(word_name);
            List<int> word_dict_idList = new List<int>();

            string queryParaphase = String.Format("select word_dict_id from word_dict where word_id = {0} and dict_type = {1}", word_id,dict_type);
            OdbcCommand myCommand = new OdbcCommand(queryParaphase, myConnection);
            OdbcDataReader reader = myCommand.ExecuteReader();
            while(reader.Read())
            {
                int word_dict_id = reader.GetInt32(0);
                word_dict_idList.Add(word_dict_id);
            }
            return word_dict_idList;
        }

        private int addDictWord(string dict_name)
        {
            string strQury = String.Format("select max({0}_word_id) from {0}_word", dict_name);
            OdbcCommand command = new OdbcCommand(strQury, myConnection);
            OdbcDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return -1;
            }
            int dict_word_id = reader.GetInt32(0);

            string strInsert = String.Format("insert into {0}_word values({1})", dict_name, dict_word_id + 1);
            command = new OdbcCommand(strInsert, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                return -1;
            }
            return dict_word_id + 1;
        }


        // PARAPHASE OPERATION

        public int addParaphase(string dict_name, string word_name,string word_paraphase, string word_type)
        {
            if (dict_name == null)
                return -1;
            // CHECK IF WORD EXIST
            // IF EXIST GET WORD_ID
            // ELSE GET WORD_ID BY addWord()
            int word_id = getWordId(word_name);
            if (word_id == -1)
                word_id = addWord(word_name);

            if(word_id == -1)
                return -1;


            // ADD Paraphase IN DICT_WORD GET DICT_WORD_ID
            int dict_word_id = getInsertDictWordId(dict_name, word_id);
            if (dict_word_id == -1)
                return -1;

            // CHECK IF PARAPHASE TO BE INSERTED IS EXIST
            string queryParaphase = String.Format("select {0}_meaning_id from {0}_meaning where meaning_cn = '{1}'",dict_name,word_paraphase);
            OdbcCommand myCommand = new OdbcCommand(queryParaphase, myConnection);
            OdbcDataReader reader = null; 
            try
            {
                reader = myCommand.ExecuteReader();
                if (reader.Read())
                {
                    // UPDATE DICT_MEANING.DICT_WORD_ID IF MEANING EXIST
                    int dict_meaning_id = reader.GetInt32(0);
                    string updateMeaning = String.Format("update {0}_meaning set {0}_word_id = {1} where {0}_meaning_id = {2}"
                                                        , dict_name, dict_word_id, dict_meaning_id);
                    myCommand = new OdbcCommand(updateMeaning, myConnection);
                    if (myCommand.ExecuteNonQuery() == 0)
                    {
                        return -1;
                    }
                    else 
                    {
                        return dict_meaning_id;
                    }
                }
            }
            catch (OdbcException e)
            {
                //throw (e);
            }


            // INSET NEW PARAPHASE INTO SPECIFIED DICT
            if (word_type == null)
                word_type = "";

            int new_dict_meaning_id = getInsertId(dict_name + "_meaning");

            string insertParaphase = String.Format("insert into {0}_meaning values({1},'{2}','{3}',{4})"
                                                    ,dict_name,new_dict_meaning_id,word_paraphase,word_type,dict_word_id);
            myCommand = new OdbcCommand(insertParaphase, myConnection);
            try
            {
                if (myCommand.ExecuteNonQuery() != 1)
                {
                    return -1 ;
                }
            }
            catch (OdbcException e)
            {
                //throw (e);
                Console.WriteLine("insert into {0} error", dict_name);
                Console.WriteLine(e.Message);
                return -1;
               
            }
            return new_dict_meaning_id;
        }

        public IEnumerable<string> getParaphase(string word_name, int dict_type)
        {
            string dict_name = getDictName(dict_type);

            var word_dict_idList = getWordDictId(word_name, dict_type);
            List<string> paragraphList = new List<string>();

            foreach (int word_dict_id in word_dict_idList)
            {
                string queryParaphase = String.Format("select dict_paraphase from {0} where word_dict_id = {1}", dict_name, word_dict_id);
                OdbcCommand myCommand = new OdbcCommand(queryParaphase, myConnection);
                OdbcDataReader reader = myCommand.ExecuteReader();
                if(reader.Read())
                    paragraphList.Add(reader[0].ToString());    
            }
            return paragraphList;
        }

        public bool updateParaphase(int dict_id, int dict_type, string dict_paraphase)
        {
            string dict_name = getDictName(dict_type);
            
            // CHECK IF PARAPHASE TO BE UPDATED IS EXIST
            // IF NOT RETURN FALSE
            string queryParaphase = String.Format("select * from {0} where dict_id = {1}", dict_name, dict_id);
            OdbcCommand myCommand = new OdbcCommand(queryParaphase, myConnection);
            OdbcDataReader reader = myCommand.ExecuteReader();
            if (!reader.Read())
                return false;

            string updateParaphase = String.Format(
                "update {0} set dict_paraphase = '{1}' where dict_id = {2} ",
                dict_name,dict_paraphase, dict_id);
            myCommand = new OdbcCommand(updateParaphase, myConnection);
            if (myCommand.ExecuteNonQuery() == 1)
                return true;
            return false;
        }

        public bool deleteParagraph(int dict_id, int dict_type)
        {
            string dict_name = getDictName(dict_type);

            // CHECK IF PARAPHASE TO BE UPDATED IS EXIST
            // IF NOT RETURN FALSE
            string queryParaphase = String.Format("select * from {0} where dict_id = {1}", dict_name, dict_id);
            OdbcCommand myCommand = new OdbcCommand(queryParaphase, myConnection);
            OdbcDataReader reader = myCommand.ExecuteReader();
            if (!reader.Read())
                return false;

            string deleteParaphase = String.Format(
                "delete from {0} where dict_id = {1}", dict_name, dict_id);
            myCommand = new OdbcCommand(deleteParaphase, myConnection);
            if (myCommand.ExecuteNonQuery() == 1)
                return true;
            return false;
        }
    }
}
