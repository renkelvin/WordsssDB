SELECT word_name,word_dict.word_dict_id 
FROM `word_test`.`word`,word_test.word_dict,word_test.mcec_dict_word 
where word_name = 'post'
and word_dict.word_id = word.word_id 
and mcec_dict_word.mcec_dict_word_id = word_dict.mcec_dict_word_id;