using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SQL_Queries
/// </summary>
public class ghQueries
{
	public ghQueries()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    static public String dashboard_total_stats()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText = @"
SELECT
COUNT([i].[interactionid]) [count]
,CONVERT(varchar,MIN([i].[datestart]),101) [datestart]
,CONVERT(varchar,MAX([i].[datestart]),101) [dateend]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
WHERE [i].[companyid] = @sp_companyid
";

        #endregion Build cmdText
        return cmdText;
    }
    static public String dashboard_total_stats_dynamic(bool campaigns, bool skills, bool agents)
    {
        #region Build cmdText
        String cmdText = "";
        cmdText = @"
                SELECT
                COUNT([i].[interactionid]) [count]
                ,CONVERT(varchar,MIN([i].[datestart]),101) [datestart]
                ,CONVERT(varchar,MAX([i].[datestart]),101) [dateend]
                FROM [dbo].[interactions] [i] WITH(NOLOCK)
                ";
        // 
        if (campaigns || skills || agents)
        {
            cmdText += "JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]\r";
        }
        cmdText += @"
                WHERE [i].[companyid] = @sp_companyid
                ";
        #region Dynamic
        if (campaigns)
        {
            cmdText += "AND [fc].[campaignid] IN (SELECT [spc].[itemid] FROM @sp_campaigns [spc])\r";
            // cmdText += @"AND [fc].[campaignid] IN (SELECT [spc].[campaignid] FROM @sp_campaigns [spc])";
        }
        if (skills)
        {
            cmdText += "AND [fc].[skillid] IN (SELECT [spc].[itemid] FROM @sp_skills [spc])\r";
        }
        if (agents)
        {
            cmdText += @"
                        AND [i].[interactionid] IN (
	                        SELECT
	                        [fca].[interactionid]
	                        FROM [dbo].[five9_call_agent] [fca] WITH(NOLOCK)
	                        WHERE 1=1
	                        AND [fca].[companyid] = [i].[companyid]
	                        AND [fca].[interactionid] = [i].[interactionid]
	                        AND [fca].[agentid] IN (SELECT [spc].[itemid] FROM @sp_agents [spc])
	                        GROUP BY [fca].[interactionid]
                        )
                        ";
        }
        #endregion Dynamic

        #endregion Build cmdText
        return cmdText;
    }
    static public String dashboard_skill_count()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
SELECT
[fis].[name] [skill]
,COUNT(DISTINCT([i].[interactionid])) [total_calls]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_item] [fis] WITH(NOLOCK) ON [fis].[itemid] = [fc].[skillid] AND [fis].[typeid] = 102000000
WHERE 1=1
AND [i].[companyid] = @sp_companyid
AND [i].[datestart] BETWEEN @sp_date_start AND @sp_date_end
AND [fc].[campaignid] IN (SELECT [spc].[campaignid] FROM @sp_campaigns [spc])
GROUP BY [fis].[name]
ORDER BY [fis].[name]
                        ";
        #endregion Build cmdText
        return cmdText;
    }
    static public String dashboard_skill_count_dynamic(bool campaigns, bool skills, bool agents)
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
SELECT
[fis].[name] [skill]
,COUNT(DISTINCT([i].[interactionid])) [total_calls]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_item] [fis] WITH(NOLOCK) ON [fis].[itemid] = [fc].[skillid] AND [fis].[typeid] = 102000000
WHERE 1=1
AND [i].[companyid] = @sp_companyid
AND [i].[datestart] BETWEEN @sp_date_start AND @sp_date_end
                        ";
        #region Dynamic
        if (campaigns)
        {
            cmdText += "AND [fc].[campaignid] IN (SELECT [spc].[itemid] FROM @sp_campaigns [spc])\r";
            // cmdText += @"AND [fc].[campaignid] IN (SELECT [spc].[campaignid] FROM @sp_campaigns [spc])";
        }
        if (skills)
        {
            cmdText += "AND [fc].[skillid] IN (SELECT [spc].[itemid] FROM @sp_skills [spc])\r";
        }
        if (agents)
        {
            cmdText += @"
                        AND [i].[interactionid] IN (
	                        SELECT
	                        [fca].[interactionid]
	                        FROM [dbo].[five9_call_agent] [fca] WITH(NOLOCK)
	                        WHERE 1=1
	                        AND [fca].[companyid] = [i].[companyid]
	                        AND [fca].[interactionid] = [i].[interactionid]
	                        AND [fca].[agentid] IN (SELECT [spc].[itemid] FROM @sp_agents [spc])
	                        GROUP BY [fca].[interactionid]
                        )
                        ";
        }
        #endregion Dynamic
        cmdText += @"
GROUP BY [fis].[name]
ORDER BY [fis].[name]
";
        cmdText += "\r";

        #endregion Build cmdText
        return cmdText;
    }
    static public String dashboard_call_by_hour()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
DECLARE @Time1 DATETIME, @Time2 DATETIME
SET @Time1 = '00:00:00'
SET @Time2 = '23:59:00'


DECLARE @tmp_Hours TABLE ([hour] varchar(25), [count] bigint)
WHILE @Time1 <= @Time2
BEGIN
	INSERT INTO @tmp_Hours SELECT CONVERT(varchar,@Time1,108), 0
	SET @Time1 = DATEADD(n,60,@Time1)
END


