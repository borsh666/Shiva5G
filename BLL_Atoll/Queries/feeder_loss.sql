select 
	Tech, 
	cast(Feeder_Length as decimal) as Feeder_Length, 
	Feeder_Att_dB,Feeder_Name, 
	a.Feeder_Type
from dbo.feeders_loss a 

inner join dbo.feeders_map b 
on a.Feeder_Type = b.Feeder_Type