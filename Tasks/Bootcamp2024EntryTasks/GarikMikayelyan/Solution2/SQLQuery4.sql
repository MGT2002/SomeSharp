With CTE_PopularityTable as (
	select bl.Id LoanID, DATEDIFF(day, LoanDate, ReturnDate)/count(l.Title) PopularityPoints, b.Id BookID
	from BookLoans bl
	join BookCopies bc on bc.Id = bl.BookCopyId
	join Books b on b.Id = bc.BookId
	join LiteraryWorks_Books lb on lb.BookId = b.Id
	join LiteraryWorks l on l.Id = lb.LiteraryWorkId
	group by bl.Id, bl.LoanDate, bl.ReturnDate, b.Id
),
CTE_LiteraryWorkPopularity as (
	select a.Id AuthorID, a.Name AuthorName, l.Title LiteraryTitle, sum(PopularityPoints) Points
	from CTE_PopularityTable
	join LiteraryWorks_Books lb on lb.BookId = CTE_PopularityTable.BookID
	join LiteraryWorks l on lb.LiteraryWorkId = l.Id
	join Authors a on a.Id = l.AuthorId
	group by l.Title, a.Name, a.Id
),
CTE_RankedLWP as (
	select l.AuthorID, l.AuthorName, l.Points, '"' + l.LiteraryTitle + '"' as LiteraryTitle,
	ROW_NUMBER() over (partition by authorname order by points desc) as Rank
	from CTE_LiteraryWorkPopularity l
),
CTE_AuthorTop3 as (
	select t.AuthorID, t.AuthorName, STRING_AGG(t.LiteraryTitle, ', ') as Top3
	from CTE_RankedLWP t
	where t.Rank <= 3
	group by t.AuthorName, t.AuthorID
),
CTE_AuthorPopularity as (
	select AuthorID, AuthorName, sum(Points) Sum
	from CTE_LiteraryWorkPopularity 
	group by AuthorID, AuthorName
)

select t.AuthorName, ap.Sum CountAll, t.Top3 from CTE_AuthorTop3 t
join CTE_AuthorPopularity ap on ap.AuthorID = t.AuthorID
order by ap.Sum desc