SELECT
[h].[hour] [Hour], LEFT([h].[hour],2) [HourShort], ISNULL([r].[total_calls],0) [count],ISNULL([r].[avg_queuetime],0) [avgQueueTime] --, [r].[avgQueueTime2]
FROM @tmp_Hours [h]
LEFT OUTER JOIN (
			SELECT
			CASE
				WHEN DATEPART(mi,DATEADD(hh,-@sp_offset,[i].[datestart])) < 60 THEN CONVERT(varchar,DATEADD(hh, DATEDIFF(hh,0, DATEADD(hh,-@sp_offset,[i].[datestart]))+0, 0),108)
				ELSE CONVERT(varchar,DATEADD(mi,60,DATEADD(hh, DATEDIFF(hh,0, DATEADD(hh,-@sp_offset,[i].[datestart]))+0, 0)),108)
			END [Hour]
			,COUNT(DISTINCT([i].[interactionid])) [total_calls]
			,AVG(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[queue_time] ELSE NULL END) [avg_queuetime]
            --,(ROUND(AVG(CAST(((CASE WHEN [c].[disposition] <> 'Abandon' AND [c].[QueueTime] > 1000 THEN [c].[QueueTime] WHEN [c].[disposition] <> 'Abandon' AND [c].[QueueTime] < 1000 THEN [c].[QueueTime] * 1000 END)) AS BIGINT)),-3)/1000) [avgQueueTime]
            --,[dbo].[ConvertMilliSecondsToHHMMss] (AVG(CAST(((CASE WHEN [c].[disposition] <> 'Abandon' AND [c].[QueueTime] > 1000 THEN [c].[QueueTime] WHEN [c].[disposition] <> 'Abandon' AND [c].[QueueTime] < 1000 THEN [c].[QueueTime] * 1000 END)) AS BIGINT))) [avgQueueTime2]
			FROM [dbo].[interactions] [i] WITH(NOLOCK)
			JOIN [dbo].[interactions_five9] [if] WITH(NOLOCK) ON [if].[companyid] = [i].[companyid] AND [if].[interactionid] = [i].[interactionid]
			JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
			JOIN [dbo].[five9_call_counts] [fcc] ON [fcc].[companyid] = [i].[companyid] AND [fcc].[interactionid] = [i].[interactionid]
			JOIN [dbo].[five9_call_time] [fct] ON [fct].[companyid] = [i].[companyid] AND [fct].[interactionid] = [i].[interactionid]
			WHERE 1=1
			AND [i].[companyid] = @sp_companyid
			AND [i].[datestart] BETWEEN @sp_date_start AND @sp_date_end
			AND [fc].[campaignid] IN (SELECT [spc].[campaignid] FROM @sp_campaigns [spc])
			GROUP BY
					CASE
						WHEN DATEPART(mi,DATEADD(hh,-@sp_offset,[i].[datestart])) < 60 THEN CONVERT(varchar,DATEADD(hh, DATEDIFF(hh, 0, DATEADD(hh,-@sp_offset,[i].[datestart]))+0, 0),108)
						ELSE CONVERT(varchar,DATEADD(mi,60,DATEADD(hh, DATEDIFF(hh, 0, DATEADD(hh,-@sp_offset,[i].[datestart]))+0, 0)),108)
					END
) [r] ON [h].[hour] = [r].[hour]
ORDER BY [h].[hour]
                            ";
        #endregion Build cmdText
        return cmdText;
    }
    static public String dashboard_call_by_hour_dynamic(bool campaigns, bool skills, bool agents)
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
DECLARE @Time1 DATETIME, @Time2 DATETIME
SET @Time1 = '00:00:00'
SET @Time2 = '23:59:00'


DECLARE @tmp_Hours TABLE ([hour] varchar(25), [count] bigint)
WHILE @Time1 <= @Time2
BEGIN
	INSERT INTO @tmp_Hours SELECT CONVERT(varchar,@Time1,108), 0
	SET @Time1 = DATEADD(n,60,@Time1)
END


