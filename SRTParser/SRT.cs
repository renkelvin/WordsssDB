using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace SRTParser
{
    class SRT
    {
        private int _season;
        private int _episode;
        private string _current_sentence;
        private string _current_word;
        private long _begin_time;
        private long _end_time;

        private bool sentence_begin = false;
        private StreamWriter _writer;
        private FileStream _fs;

        public SRT(int season, int episode)
        {
            _season = season;
            _episode = episode;
            _fs = new FileStream("srt.txt", FileMode.Create);
            _writer = new StreamWriter(_fs);
        }

        private void WriteToDB()
        {
            _writer.WriteLine(_current_word) ;
        }

        public void ParseLine(string strLine)
        {
            Regex match_time = new Regex("(\\d+):(\\d+):(\\d+),(\\d+) --> (\\d+):(\\d+):(\\d+),(\\d+)");
           
            if (match_time.IsMatch(strLine))
            {
                Match time_match = match_time.Match(strLine);
                int hour = int.Parse(time_match.Groups[1].Value);
                int minute = int.Parse(time_match.Groups[2].Value);
                int second = int.Parse(time_match.Groups[3].Value);
                int ms = int.Parse(time_match.Groups[4].Value);
                _begin_time = hour * 60 + minute * 60 + second * 1000 + ms;
                hour = int.Parse(time_match.Groups[5].Value);
                minute = int.Parse(time_match.Groups[6].Value);
                second = int.Parse(time_match.Groups[7].Value);
                ms = int.Parse(time_match.Groups[8].Value);
                _end_time = hour * 60 + minute * 60 + second * 1000 + ms;
                sentence_begin = true;
                
            }
            else if (strLine == "")
            {
                sentence_begin = false;
                return;
            }
            else if (sentence_begin)
            {
                _current_sentence = strLine;
                ParseWord();
            }
        }

        public void ParseWord()
        {
            Regex match_word = new Regex("[\\w']+");
            
            MatchCollection word_match = match_word.Matches(_current_sentence);

            _writer.WriteLine(_current_sentence);
            _writer.WriteLine(_begin_time + "->" + _end_time);

            foreach(Match match in word_match)
            {
                string word = match.Value;
                _current_word = word.ToLower();
                WriteToDB();
            }
        }

        public void Close()
        {
            _writer.Close();
            _fs.Close();
        }
    }
}
