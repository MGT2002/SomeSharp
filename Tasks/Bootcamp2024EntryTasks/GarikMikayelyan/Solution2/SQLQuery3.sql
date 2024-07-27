select CommonAndIndividualLiterary.Title, a.Name, CommonAndIndividualLiterary.YearOfWriting 
from (select lw.Title, lw.AuthorId, lw.YearOfWriting,
	count(distinct b.Title) Count
	from LiteraryWorks lw
	join LiteraryWorks_Books lb on lb.LiteraryWorkId = lw.Id
	join Books b on b.Id = lb.BookId
	group by lw.Title, lw.AuthorId, lw.YearOfWriting
	having Count(distinct b.Title) > 1) as CommonAndIndividualLiterary
join Authors a on a.Id = CommonAndIndividualLiterary.AuthorId
order by a.Name, CommonAndIndividualLiterary.YearOfWriting