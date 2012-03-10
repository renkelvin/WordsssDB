SELECT word_name FROM `word_test`.`word`,word_test.word_dict where word_name regexp '[],-./\';:?(]' 
and word_dict.word_id = word.word_id and word_dict.mcec_dict_word_id != 'null';