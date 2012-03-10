using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WeiDictParser
{
    enum MatchPosition
    {
        MATCH_WORD,
        MATCH_PROPERTY,
        MATCH_DEFINITION,
    }
    class MWCMatcher
    {
        public string word_name;
        public string function;
        public string date;
        public string etymology;
        public string inflectform;

        public bool isComplete;
        public bool isValideWord;
        public bool isTransitive;
        public bool isIntransitive;
        public bool isNeedRead;
       
        
        public MatchPosition currentPosition;
        private int definitionType;
        public List<string> defList = new List<string>();
        private bool isMultiDefinition = false;
        private bool isWaitingForTransitive = false;
        private bool isWaitingForIntransitve = false;
        private int countVerb = 0;
            
        public MWCMatcher()
        {
            isComplete = false;
            isValideWord = true;
            isTransitive = false;
            isNeedRead = true;


            function = "";
            etymology = "";
            inflectform = "";
            date = "";
            currentPosition = MatchPosition.MATCH_WORD;
            defList.Clear();
        }


        public void matchLine(string strLine)
        {
            if (strLine.StartsWith("韦氏"))
            {
                currentPosition = MatchPosition.MATCH_WORD;
                return;
            }
            if (!isValideWord)
                return;
            switch (currentPosition)
            {
                case MatchPosition.MATCH_WORD:
                    matchWordName(strLine);break;
                case MatchPosition.MATCH_PROPERTY:
                    matchProperty(strLine);break;
                case MatchPosition.MATCH_DEFINITION:
                    matchDefinition(strLine);break;
            }
        }

        public bool matchWordName(string strLine)
        {
            isComplete = false;
            Regex wordPattern = new Regex("^\\s?([a-zA-Z1-9]+)\\s*\\d?\\s*(\\w+\\.)?$");
            if (wordPattern.IsMatch(strLine))
            {
                Regex wordPattern1 = new Regex("^\\s*\\d(\\w+)\\s+\\d");
                Regex wordPattern2 = new Regex("^\\s*(\\w+)\\s+\\d");
               
                if (wordPattern1.IsMatch(strLine))
                {
                    word_name = wordPattern1.Match(strLine).Groups[1].Value;
                }else if(wordPattern2.IsMatch(strLine))
                {
                    word_name = wordPattern2.Match(strLine).Groups[1].Value;
                    
                }else 
                {
                    word_name = wordPattern.Match(strLine).Groups[1].Value;
                    
                }

                isValideWord = true;
                currentPosition = MatchPosition.MATCH_PROPERTY;
                return true;
            }
            else
                return false;
        }

        public bool matchProperty(string strLine)
        {
            Regex propertyPattern = new Regex("^[PEFIDVU][\\w\\s]+:\\s*.*\\s*$");
            if (!propertyPattern.IsMatch(strLine))
            {
                currentPosition = MatchPosition.MATCH_DEFINITION;
                return matchDefinition(strLine);
            }
            Regex functionPattern = new Regex("^Function:\\s*(.*)\\s*$");
            if (functionPattern.IsMatch(strLine))
            {
                function = functionPattern.Match(strLine).Groups[1].Value;
                return true;
            }
            Regex datePattern = new Regex("^Date:\\s*(.*)\\s*$");
            if (datePattern.IsMatch(strLine))
            {
                date = datePattern.Match(strLine).Groups[1].Value;
                return true;
            }
            Regex etymologyPattern = new Regex("^Etymology:\\s*(.*)\\s*$");
            if (etymologyPattern.IsMatch(strLine))
            {
                etymology = etymologyPattern.Match(strLine).Groups[1].Value;
                return true;
            }
            Regex inflectPattern = new Regex("^Inflected Form:\\s*(.*)\\s*$");
            if (inflectPattern.IsMatch(strLine))
            {
                inflectform = inflectPattern.Match(strLine).Groups[1].Value;
                return true;
            }
            return true;
        }

        public bool matchDefinition(string strLine)
        {
            Regex matchDef1 = new Regex("^[―:a-zA-Z]+.*");
            Regex matchDef2 = new Regex("^[0-9]+\\s+(.*)");
            Regex matchDefOften1 = new Regex("^often\\s+\\w+\\s*$");
            Regex matchDefOften2 = new Regex("^often.*(:.*)\\s*$");
            Regex matchTransitive1 = new Regex("^transitive verb\\s*(.*:.*)$");
            Regex matchTransitive2 = new Regex("^transitive verb\\s*$");
            Regex matchIntransitive1 = new Regex("^intransitive verb\\s*(.*:.*)$");
            Regex matchIntransitive2 = new Regex("^intransitive verb\\s*$");

            //MATCH OFTEN USAGE
            if(matchDefOften1.IsMatch(strLine))
            {
                isMultiDefinition = true;
                return true;
            }else if(matchDefOften2.IsMatch(strLine))
            {
                isMultiDefinition = false;
                defList.Add(matchDefOften2.Match(strLine).Groups[1].Value);
                isComplete = true;
                isNeedRead = false;
                return true;
            }

            //MATCH TRANSITIVE VERB
            if (!isWaitingForIntransitve && matchTransitive1.IsMatch(strLine))  // TRANSITIVE WITH ONLY ONE DEFINITION
            {
                if (defList.Count > 0)
                {
                    isIntransitive = true;
                    isNeedRead = true;
                    countVerb++;
                    return true;
                }
                isMultiDefinition = false;
                defList.Clear();
                defList.Add(matchTransitive1.Match(strLine).Groups[1].Value);
                isTransitive = true;
                isIntransitive = false;
                function = "transitive verb";
                isNeedRead = false;
                countVerb++;
                if (countVerb == 2)
                    isComplete = true;
                return true;
            }else if(!isWaitingForIntransitve && matchTransitive2.IsMatch(strLine))  //MULTI TRANSITIVIE DEFINITION
            {
                if (defList.Count > 0)
                {
                    isIntransitive = true;
                    isNeedRead = true;
                    countVerb++;
                    return true;
                }
                isMultiDefinition = true;
                isTransitive = false;
                isIntransitive = false;
                isWaitingForTransitive = true;
                function = "transitive verb";
                defList.Clear();
                countVerb++;
                return true;
            }

            //MATCH INTRANSITIVE VERB
            if (!isWaitingForTransitive && matchIntransitive1.IsMatch(strLine))
            {
                if (defList.Count > 0)
                {
                    isTransitive = true;
                    isNeedRead = true;
                    countVerb ++;
                    return true;
                }
                isMultiDefinition = false;
                defList.Clear();
                defList.Add(matchIntransitive1.Match(strLine).Groups[1].Value);
                function = "intransitive verb";
                isIntransitive = true;
                isTransitive = false;
               
                isNeedRead = false;
                countVerb++;
                if (countVerb == 2)
                    isComplete = true;
                return true;
            }
                // MULTI INTRANSITIVE DEFINITION
            else if(!isWaitingForTransitive && matchIntransitive2.IsMatch(strLine))
            {
                if (defList.Count > 0)
                {
                    isTransitive = true;
                    isNeedRead = true;
                    countVerb++;
                    return true;
                }
                isMultiDefinition = true;
                isIntransitive = false;
                isTransitive = false;
                isWaitingForIntransitve = true;
                function = "intransitive verb";
                defList.Clear();
                countVerb++;
                return true;
            }


            //MATCH DEFINITION
            if (matchDef1.IsMatch(strLine) && !isMultiDefinition)
            {
                defList.Add(matchDef1.Match(strLine).Value);
                currentPosition = MatchPosition.MATCH_WORD;
                isComplete = true;
                isNeedRead = false;
            }
            else if (matchDef2.IsMatch(strLine))
            {
                isMultiDefinition = true;
                defList.Add(matchDef2.Match(strLine).Groups[1].Value);
            }
            else if (isMultiDefinition && defList.Count > 0)
            {
                if (isWaitingForTransitive)
                {
                    isTransitive = true;
                    isWaitingForTransitive = false;
                    if (countVerb == 2)
                        isComplete = true;
                    else
                        isComplete = false;

                }
                else if (isWaitingForIntransitve)
                {
                    isIntransitive = true;
                    isWaitingForIntransitve = false;
                    if (countVerb == 2)
                        isComplete = true;
                    else
                        isComplete = false;
                }
                else
                {
                    currentPosition = MatchPosition.MATCH_WORD;
                    isComplete = true;
                }
                //    matchWordName(strLine);
            }
            else if (defList.Count != 0)
                currentPosition = MatchPosition.MATCH_WORD;
            return true;
        }
    }
}