SELECT
[h].[hour] [Hour], LEFT([h].[hour],2) [HourShort], ISNULL([r].[total_calls],0) [count],ISNULL([r].[avg_queuetime],0) [avgQueueTime] --, [r].[avgQueueTime2]
FROM @tmp_Hours [h]
LEFT OUTER JOIN (
			SELECT
			CASE
				WHEN DATEPART(mi,DATEADD(hh,-@sp_offset,[i].[datestart])) < 60 THEN CONVERT(varchar,DATEADD(hh, DATEDIFF(hh,0, DATEADD(hh,-@sp_offset,[i].[datestart]))+0, 0),108)
				ELSE CONVERT(varchar,DATEADD(mi,60,DATEADD(hh, DATEDIFF(hh,0, DATEADD(hh,-@sp_offset,[i].[datestart]))+0, 0)),108)
			END [Hour]
			,COUNT(DISTINCT([i].[interactionid])) [total_calls]
			,AVG(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[queue_time] ELSE NULL END) [avg_queuetime]
            --,(ROUND(AVG(CAST(((CASE WHEN [c].[disposition] <> 'Abandon' AND [c].[QueueTime] > 1000 THEN [c].[QueueTime] WHEN [c].[disposition] <> 'Abandon' AND [c].[QueueTime] < 1000 THEN [c].[QueueTime] * 1000 END)) AS BIGINT)),-3)/1000) [avgQueueTime]
            --,[dbo].[ConvertMilliSecondsToHHMMss] (AVG(CAST(((CASE WHEN [c].[disposition] <> 'Abandon' AND [c].[QueueTime] > 1000 THEN [c].[QueueTime] WHEN [c].[disposition] <> 'Abandon' AND [c].[QueueTime] < 1000 THEN [c].[QueueTime] * 1000 END)) AS BIGINT))) [avgQueueTime2]
			FROM [dbo].[interactions] [i] WITH(NOLOCK)
			JOIN [dbo].[interactions_five9] [if] WITH(NOLOCK) ON [if].[companyid] = [i].[companyid] AND [if].[interactionid] = [i].[interactionid]
			JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
			JOIN [dbo].[five9_call_counts] [fcc] ON [fcc].[companyid] = [i].[companyid] AND [fcc].[interactionid] = [i].[interactionid]
			JOIN [dbo].[five9_call_time] [fct] ON [fct].[companyid] = [i].[companyid] AND [fct].[interactionid] = [i].[interactionid]
			WHERE 1=1
			AND [i].[companyid] = @sp_companyid
			AND [i].[datestart] BETWEEN @sp_date_start AND @sp_date_end
                            ";

        #region Dynamic
        if (campaigns)
        {
            cmdText += "AND [fc].[campaignid] IN (SELECT [spc].[itemid] FROM @sp_campaigns [spc])\r";
            // cmdText += @"AND [fc].[campaignid] IN (SELECT [spc].[campaignid] FROM @sp_campaigns [spc])";
        }
        if (skills)
        {
            cmdText += "AND [fc].[skillid] IN (SELECT [spc].[itemid] FROM @sp_skills [spc])\r";
        }
        if (agents)
        {
            cmdText += @"
                        AND [i].[interactionid] IN (
	                        SELECT
	                        [fca].[interactionid]
	                        FROM [dbo].[five9_call_agent] [fca] WITH(NOLOCK)
	                        WHERE 1=1
	                        AND [fca].[companyid] = [i].[companyid]
	                        AND [fca].[interactionid] = [i].[interactionid]
	                        AND [fca].[agentid] IN (SELECT [spc].[itemid] FROM @sp_agents [spc])
	                        GROUP BY [fca].[interactionid]
                        )
                        ";
        }
        #endregion Dynamic
        cmdText += @"
			GROUP BY
					CASE
						WHEN DATEPART(mi,DATEADD(hh,-@sp_offset,[i].[datestart])) < 60 THEN CONVERT(varchar,DATEADD(hh, DATEDIFF(hh, 0, DATEADD(hh,-@sp_offset,[i].[datestart]))+0, 0),108)
						ELSE CONVERT(varchar,DATEADD(mi,60,DATEADD(hh, DATEDIFF(hh, 0, DATEADD(hh,-@sp_offset,[i].[datestart]))+0, 0)),108)
					END
) [r] ON [h].[hour] = [r].[hour]
ORDER BY [h].[hour]
";
        cmdText += @"";
        cmdText += "\r";

        #endregion Build cmdText
        return cmdText;
    }
    static public String dashboard_call_counts()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText = "";
        cmdText += @"
SELECT
COUNT([i].[interactionid]) [count]
,COUNT(DISTINCT([i].[interactionid])) [total_calls]
,COUNT(CASE WHEN [fcc].[contacted] = 1 THEN 1 ELSE NULL END) [contacted]
--,COUNT(CASE WHEN [fct].[talk_time] > 0 THEN 1 ELSE NULL END) [talk]
--,COUNT(CASE WHEN [fct].[ring_time] > 0 THEN 1 ELSE NULL END) [ring]
--,COUNT(CASE WHEN [fct].[queue_time] > 0 THEN 1 ELSE NULL END) [queue]
,COUNT(CASE WHEN [fcc].[abandoned] = 1 THEN 1 ELSE NULL END) [abandoned]

,COUNT(CASE WHEN [fcc].[abandoned] = 0 AND [fct].[queue_time] < 90 THEN 1 ELSE NULL END) [answered_90]
,COUNT(CASE WHEN [fcc].[abandoned] = 0 AND [fct].[queue_time] >= 90 AND [fct].[queue_time] <= 120 THEN 1 ELSE NULL END) [answered_120]
,COUNT(CASE WHEN [fcc].[abandoned] = 0 AND [fct].[queue_time] > 120 THEN 1 ELSE NULL END) [answered_120p]

,COUNT(CASE WHEN [fcc].[abandoned] = 1 AND [fct].[queue_time] < 90 THEN 1 ELSE NULL END) [abandon_90]
,COUNT(CASE WHEN [fcc].[abandoned] = 1 AND [fct].[queue_time] >= 90 AND [fct].[queue_time] <= 120 THEN 1 ELSE NULL END) [abandon_120]
,COUNT(CASE WHEN [fcc].[abandoned] = 1 AND [fct].[queue_time] > 120 THEN 1 ELSE NULL END) [abandon_120p]


,SUM(CASE WHEN [fcc].[abandoned] = 1 THEN [fct].[queue_time] ELSE NULL END) [abandon_total]
,AVG(CASE WHEN [fcc].[abandoned] = 1 THEN [fct].[queue_time] ELSE NULL END) [abandon_avg]
--,AVG(CASE WHEN [fcc].[abandoned] = 1 THEN [fct].[queue_time] ELSE NULL END) [avg_abandontime]

,SUM(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[queue_time] ELSE NULL END) [answer_total]
,AVG(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[queue_time] ELSE NULL END) [answer_avg]

,SUM([fct].[wrapup_time]) [wrapup_total]
,AVG([fct].[wrapup_time]) [wrapup_avg]

,SUM(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[talk_time] ELSE NULL END) [talk_total]
,AVG(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[talk_time] ELSE NULL END) [talk_avg]
--,AVG(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[talk_time] ELSE NULL END) [avg_talktime]

,SUM([fct].[queue_time]) [queue_total]
--,SUM(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[queue_time] ELSE NULL END) [total_queuetime]
,AVG([fct].[queue_time]) [queue_avg]
--,AVG(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[queue_time] ELSE NULL END) [avg_queuetime]

,SUM([fct].[ivr_time]) [ivr_total]
,AVG([fct].[ivr_time]) [ivr_avg]

