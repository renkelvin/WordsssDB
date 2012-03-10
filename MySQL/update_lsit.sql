update word.list 
set list.count = (
    select count(cs_list_word_id) from word.cs_list_word
    )
where list_id = 1;

update word.list 
set list.count = (
    select count(ma_list_word_id) from word.ma_list_word
    )
where list_id = 2;

update word.list 
set list.count = (
    select count(ph_list_word_id) from word.ph_list_word
    )
where list_id = 3;

update word.list
set list.count = (
    select count(gre_list_word_id) from word.gre_list_word
)
where list_id = 4;
