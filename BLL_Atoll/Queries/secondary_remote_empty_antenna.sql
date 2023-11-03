--declare @siteid as varchar(15) = '@SiteID@';


--select * 
--from secondary_remote_antenna
--where CellName like (substring(@siteid,3,4) + '%')  

--union

--select * 
--from empty_antenna
--where CellName like (substring(@siteid,3,4) + '%')  

select * from 
    (
    select 
        SectorId,CellName,Mtilt,Etilt,Azimuth,Technology,Band,a.AntennaType,
        Height,RruType,AntennaId,AntennaLevel,ant_map.PortId,Status,BANDRANGE,FMIN,FMAX,BANDRANGE1,BANDRANGE2,
        case 
        when Band='B7' then 700  
        when Band='B8' then 800  
        when Band='B9' then 900  
        when Band='B18' then 1800  
        when Band='B21' then 2100  
        when Band='B26' then 2600  
        when Band='B35' then 3500
        end as bandFreq  
    from
    (
        select * 
        from secondary_remote_antenna
        where CellName like  '19041%'


        union

        select * 
        from empty_antenna
        where CellName like  '19041%'
    )a

    inner join

    (
        select distinct
               ANTENNATYPE ,
               PORTID,
               BANDRANGE 
               ,SUBSTRING(BANDRANGE, 1, CHARINDEX('-', BANDRANGE) - 1) AS BANDRANGE1,
               SUBSTRING(BANDRANGE, CHARINDEX('-', BANDRANGE) + 1, LEN(BANDRANGE)) AS BANDRANGE2
       from antenna_port_map
       where BANDRANGE <> 'NOVAL'
    ) ant_map

    on a.AntennaType=ant_map.ANTENNATYPE

    inner join

    (
        select distinct
               FMIN ,PHYSICAL_ANTENNA,
               FMAX 
       from [ATOLLPRODDB].[ATOLL_5GMRAT].[dbo].antennas 
    ) ant_map1

    on ant_map1.PHYSICAL_ANTENNA=ant_map.ANTENNATYPE
)u
where 
    bandFreq>FMIN and bandFreq<FMAX 
    and bandFreq>BANDRANGE1 and bandFreq<BANDRANGE2
    and Azimuth is not null