,SUM(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[handle_time] ELSE NULL END) [handle_total]
,AVG(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[handle_time] ELSE NULL END) [handle_avg]


,SUM([fct].[thirdparty_time]) [third_total]
,AVG([fct].[thirdparty_time]) [third_avg]

--,AVG(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[ring_time] ELSE NULL END) [avg_ringtime]


FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_call_counts] [fcc] ON [fcc].[companyid] = [i].[companyid] AND [fcc].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_call_time] [fct] ON [fct].[companyid] = [i].[companyid] AND [fct].[interactionid] = [i].[interactionid]
WHERE 1=1
AND [i].[companyid] = @sp_companyid
AND [i].[datestart] BETWEEN @sp_date_start AND @sp_date_end
AND [fc].[campaignid] IN (SELECT [spc].[campaignid] FROM @sp_campaigns [spc])
                        ";
        cmdText += "\r";
        cmdText += "\r";
        cmdText += "";
        #endregion Build cmdText
        return cmdText;
    }
    static public String dashboard_call_counts_dynamic(bool campaigns, bool skills, bool agents)
    {
        #region Build cmdText
        String cmdText = "";
        cmdText = "";
        cmdText += @"
                    SELECT
                    COUNT([i].[interactionid]) [count]
                    ,COUNT(DISTINCT([i].[interactionid])) [total_calls]

                    ,CONVERT(varchar,MIN(DATEADD(hh,-@sp_offset,[i].[datestart])),101) [datestart]
                    ,CONVERT(varchar,MAX(DATEADD(hh,-@sp_offset,[i].[datestart])),101) [dateend]



                    ,COUNT(CASE WHEN [fcc].[contacted] = 1 THEN 1 ELSE NULL END) [contacted]
                    --,COUNT(CASE WHEN [fct].[talk_time] > 0 THEN 1 ELSE NULL END) [talk]
                    --,COUNT(CASE WHEN [fct].[ring_time] > 0 THEN 1 ELSE NULL END) [ring]
                    --,COUNT(CASE WHEN [fct].[queue_time] > 0 THEN 1 ELSE NULL END) [queue]
                    ,COUNT(CASE WHEN [fcc].[abandoned] = 1 THEN 1 ELSE NULL END) [abandoned]

                    ,COUNT(CASE WHEN [fcc].[abandoned] = 0 AND [fct].[queue_time] < 90 THEN 1 ELSE NULL END) [answered_90]
                    ,COUNT(CASE WHEN [fcc].[abandoned] = 0 AND [fct].[queue_time] >= 90 AND [fct].[queue_time] <= 120 THEN 1 ELSE NULL END) [answered_120]
                    ,COUNT(CASE WHEN [fcc].[abandoned] = 0 AND [fct].[queue_time] > 120 THEN 1 ELSE NULL END) [answered_120p]

                    ,COUNT(CASE WHEN [fcc].[abandoned] = 1 AND [fct].[queue_time] < 90 THEN 1 ELSE NULL END) [abandon_90]
                    ,COUNT(CASE WHEN [fcc].[abandoned] = 1 AND [fct].[queue_time] >= 90 AND [fct].[queue_time] <= 120 THEN 1 ELSE NULL END) [abandon_120]
                    ,COUNT(CASE WHEN [fcc].[abandoned] = 1 AND [fct].[queue_time] > 120 THEN 1 ELSE NULL END) [abandon_120p]


                    ,SUM(CASE WHEN [fcc].[abandoned] = 1 THEN [fct].[queue_time] ELSE NULL END) [abandon_total]
                    ,AVG(CASE WHEN [fcc].[abandoned] = 1 THEN [fct].[queue_time] ELSE NULL END) [abandon_avg]
                    --,AVG(CASE WHEN [fcc].[abandoned] = 1 THEN [fct].[queue_time] ELSE NULL END) [avg_abandontime]

                    ,SUM(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[queue_time] ELSE NULL END) [answer_total]
                    ,AVG(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[queue_time] ELSE NULL END) [answer_avg]

                    ,SUM([fct].[wrapup_time]) [wrapup_total]
                    ,AVG([fct].[wrapup_time]) [wrapup_avg]

                    ,SUM(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[talk_time] ELSE NULL END) [talk_total]
                    ,AVG(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[talk_time] ELSE NULL END) [talk_avg]
                    --,AVG(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[talk_time] ELSE NULL END) [avg_talktime]

                    ,SUM([fct].[queue_time]) [queue_total]
                    --,SUM(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[queue_time] ELSE NULL END) [total_queuetime]
                    ,AVG([fct].[queue_time]) [queue_avg]
                    --,AVG(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[queue_time] ELSE NULL END) [avg_queuetime]

                    ,SUM([fct].[ivr_time]) [ivr_total]
                    ,AVG([fct].[ivr_time]) [ivr_avg]

                    ,SUM(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[handle_time] ELSE NULL END) [handle_total]
                    ,AVG(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[handle_time] ELSE NULL END) [handle_avg]


                    ,SUM([fct].[thirdparty_time]) [third_total]
                    ,AVG([fct].[thirdparty_time]) [third_avg]

                    --,AVG(CASE WHEN [fcc].[abandoned] = 0 THEN [fct].[ring_time] ELSE NULL END) [avg_ringtime]


                    FROM [dbo].[interactions] [i] WITH(NOLOCK)
                    JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
                    JOIN [dbo].[five9_call_counts] [fcc] ON [fcc].[companyid] = [i].[companyid] AND [fcc].[interactionid] = [i].[interactionid]
                    JOIN [dbo].[five9_call_time] [fct] ON [fct].[companyid] = [i].[companyid] AND [fct].[interactionid] = [i].[interactionid]
                    WHERE 1=1
                    AND [i].[companyid] = @sp_companyid
                    AND [i].[datestart] BETWEEN @sp_date_start AND @sp_date_end
                        ";
        #region Dynamic
        if (campaigns)
        {
            cmdText += "AND [fc].[campaignid] IN (SELECT [spc].[itemid] FROM @sp_campaigns [spc])\r";
            // cmdText += @"AND [fc].[campaignid] IN (SELECT [spc].[campaignid] FROM @sp_campaigns [spc])";
        }
        if (skills)
        {
            cmdText += "AND [fc].[skillid] IN (SELECT [spc].[itemid] FROM @sp_skills [spc])\r";
        }
        if (agents)
        {
            cmdText += @"
                        AND [i].[interactionid] IN (
	                        SELECT
	                        [fca].[interactionid]
	                        FROM [dbo].[five9_call_agent] [fca] WITH(NOLOCK)
	                        WHERE 1=1
	                        AND [fca].[companyid] = [i].[companyid]
	                        AND [fca].[interactionid] = [i].[interactionid]
	                        AND [fca].[agentid] IN (SELECT [spc].[itemid] FROM @sp_agents [spc])
	                        GROUP BY [fca].[interactionid]
                        )
                        ";
        }
        #endregion Dynamic
        cmdText += @"";
        cmdText += "\r";
        cmdText += "";
        #endregion Build cmdText
        return cmdText;
    }
    static public String dashboard_call_details()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
