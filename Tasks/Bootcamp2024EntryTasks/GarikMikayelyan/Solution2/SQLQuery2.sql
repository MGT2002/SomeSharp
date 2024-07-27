Select r.FirstName, r.LastName, count(distinct lw.Title) Count
from Readers r
join BookLoans bl on bl.ReaderId = r.Id
join BookCopies bc on bl.BookCopyId = bc.Id
join Books b on bc.BookId = b.Id
join LiteraryWorks_Books lb on b.Id = lb.BookId
join LiteraryWorks lw on lb.LiteraryWorkId = lw.Id
group by r.FirstName, r.LastName
order by Count desc