USE [Database_Name] --removed
GO
/****** Object:  UserDefinedFunction [dbo].[FnAvgMonthlyCountPerProduct]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ashley Vu

-- Description:	Table-valued function to select forecasted average monthly count per product
-- =============================================
ALTER FUNCTION [dbo].[FnAvgMonthlyCountPerProduct]
(	
	@Interval float,
	@StartDate date,
	@EndDate date
)
RETURNS TABLE 
AS
RETURN 
(
	select @StartDate as StartDate, @EndDate as EndDate, PlanId, DurationTypeId, NumAvg from (
		select 
			PlanId, 
			DurationTypeId,
			sum(Num) / @Interval as NumAvg
		from (
			select YEAR(pt.CreatedDate) as ThisYear, 
				DateName(month, DateAdd(month, MONTH(pt.CreatedDate), 0) -1) as ThisMonth, 
				pp.PlanName,
				dt.DurationName, 
				pt.PlanId,
				pt.DurationTypeId,
				count(*) as Num from PlanTransactions as pt
				join PricingPlanDurationRel as ppdr on pt.PlanId = ppdr.PricingPlanId and pt.DurationTypeId = ppdr.DurationTypeId 
				join PricingPlan as pp on ppdr.PricingPlanId = pp.Id 
				join DurationType as dt on ppdr.DurationTypeId = dt.Id
				--join #dateWindows dw on pt.CreatedDate between dw.[Start] and DATEADD(day, -1, dw.[End])
				where pt.CreatedDate between @StartDate and @EndDate
				group by YEAR(pt.CreatedDate), MONTH(pt.CreatedDate), pp.PlanName, pt.PlanId, dt.DurationName, pt.DurationTypeId
			) as tbl
		group by planId, durationtypeId
	) as results
)