SELECT
TOP (@sp_top)
[i].[interactionid]
,[fc].[callid]
,[i].[datestart]
,[fic].[name] [campaign]
,[i].[originator]
,[i].[destinator]
,(	SELECT
	TOP 1
	[fi].[name]
	FROM [dbo].[five9_call_disposition] [fcd1] WITH(NOLOCK)
	JOIN [dbo].[five9_item] [fi] WITH(NOLOCK) ON [fi].[itemid] = [fcd1].[dispositionid] AND [fi].[typeid] = 103000000
	WHERE 1=1
	AND [fcd1].[companyid] = [i].[companyid]
	AND [fcd1].[interactionid] = [i].[interactionid]
	AND [fcd1].[dispositionid] NOT IN (0) -- Blank
	ORDER BY [fcd1].[datecreated] DESC
) [disposition]
,(	SELECT
	TOP 1
	[fa].[fullname]
	FROM [dbo].[five9_call_disposition] [fcd1] WITH(NOLOCK)
	JOIN [dbo].[five9_agent] [fa] WITH(NOLOCK) ON [fa].[agentid] = [fcd1].[agentid]
	WHERE 1=1
	AND [fcd1].[companyid] = [i].[companyid]
	AND [fcd1].[interactionid] = [i].[interactionid]
	AND [fcd1].[agentid] NOT IN (0) -- Blank
	ORDER BY [fcd1].[datecreated] DESC
) [agent]
,[i].[duration]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_item] [fic] WITH(NOLOCK) ON [fic].[itemid] = [fc].[campaignid] AND [fic].[typeid] = 101000000
WHERE 1=1
AND [i].[companyid] = @sp_companyid
AND [i].[datestart] BETWEEN @sp_date_start AND @sp_date_end
AND [fc].[campaignid] IN (SELECT [spc].[campaignid] FROM @sp_campaigns [spc])
                            ";
        #endregion Build cmdText
        return cmdText;
    }
    static public String dashboard_call_details_dynamic(bool campaigns, bool skills, bool agents)
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
SELECT
TOP (@sp_top)
[i].[interactionid]
,[fc].[callid]
,[i].[datestart]
,[fic].[name] [campaign]
,[i].[originator]
,[i].[destinator]
,(	SELECT
	TOP 1
	[fi].[name]
	FROM [dbo].[five9_call_disposition] [fcd1] WITH(NOLOCK)
	JOIN [dbo].[five9_item] [fi] WITH(NOLOCK) ON [fi].[itemid] = [fcd1].[dispositionid] AND [fi].[typeid] = 103000000
	WHERE 1=1
	AND [fcd1].[companyid] = [i].[companyid]
	AND [fcd1].[interactionid] = [i].[interactionid]
	AND [fcd1].[dispositionid] NOT IN (0) -- Blank
	ORDER BY [fcd1].[datecreated] DESC
) [disposition]
,(	SELECT
	TOP 1
	[fa].[fullname]
	FROM [dbo].[five9_call_disposition] [fcd1] WITH(NOLOCK)
	JOIN [dbo].[five9_agent] [fa] WITH(NOLOCK) ON [fa].[agentid] = [fcd1].[agentid]
	WHERE 1=1
	AND [fcd1].[companyid] = [i].[companyid]
	AND [fcd1].[interactionid] = [i].[interactionid]
	AND [fcd1].[agentid] NOT IN (0) -- Blank
	ORDER BY [fcd1].[datecreated] DESC
) [agent]
,[i].[duration]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_item] [fic] WITH(NOLOCK) ON [fic].[itemid] = [fc].[campaignid] AND [fic].[typeid] = 101000000
WHERE 1=1
AND [i].[companyid] = @sp_companyid
AND [i].[datestart] BETWEEN @sp_date_start AND @sp_date_end
                            ";

        #region Dynamic
        if (campaigns)
        {
            cmdText += "AND [fc].[campaignid] IN (SELECT [spc].[itemid] FROM @sp_campaigns [spc])\r";
            // cmdText += @"AND [fc].[campaignid] IN (SELECT [spc].[campaignid] FROM @sp_campaigns [spc])";
        }
        if (skills)
        {
            cmdText += "AND [fc].[skillid] IN (SELECT [spc].[itemid] FROM @sp_skills [spc])\r";
        }
        if (agents)
        {
            cmdText += @"
                        AND [i].[interactionid] IN (
	                        SELECT
	                        [fca].[interactionid]
	                        FROM [dbo].[five9_call_agent] [fca] WITH(NOLOCK)
	                        WHERE 1=1
	                        AND [fca].[companyid] = [i].[companyid]
	                        AND [fca].[interactionid] = [i].[interactionid]
	                        AND [fca].[agentid] IN (SELECT [spc].[itemid] FROM @sp_agents [spc])
	                        GROUP BY [fca].[interactionid]
                        )
                        ";
        }
        #endregion Dynamic

        #endregion Build cmdText
        return cmdText;
    }
    static public String dashboard_call_dispositions()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText = @"
