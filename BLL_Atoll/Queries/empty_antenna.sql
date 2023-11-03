declare @siteid as varchar(15) = '@SiteID@';

select distinct CF_EMPTY_ANTENNA as AntennaType,
	   substring(cast(tx_id as varchar),5,1) as SectorId
from [dbo].[utransmitters]
where  site_name like (@siteid + '%') and CF_EMPTY_ANTENNA is not null 