USE [Database_Name] --removed
GO
/****** Object:  StoredProcedure [dbo].[PlanTransactions_SelectForecastRevenues]     ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ashley Vu

-- Description:	Given a date range,
--				Select monthly forecast per Plan x DurationType,
--				Select total monthly forecast 
-- =============================================
ALTER PROCEDURE [dbo].[PlanTransactions_SelectForecastRevenues] 

	@StartDate date, 
	@EndDate date, 
	@PlanId int, 
	@DurationId int

AS
BEGIN
	SET NOCOUNT ON;

/* 
	{ Test Script }
	DECLARE	@return_value int
	
	EXEC	@return_value = [dbo].[PlanTransactions_SelectForecastRevenues]
			@StartDate = '2018-08-01',
			@EndDate = '2019-08-31',
			@PlanId = 24,
			@DurationId = 1
	
	SELECT	'Return Value' = @return_value
*/

	-- NOTE: PxD = Plan x Duration combination. E.g., Enterprise Plan x Quarterly Duration

-- Find interval (months) between input date range --> [StartMonth, EndMonth] inclusive
declare @Interval float = (select DATEDIFF(MONTH, @StartDate, @EndDate) + 1); 

if(OBJECT_ID('tempdb..#avgMonthly') is not null) drop table #avgMonthly
if(OBJECT_ID('tempdb..#indices') is not null) drop table #indices
if(OBJECT_ID('tempdb..#dateWindows') is not null) drop table #dateWindows
if(OBJECT_ID('tempdb..#avgCount') is not null) drop table #avgCount
if(OBJECT_ID('tempdb..#monthlyForecastPerProduct') is not null) drop table #monthlyForecastPerProduct

select
        DATENAME(MONTH, DATEADD(MONTH, MONTH(pt.CreatedDate), 0) -1) as AMonth,
        MONTH(pt.CreatedDate) as DateNum, 
        AVG(pp.Price * dt.DurationMonths) as PxDPrice,
        COUNT(*) as Cnt,
        PlanId,
        DurationTypeId
into #avgMonthly -- #AVGMONTHLY 
from PlanTransactions pt
join PricingPlan pp on pp.Id = pt.PlanId
join DurationType dt on dt.Id= pt.DurationTypeId
group by MONTH(pt.CreatedDate), PlanId, DurationTypeId

alter table #avgMonthly add TotalAvg decimal(18,2) -- TotalAvg = avg revenue for PxD combo 

UPDATE am 
    set am.TotalAvg = tavg.MonthlyAvg
from #avgMonthly am
join (
    select AMonth, 
    AVG(PxDPrice) as MonthlyAvg 
    from (
        select * from #avgMonthly   
    ) as avg1
    group by AMonth 
) as tavg on tavg.AMonth = am.AMonth 
select 
    *,
    (PxDPrice * Cnt / (select AVG(AmountPaid) -- Seasonal Index (demand) = (Actual revenue per PxD for month) / (Avg revenue per PxD all months)
						from PlanTransactions 
						where PlanId = #avgMonthly.PlanId and DurationTypeId = #avgMonthly.DurationTypeId)) as _Index
    into #indices --#INDICES
from #avgMonthly

-- SHIFTING DATE WINDOWS 
;with cte as (
    select
        DATEADD(MONTH, -@Interval, @StartDate) as [Start],
        @StartDate as [End]
    union all 
    select 
        DATEADD(MONTH, 1, [Start]) as [Start], 
        DATEADD(MONTH, 1, [End]) as [End]
    from cte 
        where [Start] < DATEADD(MONTH, -1, @StartDate)
)
select [Start], [End] into #dateWindows from cte -- #DATEWINDOWS


select fn.* into #avgCount from #dateWindows dw -- #AVGCOUNT
cross apply dbo.FnMonthlyCountPerProduct(@Interval, dw.[Start], DATEADD(DAY, -1, dw.[End])) fn 
where dw.[Start] = fn.StartDate and fn.EndDate = DATEADD(DAY, -1, dw.[End])
select ThisMonth,
	   OrderNum,
	   ThisYear, 
       PlanId, 
       DurationTypeId, 
       NumAvg, 
       Forecast
into #monthlyForecastPerProduct
from
(select DATENAME(MONTH, DATEADD(MONTH, MONTH(EndDate), 0)) as ThisMonth, -- normally offset -1, but don't in this case bc #DateWindows is offset by -1 already
	MONTH(DATEADD(MONTH, MONTH(EndDate), 0)) as OrderNum,
	YEAR(DATEADD(MONTH, 1, EndDate)) as ThisYear,
	ac.PlanId as PlanId, 
	ac.DurationTypeId DurationTypeId,
	NumAvg,
	NumAvg * (
	    select 
	    price * dt.DurationMonths as PxDPrice
	    from PricingPlan pp 
	    join DurationType dt on dt.Id = ac.DurationTypeId
	    where pp.Id = ac.PlanId 
	) * ISNULL(#indices._index, 1) as Forecast
from #avgCount ac
left join #indices on ac.PlanId = #indices.PlanId 
and ac.DurationTypeId = #indices.DurationTypeId  
and DATENAME(MONTH, DATEADD(MONTH, MONTH(EndDate), 0)) = #indices.AMonth 
) forecastPerMonth 
group by ThisMonth, OrderNum, ThisYear, PlanId, DurationTypeId, NumAvg, Forecast

-- SELECT FORECAST PER PRODUCT PER MONTH
select ThisYear,
		ThisMonth,
		OrderNum,
		PlanId,
		DurationTypeId,
		NumAvg,
		Forecast,
		pp.PlanName, 
		dt.DurationName
from #monthlyForecastPerProduct --#MONTHLYFORECASTPERPRODUCT 
join PricingPlan as pp on #monthlyForecastPerProduct.PlanId = pp.Id 
join DurationType as dt on #monthlyForecastPerProduct.DurationTypeId = dt.Id
where #monthlyForecastPerProduct.PlanId = @PlanId and #monthlyForecastPerProduct.DurationTypeId = @DurationId
group by ThisMonth, ThisYear, OrderNum, PlanId, DurationTypeId, NumAvg, Forecast, pp.PlanName, dt.DurationName
order by ThisYear, OrderNum


-- SELECT TOTAL FORECAST PER MONTH
select 
	ThisYear,
	ThisMonth,
	OrderNum,
	sum(Forecast) as Forecast
from #monthlyForecastPerProduct
group by ThisYear, ThisMonth, OrderNum
order by ThisYear, OrderNum

--select * from #avgMonthly 
--select * from #indices
--select * from #dateWindows
--select * from #avgCount
--select * from #monthlyForecastPerProduct

END