SELECT
[j].[dispositionname]
,COUNT([j].[interactionid]) [count]
,COUNT(DISTINCT([j].[interactionid])) [total_calls]
FROM
(
SELECT
[if].[interactionid]
,CASE
	WHEN LEN([if].[dispositionname]) > 0 THEN [if].[dispositionname]
	ELSE
		(
		SELECT
		TOP 1
		[fi].[name]
		FROM [dbo].[five9_call_disposition] [fcd1] WITH(NOLOCK)
		JOIN [dbo].[five9_item] [fi] WITH(NOLOCK) ON [fi].[itemid] = [fcd1].[dispositionid] AND [fi].[typeid] = 103000000
		WHERE 1=1
		AND [fcd1].[companyid] = [i].[companyid]
		AND [fcd1].[interactionid] = [i].[interactionid]
		AND [fcd1].[dispositionid] NOT IN (106000232) -- Blank
		--AND [fcd1].[agentid] NOT IN (0) -- Why?
		ORDER BY [fcd1].[datecreated] DESC
		)
END [dispositionname]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[interactions_five9] [if] WITH(NOLOCK) ON [if].[companyid] = [i].[companyid] AND [if].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
WHERE 1=1
AND [i].[companyid] = @sp_companyid
AND [i].[datestart] BETWEEN @sp_date_start AND @sp_date_end
AND [fc].[campaignid] IN (SELECT [spc].[campaignid] FROM @sp_campaigns [spc])
) [j]
GROUP BY [j].[dispositionname]
ORDER BY [j].[dispositionname]
";
        cmdText += "\r";
        #endregion Build cmdText
        return cmdText;
    }
    static public String dashboard_call_dispositions_dynamic(bool campaigns, bool skills, bool agents)
    {
        #region Build cmdText
        String cmdText = "";
        cmdText = @"
                    SELECT
                    [j].[dispositionname]
                    ,COUNT([j].[interactionid]) [count]
                    ,COUNT(DISTINCT([j].[interactionid])) [total_calls]
                    FROM
                    (
                    SELECT
                    [if].[interactionid]
                    ,CASE
	                    WHEN LEN([if].[dispositionname]) > 0 THEN [if].[dispositionname]
	                    ELSE
		                    (
		                    SELECT
		                    TOP 1
		                    [fi].[name]
		                    FROM [dbo].[five9_call_disposition] [fcd1] WITH(NOLOCK)
		                    JOIN [dbo].[five9_item] [fi] WITH(NOLOCK) ON [fi].[itemid] = [fcd1].[dispositionid] AND [fi].[typeid] = 103000000
		                    WHERE 1=1
		                    AND [fcd1].[companyid] = [i].[companyid]
		                    AND [fcd1].[interactionid] = [i].[interactionid]
		                    AND [fcd1].[dispositionid] NOT IN (106000232) -- Blank
		                    --AND [fcd1].[agentid] NOT IN (0) -- Why?
		                    ORDER BY [fcd1].[datecreated] DESC
		                    )
                    END [dispositionname]
                    FROM [dbo].[interactions] [i] WITH(NOLOCK)
                    JOIN [dbo].[interactions_five9] [if] WITH(NOLOCK) ON [if].[companyid] = [i].[companyid] AND [if].[interactionid] = [i].[interactionid]
                    JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
                    WHERE 1=1
                    AND [i].[companyid] = @sp_companyid
                    AND [i].[datestart] BETWEEN @sp_date_start AND @sp_date_end
                    ";
        #region Dynamic
        if (campaigns)
        {
            cmdText += "AND [fc].[campaignid] IN (SELECT [spc].[itemid] FROM @sp_campaigns [spc])\r";
            // cmdText += @"AND [fc].[campaignid] IN (SELECT [spc].[campaignid] FROM @sp_campaigns [spc])";
        }
        if (skills)
        {
            cmdText += "AND [fc].[skillid] IN (SELECT [spc].[itemid] FROM @sp_skills [spc])\r";
        }
        if (agents)
        {
            cmdText += @"
                        AND [i].[interactionid] IN (
	                        SELECT
	                        [fca].[interactionid]
	                        FROM [dbo].[five9_call_agent] [fca] WITH(NOLOCK)
	                        WHERE 1=1
	                        AND [fca].[companyid] = [i].[companyid]
	                        AND [fca].[interactionid] = [i].[interactionid]
	                        AND [fca].[agentid] IN (SELECT [spc].[itemid] FROM @sp_agents [spc])
	                        GROUP BY [fca].[interactionid]
                        )
                        ";
        }
        #endregion Dynamic
        cmdText += @"
) [j]
GROUP BY [j].[dispositionname]
ORDER BY [j].[dispositionname]

";
        cmdText += @"";
        cmdText += "\r";
        #endregion Build cmdText
        return cmdText;
    }
    static public String dashboard_call_type()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
