-- See what we're starting from
SELECT * FROM Apple
SELECT * FROM ApplesConsumed

-- We want a table that has apple variety name and the number of times it has been eaten, for a specific user
-- ApplesConsumed has the info we need except the actual apple variety name is over in the apple table
-- so we need info from 2 different tables plus a group by to tally up the counts

-- Manually written
SELECT VarietyName, TotalCount from Apple INNER JOIN 
(SELECT AppleID,SUM(Count) as TotalCount from ApplesConsumed where FujiUserID = 1 GROUP BY AppleID) AS acg
ON acg.AppleID = Apple.ID

-- Using this LINQ
/*
ApplesConsumeds.Where(ac => ac.FujiUserId == 1)
	.GroupBy(x => x.AppleId)
	.Select(g => new {AppleId = g.Key, Total = g.Sum(x => x.Count)})
	.Join(Apples,ac => ac.AppleId,a => a.Id, (ac,a) => new {VarietyName = a.VarietyName, TotalConsumed = ac.Total})
*/
-- it's transformed into this

SELECT [a0].[VarietyName], [t].[Total] AS [TotalConsumed]
FROM (
    SELECT [a].[AppleID] AS [AppleId], COALESCE(SUM([a].[Count]), 0) AS [Total]
    FROM [ApplesConsumed] AS [a]
    WHERE [a].[FujiUserID] = 1
    GROUP BY [a].[AppleID]
) AS [t]
INNER JOIN [Apple] AS [a0] ON [t].[AppleId] = [a0].[ID]

-- Apparently LINQ cannot translate the following completely to SQL and will do this in memory
-- UPDATE: apparently it can.  After upgrading and fixing LINQPad 7, the following translates well
-- This is how I orignally wrote it, as this fits how I think about the problem.  The manual LINQ Join above is pretty ugly
/*var apples = _context.ApplesConsumeds
    .Where(ac => ac.FujiUser == fu)
    .Select(ac => new
    {
        VarietyName = ac.Apple.VarietyName,
        Count = ac.Count
    })           
    .GroupBy(ac => ac.VarietyName)
    .Select(g => new
    {
        VarietyName = g.Key,
        Total = g.Sum(x => x.Count)
    });
    */
    -- translates nicely to

SELECT [a0].[VarietyName], COALESCE(SUM([a].[Count]), 0) AS [Total]
FROM [ApplesConsumed] AS [a]
INNER JOIN [Apple] AS [a0] ON [a].[AppleID] = [a0].[ID]
WHERE [a].[FujiUserID] = 1
GROUP BY [a0].[VarietyName]

-- except there is something non-ideal about this one.  Anybody see it compared to the earlier ones?