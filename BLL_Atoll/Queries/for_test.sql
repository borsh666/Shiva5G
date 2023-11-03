
----------------GSM_ATOLL------------------------------
select 
    site_name, 
    substring(cast(tx_id as varchar),5,1) as SectorId,
    cast(tx_id as varchar) as CellName, 
    'G' as Technology,
    case when FBAND = 'V GSM 900 E' then 'B9'
         when FBAND = 'V GSM 1800' then 'B18'
    end as Band,
    0.02 as BandWidth,
    HEIGHT as StructureHeight, 
    CAST(CAST(AZIMUT AS NUMERIC) as INT) as Azimuth,
    antenna_name, 
    SHAREDMAST as AntennaId,
    TILT as Mtilt, 
    FEEDERLENGTH_DL as Feeder_Length, 
    FEEDER_NAME,
    CF_TMA  as tma,
    CF_ANTENNA_MOUNTING  as AntennaMount,
    CF_COMBINER_SPLITTER  as Combiner_Splitter,
    CF_2ND_COMBINER  as Sec_Combiner_Splitter,
    CF_SITE_PROGRESS as cell_progress, 
    CF_RRU_TYPE as rru,
    value  as Antenna_ports,
    CF_PORT_LIST as RruInComb,
    POWER as PwrPerTrxAtoll,
    cast(NUM_TRX as int)  as TrxAtoll

from [dbo].[gtransmitters] a 
CROSS APPLY STRING_SPLIT(a.CF_PORT_LIST, ';') as f

where  site_name like ('VA4070%')
;
-------------------UMTS_ATOLL-------------------
select 
    site_name, 
    substring(cast(tx_id as varchar),5,1) as SectorId,
    cast(tx_id as varchar) as CellName,
    'U' as TECHNOLOGY,
    case
        when FBAND = 'UTRA Band VIII'  then 'B9'
        when FBAND = 'UTRA Band I' then 'B21'
    end as Band,
    0.2 as BandWidth,
    HEIGHT as StructureHeight,
    CAST(CAST(AZIMUT AS NUMERIC) as INT) as Azimuth,
    antenna_name, 
    SHAREDMAST as AntennaId,
    TILT as Mtilt, 
    FEEDERLENGTH_DL as Feeder_Length, 
    FEEDER_NAME,
    CF_TMA  as tma,
    CF_ANTENNA_MOUNTING  as AntennaMount,
    CF_COMBINER_SPLITTER  as Combiner_Splitter,
    CF_2ND_COMBINER  as Sec_Combiner_Splitter,
    CF_SITE_PROGRESS as cell_progress, 
    CF_RRU_TYPE as rru,
    value  as Antenna_ports,
    CF_PORT_LIST as RruInComb,
    PwrPerTrxAtoll,
    1 as TrxAtoll
     
from     [dbo].[utransmitters] a
CROSS APPLY STRING_SPLIT(a.CF_PORT_LIST, ';') as f
left HASH join
    (
     select CELL_ID,power_max as PwrPerTrxAtoll  
     from dbo.ucells
    ) ucells
on tx_id = CELL_ID collate Cyrillic_General_CI_AS
where  site_name like ('VA4070%')
;
-------------------LTE_ATOLL-------------------
select 
    site_name, 
    substring(cast(tx_id as varchar),5,1) as SectorId,
    cast(tx_id as varchar) as CellName,
    'L' as Technology,
    Band,
    BandWidth,
    HEIGHT as StructureHeight,
    CAST(CAST(AZIMUT AS NUMERIC) as INT) as Azimuth,
    antenna_name, 
    SHAREDMAST as AntennaId,
    TILT as Mtilt, 
    FEEDERLENGTH_DL as Feeder_Length, 
    FEEDER_NAME,
    CF_TMA  as tma,
    CF_ANTENNA_MOUNTING  as AntennaMount,
    CF_COMBINER_SPLITTER  as Combiner_Splitter,
    CF_2ND_COMBINER  as Sec_Combiner_Splitter,
    CF_SITE_PROGRESS as cell_progress, 
    CF_RRU_TYPE as rru,
    value  as Antenna_ports,
    CF_PORT_LIST as RruInComb,
    PwrPerTrxAtoll,
    1 as TrxAtoll

