SELECT a.Name [AuthorName], count(*) [Count]
from authors a
inner join LiteraryWorks l
on a.Id = l.AuthorId
group by a.Name
order by count(*) desc;