SELECT
[fit].[name] [calltype]
,COUNT(DISTINCT([i].[interactionid])) [total_calls]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_item] [fit] WITH(NOLOCK) ON [fit].[itemid] = [fc].[typeid] AND [fit].[typeid] = 104000000
WHERE 1=1
AND [i].[companyid] = @sp_companyid
AND [i].[datestart] BETWEEN @sp_date_start AND @sp_date_end
AND [fc].[campaignid] IN (SELECT [spc].[campaignid] FROM @sp_campaigns [spc])
GROUP BY [fit].[name]
ORDER BY [fit].[name]
                            ";
        #endregion Build cmdText
        return cmdText;
    }
    static public String dashboard_call_type_dynamic(bool campaigns, bool skills, bool agents)
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
SELECT
[fit].[name] [calltype]
,COUNT(DISTINCT([i].[interactionid])) [total_calls]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_item] [fit] WITH(NOLOCK) ON [fit].[itemid] = [fc].[typeid] AND [fit].[typeid] = 104000000
WHERE 1=1
AND [i].[companyid] = @sp_companyid
AND [i].[datestart] BETWEEN @sp_date_start AND @sp_date_end
                            ";
        #region Dynamic
        if (campaigns)
        {
            cmdText += "AND [fc].[campaignid] IN (SELECT [spc].[itemid] FROM @sp_campaigns [spc])\r";
            // cmdText += @"AND [fc].[campaignid] IN (SELECT [spc].[campaignid] FROM @sp_campaigns [spc])";
        }
        if (skills)
        {
            cmdText += "AND [fc].[skillid] IN (SELECT [spc].[itemid] FROM @sp_skills [spc])\r";
        }
        if (agents)
        {
            cmdText += @"
                        AND [i].[interactionid] IN (
	                        SELECT
	                        [fca].[interactionid]
	                        FROM [dbo].[five9_call_agent] [fca] WITH(NOLOCK)
	                        WHERE 1=1
	                        AND [fca].[companyid] = [i].[companyid]
	                        AND [fca].[interactionid] = [i].[interactionid]
	                        AND [fca].[agentid] IN (SELECT [spc].[itemid] FROM @sp_agents [spc])
	                        GROUP BY [fca].[interactionid]
                        )
                        ";
        }
        #endregion Dynamic
        cmdText += @"
GROUP BY [fit].[name]
ORDER BY [fit].[name]
";
        cmdText += "\r";

        #endregion Build cmdText
        return cmdText;
    }
    static public String dashboard_campaign_count()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
SELECT
[fic].[name] [campaign]
,COUNT(DISTINCT([i].[interactionid])) [total_calls]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_item] [fic] WITH(NOLOCK) ON [fic].[itemid] = [fc].[campaignid] AND [fic].[typeid] = 101000000
WHERE 1=1
AND [i].[companyid] = @sp_companyid
AND [i].[datestart] BETWEEN @sp_date_start AND @sp_date_end
AND [fc].[campaignid] IN (SELECT [spc].[campaignid] FROM @sp_campaigns [spc])
GROUP BY [fic].[name]
ORDER BY [fic].[name]
";
        #endregion Build cmdText
        return cmdText;
    }
    static public String dashboard_campaign_count_dynamic(bool campaigns, bool skills, bool agents)
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
SELECT
[fic].[name] [campaign]
,COUNT(DISTINCT([i].[interactionid])) [total_calls]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_item] [fic] WITH(NOLOCK) ON [fic].[itemid] = [fc].[campaignid] AND [fic].[typeid] = 101000000
WHERE 1=1
AND [i].[companyid] = @sp_companyid
AND [i].[datestart] BETWEEN @sp_date_start AND @sp_date_end
";
        #region Dynamic
        if (campaigns)
        {
            cmdText += "AND [fc].[campaignid] IN (SELECT [spc].[itemid] FROM @sp_campaigns [spc])\r";
            // cmdText += @"AND [fc].[campaignid] IN (SELECT [spc].[campaignid] FROM @sp_campaigns [spc])";
        }
        if (skills)
        {
            cmdText += "AND [fc].[skillid] IN (SELECT [spc].[itemid] FROM @sp_skills [spc])\r";
        }
        if (agents)
        {
            cmdText += @"
                        AND [i].[interactionid] IN (
	                        SELECT
	                        [fca].[interactionid]
	                        FROM [dbo].[five9_call_agent] [fca] WITH(NOLOCK)
	                        WHERE 1=1
	                        AND [fca].[companyid] = [i].[companyid]
	                        AND [fca].[interactionid] = [i].[interactionid]
	                        AND [fca].[agentid] IN (SELECT [spc].[itemid] FROM @sp_agents [spc])
	                        GROUP BY [fca].[interactionid]
                        )
                        ";
        }
        #endregion Dynamic
        cmdText += @"
