using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;

namespace WordsssDB
{
    public class WordsssDBManager
    {
        string strConn = "Driver={MySQL ODBC 5.1 Driver};Server=localhost;Database=word;User=root;Password=ohluyaomysql";
        OdbcConnection myConnection;

        private string getDictName(int dict_type)
        {
            switch( dict_type)
            {
                case 0: return "test_dic";
                case 1: return "oxford_dict";
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
            if(myCommand.ExecuteNonQuery() == 1)
                return word_id;
            return -1;
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
            string queryWord = String.Format("select word_id from word where word_name = '{0}'",word_name);
            OdbcCommand myCommand = new OdbcCommand(queryWord, myConnection);
            OdbcDataReader reader = myCommand.ExecuteReader();
            if (!reader.Read())
            {
                return -1;
            }
            return reader.GetInt32(0);
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

        private int addWordDictParaphase(int word_id, int dict_type)
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
            string insertParaphase = String.Format("insert into word_dict values({0},{1},{2})",word_dict_id,word_id,dict_type);
            myCommand = new OdbcCommand(insertParaphase,myConnection);
            if (myCommand.ExecuteNonQuery() == 1)
            {
                return word_dict_id;
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

        public int addParaphase(string word_name, int dict_type, string word_paraphase)
        {
            string dict_name = getDictName(dict_type);
            // CHECK IF WORD EXIST
            // IF EXIST GET WORD_ID
            // ELSE GET WORD_ID BY addWord()
            int word_id = getWordId(word_name);
            if (word_id == -1)
                word_id = addWord(word_name);

            // ADD Paraphase IN WORD_DICT GET WORD_DICT_ID
            int word_dict_id = addWordDictParaphase(word_id,0);
            int dict_id = -1;
            string queryDict = String.Format("select max(dict_id) from {0}",dict_name);
            OdbcCommand myCommand = new OdbcCommand(queryDict, myConnection);
            OdbcDataReader reader = myCommand.ExecuteReader();
            reader.Read();
            if (reader.IsDBNull(0))
            {
                dict_id = 1;
            }
            else
                dict_id = reader.GetInt32(0) + 1;

            // CHECK IF PARAPHASE TO BE INSERTED IS EXIST
            string queryParaphase = String.Format("select dict_id from {0} where dict_paraphase = '{1}'",dict_name,word_paraphase);
            myCommand = new OdbcCommand(queryParaphase, myConnection);
            reader = myCommand.ExecuteReader();
            if (reader.Read())
            {
                string deleteParaphase = String.Format("delete from word_dict where word_dict_id = {0}", word_dict_id);
                myCommand = new OdbcCommand(deleteParaphase, myConnection);
                myCommand.ExecuteNonQuery();
                return -1;
            }

            // INSET NEW PARAPHASE INTO SPECIFIED DICT
            string insertParaphase = String.Format("insert into {0} values({1},{2},'{3}')",dict_name,dict_id,word_dict_id,word_paraphase);
            myCommand = new OdbcCommand(insertParaphase, myConnection);
            if (myCommand.ExecuteNonQuery() == 1)
            {
                return dict_id;
            }
            return -1;
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
