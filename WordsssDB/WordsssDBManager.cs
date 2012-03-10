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

            string addWord = String.Format("insert into word values({0},'{1}',1)",word_id,word_name);
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
            reader.Read();

            if (!reader.IsDBNull(0))
            {
                return reader.GetInt32(0) + 1;
            }
            else
            {
                return 1;
            }
        }

        private int getInsertDictWordId(string dict_name,int word_id,int type)
        {
            string strQuery = String.Format("select word_dict_id from word_dict where word_id = {0}", word_id);
            OdbcCommand command = new OdbcCommand(strQuery, myConnection);
            OdbcDataReader reader = command.ExecuteReader();
            
            int word_dict_id = -1;
            if (reader.Read())
            {
                word_dict_id = reader.GetInt32(0);
                string strQueryDictWord = String.Format("select {0}_word_id from {0}_word where word_dict_id = {1} and type = {2}", dict_name, word_dict_id, type);
                command = new OdbcCommand(strQueryDictWord, myConnection);
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return reader.GetInt32(0);
                }
                else
                    return addDictWord(dict_name, type, word_dict_id);
            }
            else 
            {
                word_dict_id = addWordDictParaphase(word_id);
                if (word_id == -1)
                {
                    return -1;
                }
                return addDictWord(dict_name,type,word_dict_id);
            }
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

        private int addWordDictParaphase(int word_id)
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
            string insertParaphase = String.Format("insert into word_dict values({0},{1})",word_dict_id,word_id);
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
               // throw (e);
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

        public int addDictWord(string dict_name,int type,int word_dict_id)
        {
            int dict_word_id = getInsertId(dict_name + "_word"); 

            string strInsert = String.Format("insert into {0}_word values({1},{2},{3})", dict_name, dict_word_id,type,word_dict_id);
            OdbcCommand command = new OdbcCommand(strInsert, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                return -1;
            }
            return dict_word_id;
        }


        // PARAPHASE OPERATION

        public int addParaphase(string dict_name, string word_name,string meaning_cn,string meaning_en, int type )
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
            int dict_word_id = getInsertDictWordId(dict_name, word_id,type);
            if (dict_word_id == -1)
                return -1;

            OdbcCommand myCommand;
            // INSET NEW PARAPHASE INTO SPECIFIED DICT

            int new_dict_meaning_id = getInsertId(dict_name + "_meaning");

            string insertParaphase = String.Format("insert into {0}_meaning values({1},{2},'{3}','{4}')"
                                                    ,dict_name,new_dict_meaning_id, dict_word_id,meaning_en, meaning_cn);
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

        public int addParaphaseAhd(int dict_word_id, string meaning_cn, string meaning_en)
        {
           /// ADD Paraphase IN DICT_WORD GET DICT_WORD_ID
         
            if (dict_word_id == -1)
                return -1;

            OdbcCommand myCommand;
            // INSET NEW PARAPHASE INTO SPECIFIED DICT
            string dict_name = "ahd_dict";
            int new_dict_meaning_id = getInsertId(dict_name + "_meaning");

            string insertParaphase = String.Format("insert into {0}_meaning values({1},{2},'{3}','{4}')"
                                                    , dict_name, new_dict_meaning_id, dict_word_id, meaning_en, meaning_cn);
            myCommand = new OdbcCommand(insertParaphase, myConnection);
            try
            {
                if (myCommand.ExecuteNonQuery() != 1)
                {
                    return -1;
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


        // SENTENCE OPERATION

        public int addDictSentence(int dict_meaning_id, string sentence_en, string sentence_cn)
        {
            int sentence_id = getInsertId("ahd_dict_sentence");

            string strInsert = String.Format("insert into ahd_dict_sentence values({0},{1},'{2}','{3}')", sentence_id,dict_meaning_id, sentence_cn, sentence_en);
            OdbcCommand command = new OdbcCommand(strInsert, myConnection);
            try
            {
                if (command.ExecuteNonQuery() == -1)
                {
                    return -1;
                }
                else
                {
                    return sentence_id;
                }
            }
            catch (OdbcException e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }

        }


        //SENSE OPERATION
        public int addSense(string sense_name, int parent_id = 0)
        {
            int senseId = getInsertId("sense");
            string strInsert = String.Format("insert into sense (sense_id,meaning_cn) values({0},'{1}')",senseId,sense_name);
            OdbcCommand command = new OdbcCommand(strInsert, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                return -1;
            }

            if (parent_id == 0)
            {
                return senseId;
            }
            
            int senseFamilyId = getInsertId("sense_family");
            string insertFamily = String.Format("insert into sense_family values({0},{1},{2})", senseFamilyId, parent_id, senseId);
            command = new OdbcCommand(insertFamily, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                return -1;
            }

            return senseId;
        }

        public int addWordSense(string word_name, int sense_id, string word_meaning)
        {
            int word_id = getWordId(word_name);
            if (word_id == -1)
            {
                word_id = addWord(word_name);
            }
            if (word_id == -1)
            {
                return -1;
            }

            int word_sense_id = getInsertId("word_sense");
            string insertWordSense = String.Format("insert into word_sense values({0},{1},{2},'{3}')"
                                                    ,word_sense_id,word_id,sense_id,word_meaning);
            OdbcCommand command = new OdbcCommand(insertWordSense, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                return -1;
            }
            return word_sense_id;
        }

        //FREQUENCY OPERATION

        public int addFrequency(string word_name, int frequency, int frequency2, double frequency3)
        {
            int word_id = getWordId(word_name);
            if (word_id == -1)
            {
                word_id = addWord(word_name);
            }
            if (word_id == -1)
            {
                Console.WriteLine("add word failed");
                return -1;
            }

            int frequency_id = getInsertId("frequency");
            string strInsertFreq = String.Format("insert into frequency values({0},{1},{2},{3},{4})",
                                                  frequency_id, frequency,word_id, frequency2, frequency3);
            OdbcCommand command = new OdbcCommand(strInsertFreq, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                return -1;
            }
            return frequency_id;
        }


        // ASSOCIATION OPERATION
        public int addAssociation(string word_name, int association_type, string word_meaning, string description_cn)
        {
            int word_id = getWordId(word_name);
            if (word_id == -1)
            {
                Console.WriteLine("word {0} not fount",word_name);
                return -1;
            }

            int association_id = getInsertId("association");
            string insertAss = string.Format("insert into association values({0},{1},'{2}')",association_id,association_type,description_cn);
            OdbcCommand command = new OdbcCommand(insertAss, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                Console.WriteLine("add association failed");
                return - 1;
            }

            int word_association_id = getInsertId("word_association");
            string insertWordAss = string.Format("insert into word_association values({0},{1},{2},'{3}')",word_association_id,word_id,association_id,word_meaning);
            command = new OdbcCommand(insertWordAss, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                Console.WriteLine("add word_association failed");
                return -1;
            }
            return association_id;
        }

        //CONVERSION
        public int addConversion(string word_name, string conversion_name)
        {
            int word_id = getWordId(word_name);
            if (word_id == -1)
            {
                word_id = addWord(word_name);
            }

            int conversion_id = getWordId(conversion_name);
            if (conversion_id == -1)
            {
                conversion_id = addWord(conversion_name);
            }

            if (conversion_id == -1 || word_id == -1)
            {
                return -1;
            }
            int word_conversion_id = getInsertId("word_conversion");
            string strInsert = String.Format("insert into word_conversion values({0},{1},{2})",word_conversion_id,word_id,conversion_id);
            OdbcCommand command = new OdbcCommand(strInsert, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                return -1;
            }
            else
                return word_conversion_id;
        }

        public int addDerivation(string word_name, string derivation_name)
        {
            int word_id = getWordId(word_name);
            if (word_id == -1)
            {
                word_id = addWord(word_name);
            }

            int derivation_id = getWordId(derivation_name);
            if (derivation_id == -1)
            {
                derivation_id = addWord(derivation_name);
            }
            if (word_id == -1 || derivation_id == -1)
            {
                return -1;
            }
            int word_derivation_id = getInsertId("word_derivation");
            string strInsert = String.Format("insert into word_derivation values({0},{1},{2})",word_derivation_id,word_id,derivation_id);
            OdbcCommand command = new OdbcCommand(strInsert, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                return -1;
            }
            else
                return word_derivation_id;
        }

        public int addSynonym(string word_name, string word_synonym)
        {
            int word_name_id = getWordId(word_name);
            int word_synonym_id = getWordId(word_synonym);
            if (word_name_id == -1||word_synonym_id == -1)
            {
                return -1;
            }

            int synonym_id = getInsertId("synonym");
            string strInsert = String.Format("insert into synonym values({0},{1},{2})", synonym_id, word_name_id, word_synonym_id);
            OdbcCommand command = new OdbcCommand(strInsert, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                return -1;
            }
            else {
                return synonym_id;
            }
        }

        public int addAntonym(string word_name, string word_antonym)
        {
            int word_name_id = getWordId(word_name);
            int word_antonym_id = getWordId(word_antonym);
            if (word_name_id == -1 || word_antonym_id == -1)
            {
                return -1;
            }

            int antonym_id = getInsertId("antonym");
            string strInsert = String.Format("insert into antonym values({0},{1},{2})", antonym_id, word_name_id, word_antonym_id);
            OdbcCommand command = new OdbcCommand(strInsert, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                return -1;
            }
            else
            {
                return antonym_id;
            }
        }

        public int addRootaffix(string phrase, string deformation, string meaning_cn, string meaning_en)
        {
            int rootaffix_id = getInsertId("rootaffix");
            if (rootaffix_id == -1)
            {
                return -1;
            }

            string strInsert = String.Format("insert into rootaffix(rootaffix_id,phrase,deformation,meaning_cn,meaning_en)values({0},'{1}','{2}','{3}','{4}')",
                                                    rootaffix_id, phrase, deformation, meaning_cn, meaning_en);
            OdbcCommand command = new OdbcCommand(strInsert, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                return -1;
            }
            return rootaffix_id;
        }

        public int addWordRootaffix(string word_name, string word_meaning, string word_equation,int rootaffix_id)
        {
            int word_id = getWordId(word_name);
            if (word_id == -1)
                return -1;

            int word_rootaffix_id = getInsertId("word_rootaffix");
            string strInsert = String.Format("insert into word_rootaffix values({0},{1},{2},'{3}','{4}')",
                                        word_rootaffix_id, rootaffix_id, word_id, word_meaning, word_equation);

            OdbcCommand command = new OdbcCommand(strInsert, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                return -1;
            }
            return word_rootaffix_id;
        }

        public int addDictWord2(string word_name, int type)
        {
            int word_id = getWordId(word_name);
            if (word_id == -1)
                return -1;
            string strQuery = String.Format("select word_dict_id from word_dict where word_id = {0}", word_id);
            OdbcCommand command = new OdbcCommand(strQuery,myConnection);
            OdbcDataReader reader = command.ExecuteReader();
             int word_dict_id = -1;
            if (reader.Read())
            {
               word_dict_id = reader.GetInt32(0);
            }
            else
            {
                word_dict_id = getInsertId("word_dict");
                string strInsert = String.Format("insert into word_dict values({0},{1})",word_dict_id,word_id);
                command = new OdbcCommand(strInsert,myConnection);
                if(command.ExecuteNonQuery() == 0)
                {
                    return -1;
                }
            }

            int dict_word_id = getInsertId("ahd_dict_word");
            string strInsertDictWord = String.Format("insert into ahd_dict_word values({0},{1},{2})",dict_word_id,type,word_dict_id);
            command = new OdbcCommand(strInsertDictWord,myConnection);
            if(command.ExecuteNonQuery() == 0)
            {
                return -1;      
            }
            return dict_word_id;
        }
        //WORD_LIST
        public int addWordList(int word_id)
        {
            string strQuery = string.Format("select word_list_id from word_list where word_id = {0}",word_id);
            OdbcCommand command = new OdbcCommand(strQuery, myConnection);
            OdbcDataReader reader = command.ExecuteReader();
            int word_list_id = -1;
            if (reader.Read())
            {
                word_list_id = reader.GetInt32(0);
            }
            else {
                word_list_id = -1;
            }

            if (word_list_id != -1)
                return word_list_id;

            word_list_id = getInsertId("word_list");
            if (word_list_id == -1)
                return -1;

            string strInsert = string.Format("insert into word_list values({0},{1})", word_list_id, word_id);
            command = new OdbcCommand(strInsert, myConnection);
            if (command.ExecuteNonQuery() == 0)
                return -1;

            return word_list_id;
        }
        //LIST
        public int addCsListWord(string word_name, string full_name, string meaning)
        {
            int word_id = getWordId(word_name);
            if (word_id == -1)
            {
                return -1;
            }

            int word_list_id = addWordList(word_id);
            if (word_list_id == -1)
                return -1;

            int cs_list_word_id = getInsertId("cs_list_word");
            string strInsertCs = String.Format("insert into cs_list_word values({0},{1},'{2}','{3}')",
                                                cs_list_word_id,word_list_id,meaning,full_name);
            OdbcCommand command = new OdbcCommand(strInsertCs, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                return -1;
            }

            return cs_list_word_id;
        }

        public int addWlListWord(string word_name, string word_meaning)
        {
            int word_id = getWordId(word_name);
            if (word_id == -1)
            {
                return -1;
            }

            int word_list_id = addWordList(word_id);
            if (word_list_id == -1)
                return -1;

            int physics_list_word_id = getInsertId("ph_list_word");
            string strInsertPhysics = String.Format("insert into ph_list_word values({0},'{1}',{2})",
                                                physics_list_word_id, word_meaning, word_list_id);
            OdbcCommand command = new OdbcCommand(strInsertPhysics, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                return -1;
            }

            return physics_list_word_id;
        }

        public int addMaListWord(string word_name, string word_meaning)
        {
            int word_id = getWordId(word_name);
            if (word_id == -1)
            {
                    return -1;
            }

            int word_list_id = addWordList(word_id);
            if (word_list_id == -1)
                return -1;

            int math_list_word_id = getInsertId("ma_list_word");
            string strInsertMath = String.Format("insert into ma_list_word values({0},'{1}',{2})",
                                                math_list_word_id, word_meaning, word_list_id);
            OdbcCommand command = new OdbcCommand(strInsertMath, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                return -1;
            }

            return math_list_word_id;
        }

        //MWC   
        public int addMWCDictWord(string word_name,
                                  string function,
                                  string date,
                                  string etymology,
                                  string inflect)
        {
            int word_id = getWordId(word_name);
            if (word_id == -1)
                return -1;

            string strQuery = String.Format("select word_dict_id from word_dict where word_id = {0}", word_id);
            OdbcCommand command = new OdbcCommand(strQuery, myConnection);
            OdbcDataReader reader = command.ExecuteReader();
            int word_dict_id = -1;
            if (reader.Read())
            {
                word_dict_id = reader.GetInt32(0);
            }
            else
            {
                word_dict_id = getInsertId("word_dict");
                string strInsert = String.Format("insert into word_dict values({0},{1})", word_dict_id, word_id);
                command = new OdbcCommand(strInsert, myConnection);
                if (command.ExecuteNonQuery() == 0)
                {
                    return -1;
                }
            }

            function = function.Replace("'", "''");
            etymology = etymology.Replace("'", "''"); 
            date = date.Replace("'", "''"); 
            inflect = inflect.Replace("'", "''");
            int dict_word_id = getInsertId("mwc_dict_word");
            string strInsertDictWord = String.Format("insert into mwc_dict_word values({0},'{1}','{2}','{3}','{4}',{5})"
                                                , dict_word_id, function, etymology, inflect, date, word_dict_id);

            try
            {
                command = new OdbcCommand(strInsertDictWord, myConnection);
                if (command.ExecuteNonQuery() == 0)
                {
                    return -1;
                }
            }
            catch (OdbcException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("==>" + word_name);
            }
            return dict_word_id;
        }

        public int addMWCMeaning(int dict_word_id, string meaning)
        {
            if (dict_word_id == -1)
                return -1;
            int mwc_dict_meaning_id = getInsertId("mwc_dict_meaning");

            if (mwc_dict_meaning_id == -1)
                return -1;

            meaning = meaning.Replace("'", "''");
            string strInsert = string.Format("insert into mwc_dict_meaning values({0},'{1}',{2})",
                                            mwc_dict_meaning_id,meaning, dict_word_id);
            try
            {
                OdbcCommand command = new OdbcCommand(strInsert, myConnection);
                if (command.ExecuteNonQuery() == 0)
                    return -1;
            }
            catch (OdbcException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(meaning);
            }
            return mwc_dict_meaning_id;
        }

        //GRE
        public int addGREListWord(string word_name, string word_meaning)
        {
            int word_id = getWordId(word_name);
            if (word_id == -1)
                return -1;

            int word_list_id = addWordList(word_id);
            if (word_list_id == -1)
                return -1;

            int gre_list_word_id = getInsertId("gre_list_word");

            word_meaning = word_meaning.Replace("'", "''");
            string strInsert = string.Format("insert into gre_list_word values({0},'{1}',{2})", gre_list_word_id, word_meaning, word_list_id);
            OdbcCommand command = new OdbcCommand(strInsert, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                return -1;
            }

            return gre_list_word_id;
        }

        public int addGREListMemory(string type, string description, int gre_list_word_id)
        {
            int gre_list_memory_id = getInsertId("gre_list_memory");

            if (gre_list_memory_id == -1)
                return -1;

            if (gre_list_word_id == -1)
                return -1;

            description = description.Replace("'", "''");
            string strInsert = string.Format("insert into gre_list_memory values({0},'{1}',{2},'{3}')"
                                                ,gre_list_memory_id,
                                                description,
                                                gre_list_word_id,
                                                type
                                                );
            OdbcCommand command = new OdbcCommand(strInsert, myConnection);
            if (command.ExecuteNonQuery() == 0)
            {
                return -1;
            }
            return gre_list_memory_id;
        }
    }
}
