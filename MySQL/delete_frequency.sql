delete from word.word
where not exists
(select * from word.frequency where frequency.word_id = word.word_id
)