from     [dbo].[xgtransmitters]  a
CROSS APPLY STRING_SPLIT(a.CF_PORT_LIST, ';') as f
inner HASH join
    (
     select 
            CELL_ID as CellName, 
            RS_EPRE as PwrPerTrxAtoll, 
            case
                    when FBAND = 'n1 / E-UTRA 1' then 'B21'
                    when FBAND = 'n7 / E-UTRA 7' then 'B26'
                    when FBAND = 'n8 / E-UTRA 8' then 'B9'
                    when FBAND = 'n3 / E-UTRA 3' then 'B18'
            end as Band,
            DL_WIDTH as BandWidth
     from xgcellslte cellslte
     inner HASH join dbo.xgcarriers carriers 
     on carriers.NAME = cellslte.CARRIER
    ) xgcellslte
on tx_id = xgcellslte.CellName
where  site_name like ('VA4070%')
;
-------------------NR_ATOLL-------------------
select 
	site_name, 
	substring(cast(tx_id as varchar),5,1) as SectorId,
	cast(tx_id as varchar) as CellName,
	case 
		when Band = 'B35' then 'NR'
		else 'DSS' end as Technology,
	Band,
	BandWidth,
	HEIGHT as StructureHeight,
	CAST(CAST(AZIMUT AS NUMERIC) as INT) as Azimuth,
	antenna_name, 
	SHAREDMAST as AntennaId,
	TILT as Mtilt, 
	FEEDERLENGTH_DL as Feeder_Length, 
	FEEDER_NAME,
	CF_TMA  as tma,
	CF_ANTENNA_MOUNTING  as AntennaMount,
	CF_COMBINER_SPLITTER  as Combiner_Splitter,
	CF_2ND_COMBINER  as Sec_Combiner_Splitter,
	CF_SITE_PROGRESS as cell_progress, 
	CF_RRU_TYPE as rru,
	value  as Antenna_ports,
	CF_PORT_LIST as RruInComb,
	case when CF_RRU_TYPE like '%AAU%' then 7.5
		when CF_RRU_TYPE like '%5258%' then 60
		else 0 end as PwrPerTrxAtoll,
	case when band = 'B35' then 1
		else 0 end as TrxAtoll
     
from     [dbo].[xgtransmitters]  a
CROSS APPLY STRING_SPLIT(a.[CF_PORT_LIST], ';') as f
inner HASH join
	(
		select 
			CELL_ID as CellName, 
			0 as PwrPerTrxAtoll, 
			case
				when FBAND = 'n1 / E-UTRA 1' then 'B21'
				when FBAND = 'n7 / E-UTRA 7' then 'B26'
				when FBAND = 'n8 / E-UTRA 8' then 'B9'
				when FBAND = 'n3 / E-UTRA 3' then 'B18'
				when FBAND = 'n78' then 'B35'
			end as Band,
			DL_WIDTH as BandWidth
		from dbo.xgcells5gnr cellslte
		inner HASH join dbo.xgcarriers carriers 
		on carriers.NAME = cellslte.CARRIER
	) xgcellslte
on tx_id = xgcellslte.CellName
where  site_name like ('VA4070%')
;
-------------------GSM_CM-------------------
select CellName,avg(PwrPerTrxCm) as PwrPerTrxCm, count(PwrPerTrxCm) as TrxCm
             
from 
	(
		select Cell_Name as CellName,TRX_ID,
			   avg(eGBTS_Power_Type_01dBm /10) as PwrPerTrxCm 
		from panorama.hwi.CM2G.GBTS_power 
		where timestamp = (select max(timestamp) from panorama.hwi.CM2G.GBTS_power)
			  and BTS_Name like ('VA4070%')
		group by Cell_Name,TRX_ID 
	) a1
group by CellName
;
-------------------UMTS_CM-------------------
select 
	CellName ,
	MaxTransmitPowerofCell/10 as PwrPerTrxCm
      