GROUP BY [fic].[name]
ORDER BY [fic].[name]
";
        cmdText += "\r";
        #endregion Build cmdText
        return cmdText;
    }
    static public String portal_ddl_clients(Int32 clientid)
    {
        #region Build cmdText
        String cmdText = "";
        cmdText = @"
                        SELECT
                        [c].[name]
                        ,[c].[clientid]
                        FROM [dbo].[clients] [c] WITH(NOLOCK)
                        WHERE 1=1
                                ";
        if (HttpContext.Current.User.IsInRole("Client"))
        {
            cmdText += "AND [c].[name] = @sp_client\r";
        }
        else if (HttpContext.Current.Session["clientname"] != null && HttpContext.Current.Session["clientname"].ToString().Length > 0)
        {
            cmdText += "AND [c].[name] = @sp_client\r";
        }
        else if (HttpContext.Current.User.Identity.Name.Contains("ncia2"))
        {
            cmdText += "AND [c].[name] = 'American Red Cross'\r";
        }
        else if (HttpContext.Current.User.Identity.Name.Contains("patriot2"))
        {
            cmdText += "AND [c].[name] = 'American Red Cross'\r";
        }
        else if (HttpContext.Current.User.Identity.Name.Contains("redcross2"))
        {
            cmdText += "AND [c].[name] = 'American Red Cross'\r";
        }
        else if (HttpContext.Current.User.Identity.Name.Contains("capella2"))
        {
            cmdText += "AND [c].[name] = 'Capella University'\r";
        }
        else if (HttpContext.Current.User.Identity.Name.Contains("hgtc2"))
        {
            cmdText += "AND [c].[name] = 'Horry Georgetown (HGTC)'\r";
        }
        else if (HttpContext.Current.User.Identity.Name.Contains("csn2"))
        {
            cmdText += "AND [c].[name] = 'College of Southern Nevada'\r";
        }
        else if (HttpContext.Current.User.IsInRole("Manager") == true && !HttpContext.Current.User.Identity.Name.Contains("@greenwoodhall.com"))
        {
            cmdText += "AND [c].[name] = @sp_client\r";
        }
        else if (HttpContext.Current.User.IsInRole("Client") == false && HttpContext.Current.User.Identity.Name.Contains("@greenwoodhall.com"))
        {
            cmdText += "\r";
        }
        else
        {
            cmdText += "AND [c].[clientid] = -1\r";
        }

        if (clientid > 0)
        {
            cmdText += "AND [c].[clientid] = @sp_clientid\r";
        }
        cmdText += "\r";
        cmdText += "ORDER BY [c].[clientid]\r";
        #endregion Build cmdText
        return cmdText;
    }
    static public String portal_ddl_campaigns(Int32 clientid)
    {
        #region Build cmdText
        String cmdText = "";
        cmdText = @"
SELECT
[fic].[name] [campaign]
,[fic].[itemid] [campaignid]
FROM [dbo].[five9_item] [fic] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fic].[itemid] = [fc].[campaignid]
WHERE [fic].[typeid] = 101000000
                                ";
        if (clientid > 0)
        {
            cmdText += @"
AND [fic].[itemid] IN (SELECT [ci].[itemid] FROM [dbo].[clients_items] [ci] WITH(NOLOCK) WHERE [ci].[clientid] = @sp_clientid AND [ci].[itemid] = [fic].[itemid])
";
        }
        cmdText += @"
GROUP BY [fic].[itemid], [fic].[name]
ORDER BY [fic].[name], [fic].[itemid]
";

        #endregion Build cmdText
        return cmdText;
    }
    static public String portal_ddl_skills(Int32 clientid)
    {
        #region Build cmdText
        String cmdText = "";
        cmdText = @"
SELECT
[fic].[name] [skill]
,[fic].[itemid] [skillid]
FROM [dbo].[five9_item] [fic] WITH(NOLOCK)
WHERE [fic].[typeid] = 102000000

                                ";
        if (clientid > 0)
        {
            cmdText += @"
AND [fic].[itemid] IN (SELECT [ci].[itemid] FROM [dbo].[clients_items] [ci] WITH(NOLOCK) WHERE [ci].[clientid] = @sp_clientid AND [ci].[itemid] = [fic].[itemid])
";
        }
        cmdText += @"
GROUP BY [fic].[itemid], [fic].[name]
ORDER BY CASE WHEN [fic].[name] = '[None]' THEN '0' WHEN [fic].[name] = '' THEN '1' ELSE [fic].[name] END
";
        #endregion Build cmdText
        return cmdText;
    }
    static public String portal_ddl_agents(Int32 clientid)
    {
        #region Build cmdText
        String cmdText = "";
        cmdText = @"
SELECT
CASE
	WHEN LEN([fa].[fullname]) > 0 AND LEN([fa].[username]) > 0 AND [fa].[fullname] != '[None]' THEN [fa].[fullname] + ' [' + [fa].[username] + ']'
	ELSE [fa].[fullname]
END [agent]
,[fa].[agentid] [agentid]
FROM [dbo].[five9_agent] [fa] WITH(NOLOCK)
WHERE 1=1
                                ";
        if (clientid > 0)
        {
            cmdText += @"
AND [fa].[agentid] IN (
    SELECT
    [fca].[agentid]
    FROM [dbo].[five9_call] [fc] WITH(NOLOCK)
    JOIN [dbo].[five9_call_agent] [fca] WITH(NOLOCK) ON [fca].[companyid] = [fc].[companyid] AND [fca].[interactionid] = [fc].[interactionid]
    WHERE 1=1
    AND [fc].[campaignid] IN (SELECT [ci].[itemid] FROM [dbo].[clients_items] [ci] WITH(NOLOCK) WHERE [ci].[clientid] = @sp_clientid AND [ci].[typeid] = 101000000)
    GROUP BY [fca].[agentid]
)
";
        }
        cmdText += @"
GROUP BY [fa].[agentid], [fa].[fullname], [fa].[username]
ORDER BY CASE WHEN [fa].[fullname] = '[None]' THEN '0' WHEN [fa].[fullname] = '' THEN '1' ELSE [fa].[fullname] END
";
        #endregion Build cmdText
        return cmdText;
    }


    static public String dashboard_sample()
    {
        #region Build cmdText
        String cmdText = "";
        #endregion Build cmdText
        return cmdText;
    }
    static public String dashboard_sample2()
    {
        #region Build cmdText
        String cmdText = ghQueries.dashboard_total_stats();
        #endregion Build cmdText
        return cmdText;
    }
}