from panorama.hwi.CM.UCELLSETUP_SRAN 
where timestamp = (select max(timestamp) from panorama.hwi.CM.UCELLSETUP_SRAN)
      and NodeBName like ('VA4070%')
	  
-------------------LTE_CM-------------------
select cell_name as CellName, 
    case 
        when power(10,cast(Reference_signal_power_01dBm as float)/100) *
             cast(pb as float)*pa*downlink_bandwidth*12=0 then 0 
        else CEILING(log(power(10,cast(Reference_signal_power_01dBm as float)/100) * 
                         cast(pb as float)*pa*downlink_bandwidth*12,10)*100) /10
    end as PwrPerTrxCm

from 
	(
		select 
			cell_name,
			Reference_signal_power_01dBm, 
			substring(downlink_bandwidth,10,len(downlink_bandwidth)-9) as Downlink_bandwidth,
			case 
				when PA_for_even_power_distribution_dB='DB_3_P_A' then round(power(10.0,(-3.0/10)),2)
				when PA_for_even_power_distribution_dB='DB0_P_A' then round(power(10.0,(0.0/10)),2)
				when PA_for_even_power_distribution_dB='DB1_P_A' then round(power(10.0,(1.0/10)),2)
				when PA_for_even_power_distribution_dB='DB2_P_A' then round(power(10.0,(2.0/10)),2)
				when PA_for_even_power_distribution_dB='DB2_P_A' then round(power(10.0,(3.0/10)),2)
				when PA_for_even_power_distribution_dB='DB_1DOT77_P_A' then round(power(10.0,(-1.77/10)),2)
				when PA_for_even_power_distribution_dB='DB_4DOT77_P_A' then round(power(10.0,(-4.77/10)),2)
				when PA_for_even_power_distribution_dB='DB_6_P_A' then round(power(10.0,(-6.0/10)),2)
			end as pa,
			case 
				when Cell_transmission_and_reception_mode like '%1T%' 
				then case 
						when pb='0' then 1.0
						when pb='1' then 4.0/5
						when pb='2' then 3.0/5
						when pb='3' then 2.0/5 
						else null end
				else 
					case 
						when pb='0' then 5.0/4
						when pb='1' then 1.0
						when pb='2' then 3.0/4 
						when pb='3' then 1.0/2 
						else null end end as pb
		from panorama.hwi.CM4G.LCELL a
		where timestamp=(select max(timestamp) from panorama.hwi.CM4G.LCELL)
			  and Cell_active_state = 'CELL_ACTIVE'
			  and Reference_signal_power_01dBm >0
			  and Downlink_bandwidth is not null
			  and NE_Name like ('VA4070%')
	) a
;
-------------------ANTENNAS_ATOLL-------------------
select * 

from [dbo].[antennas]
;
-------------------ANTENNA_PORT_MAP_SHIVA-------------------
select ANTENNATYPE,
	   PORTID,
	   BANDRANGE,
       case 
		when PORTTYPE ='NOVAL' then '' 
		else PORTTYPE 
	   end as PORTTYPE 

from [shiva].[dbo].[antenna_port_map]
;
-------------------RRU_CM-------------------
select 
	CellName,
	RRU_type as RruTypeCm,
	RRU_Serial_Number as RruSN  

from panorama.hwi.CMSR.RRUsPerNodes_5G_included rru_cm
where timestamp = (select max(timestamp) from panorama.hwi.CMSR.RRUsPerNodes_5G_included)
	  and SITE like ('VA4070%')
;
----------------------MAIN QUERY------------------------------
    
select 
    left(replace(sites.name,'Prop',''),6) as SiteId,
    SectorId,
    right(sites.name,1) as Candidate, 
    sites.ALIAS_ as site_name_alias, 
    sites.CF_NAME1 as SiteName, 
    Technology,
    Band,
    BandWidth,
    trans.CellName, 
    sites.LATITUDE as latitude, 
    sites.LONGITUDE as longitude,
    StructureHeight as Height, 
    Mtilt, 
    cast(Feeder_Length as decimal) as Feeder_Length,
    FEEDER_NAME as Feeder_Name,
    Azimuth,
    AntennaId,  
    ant.BEAMWIDTH as ant_beam, 
    ant.CF_VBEAMWIDTH as verticalbw, 
    ant.PHYSICAL_ANTENNA as AntennaType, 
    ant.ELECTRICAL_TILT as Etilt, 
    ant.fmin as frequency,
    sites.CF_PROVINCE + sites.CF_ADDRESS1 as SiteAddress,  
    sites.CF_ADDRESS2 as address2, 
    null as town, 
    sites.CF_STATE as state,
    cell_progress as cell_progress, 
    sites.CF_SITE_PROGRESS as site_progress,
    dense_rank() over (partition by left(replace(sites.name,'Prop',''),6) order by
    case when sites.CF_SITE_PROGRESS='On Air' then 1
    when sites.CF_SITE_PROGRESS='UMTS Not on Air' then 2
    when sites.CF_SITE_PROGRESS='MSV Done' then 3
    when sites.CF_SITE_PROGRESS='SIR Validated' then 4
    when sites.CF_SITE_PROGRESS='SARF Issued' then 5
    when sites.CF_SITE_PROGRESS='Surveyed' then 6
    when sites.CF_SITE_PROGRESS='Not Surveyed' then 7
    when sites.CF_SITE_PROGRESS='None' then 8
    when sites.CF_SITE_PROGRESS='Shut Down' then 9
    when sites.CF_SITE_PROGRESS='Decommisioned' then 10
    else 11 end) as priority_1, 
    rru as RruTypeAtoll,
    RruTypeCm,
    RruSN,
    tma,
    Antenna_ports,
    RruInComb,
    AntennaMount,
    Combiner_Splitter,
    Sec_Combiner_Splitter,
    bandrange as BandRange,
    porttype,
    portid,
    TrxAtoll,
    TrxCm,
    PwrPerTrxAtoll,
    PwrPerTrxCm,
    CF_STRUCTURE_TYPE as StructureType,
    CF_CO_LOCATION as ColocationType,
    CF_BSC as BSC,
    CF_RNC as RNC,
    CF_SITE_OWNER as SiteOwner
    
from
    (select * from [dbo].[sites]) sites

left HASH join
(
    select a.*, 
           PwrPerTrxCm,
           1 as TrxCm 
    from #GSM_ATOLL a
    left HASH join #GSM_CM b
    on a.CellName = b.CellName collate Cyrillic_General_CI_AS
    
    union
     
    select a.*, 
           PwrPerTrxCm,
           1 as TrxCm 
    from #UMTS_ATOLL a
    left HASH join #UMTS_CM b
    on a.CellName = b.CellName collate Cyrillic_General_CI_AS
    
    union
     
    select a.*, 
           PwrPerTrxCm,
           1 as TrxCm 
    from #LTE_ATOLL a
    left HASH join #LTE_CM b
    on a.CellName = b.CellName collate Cyrillic_General_CI_AS
    
    union
     
    select a.*, 
           case when rru like '%AAU%' then 7.5
                     when rru like '%5258%' then 60
                     else 0 end as PwrPerTrxCm,
           case when band = 'B35' then 1
                     else 0 end as TrxCm
    from #NR_ATOLL a
)trans
on sites.name=trans.site_name

left HASH join #ANTENNAS_ATOLL ant
on trans.ANTENNA_NAME=ant.name

left HASH join #ANTENNA_PORT_MAP_SHIVA ant_map
on ant.PHYSICAL_ANTENNA=ant_map.antennatype and trans.Antenna_ports = ant_map.portid

left HASH join #RRU_CM rru_cm
on trans.CellName = rru_cm.CellName collate Cyrillic_General_CI_AS


where left(replace(sites.name,'Prop',''),6)='VA4070' 
      and SectorID is not null
      and SectorID <= 9
      and AntennaID is not null
      and AntennaID % 10 <= 4  
      and PortId is not null
      and Band is not null
      and sites.CF_PARAMETERS_PLAN = 'Planned'
      and sites.CF_CANDIDATE = 'Accepted'
