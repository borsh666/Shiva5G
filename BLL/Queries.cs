using System;
using System.Collections.Generic;
using System.Linq;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Threading.Tasks;
using DAL_Asset10;
using MoreLinq;
using System.IO;
using Newtonsoft.Json;
using DAL_HWI;
using OfficeOpenXml.FormulaParsing.Utilities;

namespace BLL
{
    public class Queries
    {
        private string siteID;

        private HWI_DB hwi_db = new HWI_DB();

        public Queries(string siteID)
        {
            this.siteID = siteID;
            hwi_db.Database.CommandTimeout = 160;
        }

        private List<ViewModelTechnology> GetPorts2G()
        {
            using (var contextAsset = new Entities())
            {
                var ports = contextAsset.Database.SqlQuery<ViewModelTechnology>($@"
                                select 												
                                    a.IDNAME as CellName,b.GSMID as CellID,la.PORTS as PortNumber,												
                                    replace(replace( k1.IDNAME, 'GSM',''),'_Default','') as Band												
                                FROM network_planning.GSMCELL b												
                                INNER JOIN network_planning.LOGCELL a ON b.PROJECTNO = a.PROJECTNO AND a.LOGCELLPK = b.LOGCELLfK 												
                                INNER JOIN network_planning.munode c ON a.PROJECTNO = c.PROJECTNO AND a.LOGNODEFK = c.LOGNODEPK												
                                INNER JOIN network_planning.LOGNODE cn ON c.PROJECTNO = cn.PROJECTNO AND c.LOGNODEpK = cn.LOGNODEPK 												
                                INNER JOIN network_planning.SITEADDRESS d ON cn.projectno = d.projectno AND cn.addressfk = d.addresskey												
                                INNER JOIN network_planning.CELLAYDATA g ON a.projectno = g.projectno AND a.LOGCELLpK = g.CELLkey												
                                INNER JOIN network_planning.LOGCELLFEEDER k ON a.projectno = k.projectno AND a.LOGCELLPK = k.logcellfk and g.CELLAYKEY=k.GSM1CELLLAYERFK  												
                                INNER JOIN network_planning.CELLAY k1 ON k1.projectno = g.projectno AND k1.cellaykey = g.cellaykey												
                                inner join logicalantenna la on la.PROJECTno = k.PROJECTno and la.LOGANTENNAPK = k.LOGANTENNAFK 												
                                join network_planning.FLAGVALUES z ON c.projectno = z.projectno AND c.LOGNODEPK = z.objectkey												
                                INNER JOIN network_planning.FLAGS y ON y.PROJECTNO = z.PROJECTNO AND y.FLAGGROUPKEY = z.FLAGGROUPKEY AND y.FLAGKEY = z.FLAGKEY												
                                INNER JOIN network_planning.FLAGGROUPS x ON x.PROJECTNO = y.PROJECTNO AND x.FLAGGROUPKEY = y.FLAGGROUPKEY												
												
                                where(x.flaggroupid = 'Candidate')												
                                AND(y.flagid = 'Accepted')												
                                AND(a.PROJECTNO = 1) AND(b.GSMID > 0)
                                and d.idname like '%{siteID}%'").ToList();
                return ports;
            }

        }

        private List<ViewModelTechnology> GetPorts3G()
        {
            using (var contextAsset = new Entities())
            {
                var ports = contextAsset.Database.SqlQuery<ViewModelTechnology>($@"
                                select  
                                    b1.umtscellid as CellID, c2.IDNAME AS CellName, la.PORTS as PortNumber

                                from  network_planning.LOGUMTSCELL b1 
                                INNER JOIN network_planning.LOGCELL c2 ON b1.PROJECTNO = c2.PROJECTNO AND b1.LOGCELLFK = c2.LOGCELLPK 
                                INNER JOIN network_planning.LOGNODE c1 ON c2.PROJECTNO = c1.PROJECTNO AND c2.LOGNODEFK = c1.LOGNODEPK 
                                INNER JOIN network_planning.LOGUMTSCELLCAR ucc ON b1.PROJECTNO = ucc.PROJECTNO AND b1.LOGCELLFK = ucc.LOGCELLFK 
                                INNER JOIN network_planning.LOGUMTSCAR uc ON ucc.projectno = uc.projectno AND ucc.CARRIERFK = uc.UMTSCARPK 
                                INNER JOIN network_planning.TGCARRIER ut ON uc.projectno = ut.projectno AND uc.tgcarrierfk = ut.carrierkey 
                                INNER JOIN network_planning.SITEADDRESS h ON c1.projectno = h.projectno AND c1.addressfk = h.addresskey 
                                INNER JOIN network_planning.LOGCONNECTION d1 ON c1.PROJECTNO = d1.PROJECTNO AND c1.LOGNODEPK = d1.LOGNODEBFK 
                                INNER JOIN network_planning.LOGRNC e1 ON d1.PROJECTNO = e1.PROJECTNO AND d1.lognodeafk = e1.lognodepk 
                                INNER JOIN network_planning.LOGcellFEEDER f ON b1.PROJECTNO = f.projectno AND b1.LOGCELLFK = f.LOGCELLFK 
                                INNER join logicalantenna la on la.PROJECTno=f.PROJECTno and la.LOGANTENNAPK =f.LOGANTENNAFK

                                INNER JOIN network_planning.FLAGVALUES z ON c1.projectno = z.projectno AND c1.lognodepk = z.objectkey 
                                INNER JOIN network_planning.FLAGS y ON y.PROJECTNO = z.PROJECTNO AND y.FLAGGROUPKEY = z.FLAGGROUPKEY AND y.FLAGKEY = z.FLAGKEY 
                                INNER JOIN network_planning.FLAGGROUPS x ON x.PROJECTNO = y.PROJECTNO AND x.FLAGGROUPKEY = y.FLAGGROUPKEY

                                INNER JOIN network_planning.FLAGVALUES z1 ON c1.projectno = z1.projectno AND c2.logcellpk = z1.objectkey 
                                INNER JOIN network_planning.FLAGS y1 ON y1.PROJECTNO = z.PROJECTNO AND y1.FLAGGROUPKEY = z1.FLAGGROUPKEY AND y1.FLAGKEY = z1.FLAGKEY 
                                INNER JOIN network_planning.FLAGGROUPS x1 ON x1.PROJECTNO = y1.PROJECTNO AND x1.FLAGGROUPKEY = y1.FLAGGROUPKEY

                                where 
                                (x.flaggroupid = 'Candidate') AND (y.flagid ='Accepted') 
                                AND (x1.flaggroupid = 'Site Progress') AND (y1.flagid ='On Air') 
                                AND (b1.PROJECTNO = 1) 
                                AND c1.idname like '%{siteID}%'").ToList();

                return ports;
            }
        }

        private List<ViewModelTechnology> GetPorts4G()
        {
            using (var contextAsset = new Entities())
            {
                var ports = contextAsset.Database.SqlQuery<ViewModelTechnology>($@"
                                    select 			
                                        a.cellid as CellID, b4.idname as CellName			
                                        ,la.PORTS as PortNumber			
			
                                    from LOGLTECELL a       			
                                    INNER JOIN LogLteCellCar a1 ON a1.PROJECTNO =a.PROJECTNO AND a1.LOGCELLFK = a.LOGCELLFK    			
                                    INNER JOIN logcell b4 ON b4.PROJECTNO =a.PROJECTNO AND b4.LOGCELLPK = a.LOGCELLFK  			
                                    INNER JOIN LOGNODE b5 ON b4.PROJECTNO = b5.PROJECTNO AND b4.LOGNODEFK = b5.LOGNODEPK			
                                    INNER JOIN network_planning.LOGcellFEEDER f ON a.PROJECTNO = f.projectno AND a.LOGCELLFK = f.LOGCELLFK			
                                    inner join logicalantenna la on la.PROJECTno=f.PROJECTno and la.LOGANTENNAPK =f.LOGANTENNAFK			
                                    inner join network_planning.FLAGVALUES z ON b5.projectno = z.projectno AND b5.lognodepk = z.objectkey 			
                                    INNER JOIN network_planning.FLAGS y ON y.PROJECTNO = z.PROJECTNO AND y.FLAGGROUPKEY = z.FLAGGROUPKEY AND y.FLAGKEY = z.FLAGKEY 			
                                    INNER JOIN network_planning.FLAGGROUPS x ON x.PROJECTNO = y.PROJECTNO AND x.FLAGGROUPKEY = y.FLAGGROUPKEY			
			
                                    where (x.flaggroupid = 'Candidate')  			
                                    AND (y.flagid ='Accepted') 			
                                    AND (a.PROJECTNO = 1) 	
                                     and b5.idname like '%{siteID}%'			
			                         ").ToList();
                return ports;
            }

        }

        private List<ViewModelTechnology> EtiltAsset2G()
        {
            using (var contextAsset = new Entities())
            {
                var eTilts = contextAsset.Database.SqlQuery<ViewModelTechnology>($@"
                                select  a.IDNAME as CellName,b.GSMID as CellID,	la.INDEXNO as LOGINDEX,															
															
                                case when la.INHERITMASTERPATTERN = 0 then ap.downtilt															
                                   when la.INHERITMASTERPATTERN = 1 then ap1.downtilt															
                                   when la.INHERITMASTERPATTERN = 2 then ap2.downtilt															
                                   when la.INHERITMASTERPATTERN = 3 then ap3.downtilt															
                                   when la.INHERITMASTERPATTERN = 4 then ap4.downtilt															
                                   end AS Etilt															
															
															
                                FROM network_planning.GSMCELL b 															
                                            INNER JOIN  network_planning.LOGCELL a ON b.PROJECTNO = a.PROJECTNO AND a.LOGCELLPK=b.LOGCELLfK 															
                                            INNER JOIN  network_planning.munode c ON a.PROJECTNO = c.PROJECTNO AND a.LOGNODEFK = c.LOGNODEPK 															
                                            INNER JOIN  network_planning.LOGNODE cn ON c.PROJECTNO = cn.PROJECTNO AND c.LOGNODEPK = cn.LOGNODEPK 															
                                            INNER JOIN  network_planning.SITEADDRESS d ON cn.projectno = d.projectno AND cn.addressfk = d.addresskey 															
                                                INNER JOIN network_planning.CELLAYDATA g ON b.projectno = g.projectno AND b.LOGCELLfK = g.cellkey 															
                                         --  INNER JOIN network_planning.CARRIERS i ON g.CELLAYDATAPK = i.CELLAYDATAFK AND g.PROJECTNO = i.PROJECTNO 															
                                           -- INNER JOIN network_planning.CARLAY j ON i.PROJECTNO = j.PROJECTNO AND i.CARLAYKEY = j.CARLAYKEY 															
                                          INNER JOIN network_planning.LOGCELLFEEDER k ON a.projectno = k.projectno AND a.LOGCELLPK = k.logcellfk and g.CELLAYKEY=k.GSM1CELLLAYERFK 															
															
                                inner join logicalantenna la on la.PROJECTno=k.PROJECTno and la.LOGANTENNAPK =k.LOGANTENNAFK    -- ?!?!??            															
                                --inner join network_planning.FEEDER ff  on k.FEEDERKEY = ff.FEEDERKEY     -- on k.PROJECTNO = ff.PROJECTNO --- PROJECT IS WRONG !!!!!!!															
                                --inner join logicalantenna la on la.PROJECTno=ff.PROJECTno and la.LOGANTENNAPK =ff.FEEDERKEY    -- ?!?!??															
                                INNER JOIN phyantenna pa ON pa.projectno = la.projectno and pa.PHYANTENNApK = la.PHYANTENNAfk															
															
                                inner join antennadevice ad on ad.PROJECTNO = pa.PROJECTNO and ad.DEVICEPK = pa.DEVICEFK 															
                                INNER JOIN  ANTENNAPATTERN ap ON la.PROJECTno = ap.PROJECTno and la.anttypefk = ap.patternpk   															
                                inner join ANTENNAPATTERN ap1 on ap1.PROJECTNO = pa.PROJECTNO and ap1.PATTERNPK = pa.MASTERPATTERN1FK															
                                inner join ANTENNAPATTERN ap2 on ap2.PROJECTNO = pa.PROJECTNO and ap2.PATTERNPK = pa.MASTERPATTERN2FK															
                                inner join ANTENNAPATTERN ap3 on ap3.PROJECTNO = pa.PROJECTNO and ap3.PATTERNPK = pa.MASTERPATTERN3FK															
                                inner join ANTENNAPATTERN ap4 on ap4.PROJECTNO = pa.PROJECTNO and ap4.PATTERNPK = pa.MASTERPATTERN4FK															
															
                                            INNER JOIN  network_planning.FLAGVALUES z ON c.projectno = z.projectno AND c.LOGNODEPK = z.objectkey 															
                                            INNER JOIN  network_planning.FLAGS y ON y.PROJECTNO = z.PROJECTNO AND y.FLAGGROUPKEY = z.FLAGGROUPKEY AND y.FLAGKEY = z.FLAGKEY 															
                                            INNER JOIN  network_planning.FLAGGROUPS x ON x.PROJECTNO = y.PROJECTNO AND x.FLAGGROUPKEY = y.FLAGGROUPKEY															
															
														
                                where(x.flaggroupid = 'Candidate')															
                                AND(y.flagid = 'Accepted')															
                                AND(a.PROJECTNO = 1) AND(b.GSMID > 0)
                                and d.idname like '%{siteID}%'").ToList();
                return eTilts;
            }

        }

        private List<ViewModelTechnology> EtiltAsset3G()
        {
            using (var contextAsset = new Entities())
            {
                var eTilts = contextAsset.Database.SqlQuery<ViewModelTechnology>($@"
                        select  

                        b1.umtscellid as CellID, c2.IDNAME AS CellName, la.INDEXNO as LOGINDEX,	

                        case when la.INHERITMASTERPATTERN=0 then ap.downtilt
                        when la.INHERITMASTERPATTERN=1 then ap1.downtilt 
                        when la.INHERITMASTERPATTERN=2 then ap2.downtilt
                        when la.INHERITMASTERPATTERN=3 then ap3.downtilt
                        when la.INHERITMASTERPATTERN=4 then ap4.downtilt
                        end AS Etilt

                        from  network_planning.LOGUMTSCELL b1 
                        INNER JOIN network_planning.LOGCELL c2 ON b1.PROJECTNO = c2.PROJECTNO AND b1.LOGCELLFK = c2.LOGCELLPK 
                        INNER JOIN network_planning.LOGNODE c1 ON c2.PROJECTNO = c1.PROJECTNO AND c2.LOGNODEFK = c1.LOGNODEPK 
                        INNER JOIN network_planning.LOGUMTSCELLCAR ucc ON b1.PROJECTNO = ucc.PROJECTNO AND b1.LOGCELLfK = ucc.LOGCELLFK 
                        INNER JOIN network_planning.LOGUMTSCAR uc ON ucc.projectno = uc.projectno AND ucc.CARRIERFK = uc.UMTSCARPK 
                        INNER JOIN network_planning.TGCARRIER ut ON uc.projectno = ut.projectno AND uc.tgcarrierfk = ut.carrierkey 
                        INNER JOIN network_planning.SITEADDRESS h ON c1.projectno = h.projectno AND c1.addressfk = h.addresskey 
                        INNER JOIN network_planning.LOGCONNECTION d1 ON c1.PROJECTNO = d1.PROJECTNO AND c1.LOGNODEPK = d1.LOGNODEBFK 
                        INNER JOIN network_planning.LOGRNC e1 ON d1.PROJECTNO = e1.PROJECTNO AND d1.lognodeafk = e1.lognodepk 
                        INNER JOIN network_planning.LOGcellFEEDER f ON b1.PROJECTNO = f.projectno AND b1.LOGCELLfk = f.logcellfk 

                        inner join logicalantenna la on la.PROJECTno=f.PROJECTno and la.LOGANTENNAPK =f.LOGANTENNAFK
                        inner join phyantenna pa on pa.PROJECTNO = la.PROJECTNO and pa.PHYANTENNAPK = la.PHYANTENNAFK
                        INNER JOIN  ANTENNAPATTERN ap  on ap.PROJECTNO = la.PROJECTNO and la.anttypefk = ap.patternpk 
                        inner join ANTENNAPATTERN ap1 on ap1.PROJECTNO = pa.PROJECTNO and ap1.PATTERNPK = pa.MASTERPATTERN1FK
                        inner join ANTENNAPATTERN ap2 on ap2.PROJECTNO = pa.PROJECTNO and ap2.PATTERNPK = pa.MASTERPATTERN2FK
                        inner join ANTENNAPATTERN ap3 on ap3.PROJECTNO = pa.PROJECTNO and ap3.PATTERNPK = pa.MASTERPATTERN3FK
                        inner join ANTENNAPATTERN ap4 on ap4.PROJECTNO = pa.PROJECTNO and ap4.PATTERNPK = pa.MASTERPATTERN4FK

                        INNER JOIN network_planning.FLAGVALUES z ON c1.projectno = z.projectno AND c1.lognodepk = z.objectkey 
                        INNER JOIN network_planning.FLAGS y ON y.PROJECTNO = z.PROJECTNO AND y.FLAGGROUPKEY = z.FLAGGROUPKEY AND y.FLAGKEY = z.FLAGKEY 
                        INNER JOIN network_planning.FLAGGROUPS x ON x.PROJECTNO = y.PROJECTNO AND x.FLAGGROUPKEY = y.FLAGGROUPKEY

                        INNER JOIN network_planning.FLAGVALUES z1 ON c1.projectno = z1.projectno AND c2.logcellpk = z1.objectkey 
                        INNER JOIN network_planning.FLAGS y1 ON y1.PROJECTNO = z.PROJECTNO AND y1.FLAGGROUPKEY = z1.FLAGGROUPKEY AND y1.FLAGKEY = z1.FLAGKEY 
                        INNER JOIN network_planning.FLAGGROUPS x1 ON x1.PROJECTNO = y1.PROJECTNO AND x1.FLAGGROUPKEY = y1.FLAGGROUPKEY

                        where 
                        (x.flaggroupid = 'Candidate')  AND (y.flagid ='Accepted') 
                        AND (x1.flaggroupid = 'Site Progress') AND (y1.flagid ='On Air') 
                        AND (b1.PROJECTNO = 1) 
                        and c1.idname like '%{siteID}%'").ToList();
                return eTilts;
            }

        }

        private List<ViewModelTechnology> EtiltAsset4G()
        {
            using (var contextAsset = new Entities())
            {
                var eTilts = contextAsset.Database.SqlQuery<ViewModelTechnology>($@"
                    select 
                    a.cellid as CellID, b4.idname as CellName,la.INDEXNO as LOGINDEX,	

                    case when la.INHERITMASTERPATTERN=0 then ap.downtilt
                    when la.INHERITMASTERPATTERN=1 then ap1.downtilt 
                    when la.INHERITMASTERPATTERN=2 then ap2.downtilt
                    when la.INHERITMASTERPATTERN=3 then ap3.downtilt
                    when la.INHERITMASTERPATTERN=4 then ap4.downtilt
                    end AS Etilt


                    from LOGLTECELL a       
                    INNER JOIN LogLteCellCar a1 ON a1.PROJECTNO =a.PROJECTNO AND a1.logcellfk = a.LOGCELLFK    
                    INNER JOIN logcell b4 ON b4.PROJECTNO =a.PROJECTNO AND b4.LOGCELLPK = a.LOGCELLFK   
                    INNER JOIN LOGNODE b5 ON b4.PROJECTNO = b5.PROJECTNO AND b4.LOGNODEFK = b5.LOGNODEPK
                    inner join network_planning.SITEADDRESS h ON b5.projectno = h.projectno AND b5.addressfk = h.addresskey
                    INNER JOIN network_planning.LOGcellFEEDER f ON a.PROJECTNO = f.projectno AND a.LOGCELLFK = f.LOGCELLFK 

                    inner join logicalantenna la on la.PROJECTno=f.PROJECTno and la.LOGANTENNAPK =f.LOGANTENNAfK
                    inner join phyantenna pa on pa.PROJECTNO = la.PROJECTNO and pa.PHYANTENNAPK = la.PHYANTENNAFK
                    INNER JOIN  ANTENNAPATTERN ap  on ap.PROJECTNO = la.PROJECTNO and la.anttypefk = ap.patternpk 
                    inner join ANTENNAPATTERN ap1 on ap1.PROJECTNO = pa.PROJECTNO and ap1.PATTERNPK = pa.MASTERPATTERN1FK
                    inner join ANTENNAPATTERN ap2 on ap2.PROJECTNO = pa.PROJECTNO and ap2.PATTERNPK = pa.MASTERPATTERN2FK
                    inner join ANTENNAPATTERN ap3 on ap3.PROJECTNO = pa.PROJECTNO and ap3.PATTERNPK = pa.MASTERPATTERN3FK
                    inner join ANTENNAPATTERN ap4 on ap4.PROJECTNO = pa.PROJECTNO and ap4.PATTERNPK = pa.MASTERPATTERN4FK


                    inner join network_planning.FLAGVALUES z ON b5.projectno = z.projectno AND b5.lognodepk = z.objectkey 
                    INNER JOIN network_planning.FLAGS y ON y.PROJECTNO = z.PROJECTNO AND y.FLAGGROUPKEY = z.FLAGGROUPKEY AND y.FLAGKEY = z.FLAGKEY 
                    INNER JOIN network_planning.FLAGGROUPS x ON x.PROJECTNO = y.PROJECTNO AND x.FLAGGROUPKEY = y.FLAGGROUPKEY


                    where (x.flaggroupid = 'Candidate')  
                    AND (y.flagid ='Accepted') 
                    AND (a.PROJECTNO = 1) 
                    and b5.idname like '%{siteID}%'").ToList();
                return eTilts;
            }

        }

        private List<ViewModelTechnology> Get2GSiteAsset()
        {

            using (var contextAsset = new Entities())
            {
                var outputGSM = (
                    from gsmCell in contextAsset.GSMCELLs
                    join logCell in contextAsset.LOGCELLs on
                        new { prj = gsmCell.PROJECTNO, logCell1 = gsmCell.LOGCELLFK }
                        equals new { prj = logCell.PROJECTNO, logCell1 = logCell.LOGCELLPK }
                    join muNode in contextAsset.MUNODEs on
                        new { prj = logCell.PROJECTNO, logNode1 = logCell.LOGNODEFK }
                        equals new { prj = muNode.PROJECTNO, logNode1 = muNode.LOGNODEPK }
                    join logNode in contextAsset.LOGNODEs on
                        new { prj = muNode.PROJECTNO, LOGNODEPK = muNode.LOGNODEPK }
                        equals new { prj = logNode.PROJECTNO, LOGNODEPK = logNode.LOGNODEPK }
                    join siteAddress in contextAsset.SITEADDRESSes on
                        new { prj = logNode.PROJECTNO, ADDRESS = logNode.ADDRESSFK }
                        equals new { prj = siteAddress.PROJECTNO, ADDRESS = siteAddress.ADDRESSKEY }
                    join cellayData in contextAsset.CELLAYDATAs on
                        new { prj = gsmCell.PROJECTNO, CELL = gsmCell.LOGCELLFK }
                        equals new { prj = cellayData.PROJECTNO, CELL = cellayData.CELLKEY }
                    join logcellfeeder in contextAsset.LOGCELLFEEDERs on
                        new { prj = logCell.PROJECTNO, LOGCELL = logCell.LOGCELLPK, CELLAYKEY = cellayData.CELLAYKEY }
                        equals new { prj = logcellfeeder.PROJECTNO, LOGCELL = logcellfeeder.LOGCELLFK, CELLAYKEY = logcellfeeder.GSM1CELLLAYERFK }
                    join feeder in contextAsset.FEEDERs on logcellfeeder.FEEDERFK equals feeder.FEEDERKEY
                    join logicalantenna in contextAsset.LOGICALANTENNAs on
                        new { prj = logcellfeeder.PROJECTNO, LOGANTENNA = logcellfeeder.LOGANTENNAFK }
                        equals new { prj = logicalantenna.PROJECTNO, LOGANTENNA = logicalantenna.LOGANTENNAPK }
                    join phyantenna in contextAsset.PHYANTENNAs on
                        new { prj = logicalantenna.PROJECTNO, PHYANTENNA = logicalantenna.PHYANTENNAFK }
                        equals new { prj = phyantenna.PROJECTNO, PHYANTENNA = phyantenna.PHYANTENNAPK }
                    join antennaDevice in contextAsset.ANTENNADEVICEs on
                        new { prj = phyantenna.PROJECTNO, DEVICE = (decimal)phyantenna.DEVICEFK }
                        equals new { prj = antennaDevice.PROJECTNO, DEVICE = antennaDevice.DEVICEPK }
                    join logconnection in contextAsset.LOGCONNECTIONs
                        on new { prj = logNode.PROJECTNO, logconnection = logNode.LOGNODEPK }
                        equals new { prj = logconnection.PROJECTNO, logconnection = logconnection.LOGNODEBFK }
                    join bsc in contextAsset.BSCs
                        on new { prj = logconnection.PROJECTNO, lognode = logconnection.LOGNODEAFK }
                        equals new { prj = bsc.PROJECTNO, lognode = bsc.LOGNODEPK }


                    join flag in contextAsset.FLAGVALUES on new { logNode.PROJECTNO, flag = logNode.LOGNODEPK } equals new { flag.PROJECTNO, flag = flag.OBJECTKEY }
                    join flag1 in contextAsset.FLAGVALUES on new { gsmCell.PROJECTNO, flag1 = gsmCell.LOGCELLFK } equals new { flag1.PROJECTNO, flag1 = flag1.OBJECTKEY }
                    join flag2 in contextAsset.FLAGVALUES on new { gsmCell.PROJECTNO, flag2 = gsmCell.LOGCELLFK } equals new { flag2.PROJECTNO, flag2 = flag2.OBJECTKEY }
                    join flag3 in contextAsset.FLAGVALUES on new { gsmCell.PROJECTNO, flag3 = gsmCell.LOGCELLFK } equals new { flag3.PROJECTNO, flag3 = flag3.OBJECTKEY }
                    join flag4 in contextAsset.FLAGVALUES on new { gsmCell.PROJECTNO, flag4 = gsmCell.LOGCELLFK } equals new { flag4.PROJECTNO, flag4 = flag4.OBJECTKEY }
                    join flag5 in contextAsset.FLAGVALUES on new { logNode.PROJECTNO, flag5 = logNode.LOGNODEPK } equals new { flag5.PROJECTNO, flag5 = flag5.OBJECTKEY }
                    join flag6 in contextAsset.FLAGVALUES on new { gsmCell.PROJECTNO, flag6 = gsmCell.LOGCELLFK } equals new { flag6.PROJECTNO, flag6 = flag6.OBJECTKEY }

                    where siteAddress.PROJECTNO == 1 && siteAddress.IDNAME.Contains(siteID)
                    && flag.FLAGGROUP.FLAGGROUPID == "Candidate" && flag.FLAG.FLAGID == "Accepted"
                    && flag1.FLAGGROUP.FLAGGROUPID == "TMA"
                    && flag2.FLAGGROUP.FLAGGROUPID == "RRU type"
                    && flag3.FLAGGROUP.FLAGGROUPID == "Combiner/Splitter"
                    && flag4.FLAGGROUP.FLAGGROUPID == "2nd Combiner"
                    && flag5.FLAGGROUP.FLAGGROUPID == "Co-Location"
                    && flag6.FLAGGROUP.FLAGGROUPID == "Antenna Mounting"



                    select new ViewModelTechnology
                    {
                        Controler = bsc.LOGNODE.IDNAME,
                        SiteID = siteAddress.IDNAME,
                        Candidate = siteAddress.IDNAME.Substring(10, 1),
                        SiteName = logNode.NAME,
                        SiteAddress = siteAddress.TOWN,
                        SiteAddress1 = siteAddress.ADDRESS1,
                        SiteAddress2 = siteAddress.ADDRESS2,
                        AntennaType = antennaDevice.IDNAME,
                        PHYINDEX = phyantenna.PHYINDEX,
                        Azimuth = phyantenna.AZIMUTH,
                        AGL = phyantenna.HEIGHT,
                        ARTL = phyantenna.HEIGHTOFFSET,
                        MECHANICAL_TILT = phyantenna.TILT,

                        //New Add Port binding 13.02.2018
                        LOGINDEX = logicalantenna.INDEXNO,
                        PortNumber = logicalantenna.PORTS,


                        FEEDERLENGTH = logcellfeeder.FEEDERLEN,
                        FEEDERTYPE = feeder.IDNAME,
                        CellName = logCell.IDNAME,
                        Sector = logCell.IDNAME.Substring(4, 1),
                        RRU_Type = flag2.FLAG.FLAGID,

                        //12.04.2017 New way of GSM TRX counting. You don’t have to assign carriers anymore.
                        GSM_TRX = cellayData.CARLAYDATAs.Sum(n => n.TRXREQUIRED),
                        GSM_Pwr_per_TRX = cellayData.OUTPUTPOWER,
                        UMTS_TRX = 0,
                        UMTS_Pwr_per_TRX = 0,
                        LTE_TRX = 0,
                        LTE_Pwr_per_TRX = 0,
                        NR_TRX = 0,
                        NR_Pwr_per_TRX = 0,
                        LAYER_TECHNOLOGY = "G",
                        Band = cellayData.CELLAY.IDNAME.Contains("900") ? "900" : "1800",
                        TMA = flag1.FLAG.FLAGID,
                        COMBINER_SPLITTER = flag3.FLAG.FLAGID,
                        SEC_COMBINER_SPLITTER = flag4.FLAG.FLAGID,
                        CoLocation = flag5.FLAG.FLAGID,
                        ANTENNA_MOUNT = flag6.FLAG.FLAGID,
                        CellID = gsmCell.GSMID,

                    }

                ).Distinct();
                var tech2G = outputGSM.ToList();

                foreach (var tech in tech2G)
                {
                    if (tech.Band == "900")
                        tech.GSM_TRX = 1;
                    else if (tech.Band == "1800")
                        tech.GSM_TRX = 0;
                }

                return tech2G;

            }
        }

        private List<ViewModelTechnology> Get3GSiteAsset()
        {
            using (var contextAsset = new Entities())
            {
                var outputUMTS = (

                  from siteaddress in contextAsset.SITEADDRESSes
                  from lognode in siteaddress.LOGNODEs
                  from logcell in lognode.LOGCELLs
                  from cell in logcell.LOGUMTSCELLs
                  from logcellfeeder in logcell.LOGCELLFEEDERs

                  join feeder in contextAsset.FEEDERs on logcellfeeder.FEEDERFK equals feeder.FEEDERKEY
                  join logicalantenna in contextAsset.LOGICALANTENNAs on logcellfeeder.LOGANTENNAFK equals logicalantenna.LOGANTENNAPK
                  join phyantenna in contextAsset.PHYANTENNAs on logicalantenna.PHYANTENNAFK equals phyantenna.PHYANTENNAPK
                  join antennaDevice in contextAsset.ANTENNADEVICEs on phyantenna.DEVICEFK equals antennaDevice.DEVICEPK


                  from cellcar in cell.LOGUMTSCELLCARs

                  join tma in contextAsset.MASTHEADAMPs on logcellfeeder.MHAFK equals tma.MHAKEY into tmaLeftJoin
                  from tma in tmaLeftJoin.DefaultIfEmpty()

                      //RNC part 
                  join munode in contextAsset.MUNODEs
                      on new { prj = logcell.PROJECTNO, lognode = logcell.LOGNODEFK }
                      equals new { prj = munode.PROJECTNO, lognode = munode.LOGNODEPK }

                  join lognode1 in contextAsset.LOGNODEs
                  on new { prj = munode.PROJECTNO, lognode = munode.LOGNODE.LOGNODEPK }
                  equals new { prj = lognode1.PROJECTNO, lognode = lognode1.LOGNODEPK }

                  join logconnection in contextAsset.LOGCONNECTIONs
                  on new { prj = lognode1.PROJECTNO, logconnection = lognode1.LOGNODEPK }
                  equals new { prj = logconnection.PROJECTNO, logconnection = logconnection.LOGNODEBFK }

                  join rnc in contextAsset.LOGRNCs
                  on new { prj = logconnection.PROJECTNO, lognode = logconnection.LOGNODEAFK }
                  equals new { prj = rnc.PROJECTNO, lognode = rnc.LOGNODEPK }
                  //RNC part


                  join flag in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag = lognode.LOGNODEPK } equals new { flag.PROJECTNO, flag = flag.OBJECTKEY }
                  join flag1 in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag1 = logcell.LOGCELLPK } equals new { flag1.PROJECTNO, flag1 = flag1.OBJECTKEY }
                  join flag2 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag2 = cell.LOGCELLFK } equals new { flag2.PROJECTNO, flag2 = flag2.OBJECTKEY }
                  join flag3 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag3 = cell.LOGCELLFK } equals new { flag3.PROJECTNO, flag3 = flag3.OBJECTKEY }
                  join flag4 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag4 = cell.LOGCELLFK } equals new { flag4.PROJECTNO, flag4 = flag4.OBJECTKEY }
                  join flag5 in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag5 = lognode.LOGNODEPK } equals new { flag5.PROJECTNO, flag5 = flag5.OBJECTKEY }
                  join flag6 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag6 = cell.LOGCELLFK } equals new { flag6.PROJECTNO, flag6 = flag6.OBJECTKEY }

                  where lognode.SITEADDRESS.PROJECTNO == 1 && lognode.SITEADDRESS.IDNAME.Contains(siteID)
                  && flag.FLAGGROUP.FLAGGROUPID == "Candidate" && flag.FLAG.FLAGID == "Accepted"
                  && flag1.FLAGGROUP.FLAGGROUPID == "Site Progress" && flag1.FLAG.FLAGID == "On Air"
                  && flag2.FLAGGROUP.FLAGGROUPID == "RRU type"
                  && flag3.FLAGGROUP.FLAGGROUPID == "Combiner/Splitter"
                  && flag4.FLAGGROUP.FLAGGROUPID == "2nd Combiner"
                  && flag5.FLAGGROUP.FLAGGROUPID == "Co-Location"
                  && flag6.FLAGGROUP.FLAGGROUPID == "Antenna Mounting"

                
                select new ViewModelTechnology
                  {
                      Controler = rnc.LOGNODE.IDNAME,
                      SiteID = siteaddress.IDNAME,
                      Candidate = lognode.SITEADDRESS.IDNAME.Substring(10, 1),
                      SiteName = lognode.NAME,
                      SiteAddress = siteaddress.TOWN,
                      SiteAddress1 = siteaddress.ADDRESS1,
                      SiteAddress2 = siteaddress.ADDRESS2,
                      AntennaType = antennaDevice.IDNAME,
                      PHYINDEX = phyantenna.PHYINDEX,
                      Azimuth = phyantenna.AZIMUTH,
                      AGL = phyantenna.HEIGHT,
                      ARTL = phyantenna.HEIGHTOFFSET,
                      MECHANICAL_TILT = phyantenna.TILT,

                      //New Add Port binding 13.02.2018
                      //LOGINDEX = umtsfeeder.LOGICALANTENNA.INDEXNO,
                      //PortNumber = umtsfeeder.LOGICALANTENNA.PORTS,

                      FEEDERLENGTH = logcellfeeder.FEEDERLEN,
                      FEEDERTYPE = feeder.IDNAME,
                      CellName = logcell.IDNAME,
                      Sector = logcell.IDNAME.Substring(4, 1),
                      RRU_Type = flag2.FLAG.FLAGID,
                      GSM_TRX = 0,
                      GSM_Pwr_per_TRX = 0,
                      UMTS_TRX = 1,
                      UMTS_Pwr_per_TRX = cellcar.MAXTXPOWER,
                      LTE_TRX = 0,
                      LTE_Pwr_per_TRX = 0,
                      NR_TRX = 0,
                      NR_Pwr_per_TRX = 0,
                      LAYER_TECHNOLOGY = "U",
                      Band = cellcar.LOGUMTSCAR.TGCARRIER.DOWNLINKCH > 10000 ? "2100" : "900",
                      TMA = tma.IDNAME,
                      COMBINER_SPLITTER = flag3.FLAG.FLAGID,
                      SEC_COMBINER_SPLITTER = flag4.FLAG.FLAGID,
                      CoLocation = flag5.FLAG.FLAGID,
                      ANTENNA_MOUNT = flag6.FLAG.FLAGID,
                      CellID = cell.UMTSCELLID

                  }

                  ).Distinct().ToList();

                return outputUMTS;
            }
        }

        public List<ViewModelTechnology> Get4GSiteAsset()
        {
            using (var contextAsset = new Entities())
            {
                var outputLTE = (

                 from siteaddress in contextAsset.SITEADDRESSes
                 from lognode in siteaddress.LOGNODEs
                 from logcell in lognode.LOGCELLs
                 from cell in logcell.LOGLTECELLs
                 from cellcar in cell.LOGLTECELLCARs

                 from ltefeeder in logcell.LOGCELLFEEDERs
                 join feeder in contextAsset.FEEDERs on ltefeeder.FEEDERFK equals feeder.FEEDERKEY

                 join antennaDevice in contextAsset.ANTENNADEVICEs on ltefeeder.LOGICALANTENNA.PHYANTENNA.DEVICEFK equals antennaDevice.DEVICEPK

                 join tma in contextAsset.MASTHEADAMPs on ltefeeder.MHAFK equals tma.MHAKEY into tmaLeftJoin
                 from tma in tmaLeftJoin.DefaultIfEmpty()


                 join flag in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag = lognode.LOGNODEPK } equals new { flag.PROJECTNO, flag = flag.OBJECTKEY }
                 join flag2 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag2 = cell.LOGCELLFK } equals new { flag2.PROJECTNO, flag2 = flag2.OBJECTKEY }
                 join flag3 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag3 = cell.LOGCELLFK } equals new { flag3.PROJECTNO, flag3 = flag3.OBJECTKEY }
                 join flag4 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag4 = cell.LOGCELLFK } equals new { flag4.PROJECTNO, flag4 = flag4.OBJECTKEY }
                 join flag5 in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag5 = lognode.LOGNODEPK } equals new { flag5.PROJECTNO, flag5 = flag5.OBJECTKEY }
                 join flag6 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag6 = cell.LOGCELLFK } equals new { flag6.PROJECTNO, flag6 = flag6.OBJECTKEY }

                 where lognode.SITEADDRESS.PROJECTNO == 1 && lognode.SITEADDRESS.IDNAME.Contains(siteID)
                 && flag.FLAGGROUP.FLAGGROUPID == "Candidate" && flag.FLAG.FLAGID == "Accepted"
                 && flag2.FLAGGROUP.FLAGGROUPID == "RRU type"
                 && flag3.FLAGGROUP.FLAGGROUPID == "Combiner/Splitter"
                 && flag4.FLAGGROUP.FLAGGROUPID == "2nd Combiner"
                 && flag5.FLAGGROUP.FLAGGROUPID == "Co-Location"
                 && flag6.FLAGGROUP.FLAGGROUPID == "Antenna Mounting"


                 select new ViewModelTechnology
                 {
                     Bandwidth = cellcar.LOGLTECAR.LTECARRIER.BANDWIDTH_MHZ,
                     Controler = " ",
                     SiteID = siteaddress.IDNAME,
                     Candidate = lognode.SITEADDRESS.IDNAME.Substring(10, 1),
                     SiteName = lognode.NAME,
                     SiteAddress = siteaddress.TOWN,
                     SiteAddress1 = siteaddress.ADDRESS1,
                     SiteAddress2 = siteaddress.ADDRESS2,
                     AntennaType = antennaDevice.IDNAME,
                     PHYINDEX = ltefeeder.LOGICALANTENNA.PHYANTENNA.PHYINDEX,
                     Azimuth = ltefeeder.LOGICALANTENNA.PHYANTENNA.AZIMUTH,
                     AGL = ltefeeder.LOGICALANTENNA.PHYANTENNA.HEIGHT,
                     ARTL = ltefeeder.LOGICALANTENNA.PHYANTENNA.HEIGHTOFFSET,
                     MECHANICAL_TILT = ltefeeder.LOGICALANTENNA.PHYANTENNA.TILT,

                     //New Add Port binding 13.02.2018
                     LOGINDEX = ltefeeder.LOGICALANTENNA.INDEXNO,
                     PortNumber = ltefeeder.LOGICALANTENNA.PORTS,

                     FEEDERLENGTH = ltefeeder.FEEDERLEN,
                     FEEDERTYPE = feeder.IDNAME,
                     CellName = logcell.IDNAME,
                     Sector = logcell.IDNAME.Substring(4, 1),
                     RRU_Type = flag2.FLAG.FLAGID,
                     GSM_TRX = 0,
                     GSM_Pwr_per_TRX = 0,
                     UMTS_TRX = 0,
                     UMTS_Pwr_per_TRX = 0,
                     LTE_TRX = 1,
                     LTE_Pwr_per_TRX = cellcar.REFERENCESIGNALPPRE,
                     NR_TRX = 0,
                     NR_Pwr_per_TRX = 0,
                     LAYER_TECHNOLOGY = "L",
                     Band = cellcar.LOGLTECAR.LTECARRIER.IDNAME.Contains("1800") ? "1800" :
                            cellcar.LOGLTECAR.LTECARRIER.IDNAME.Contains("900") ? "900" :
                            cellcar.LOGLTECAR.LTECARRIER.IDNAME.Contains("2100") ? "2100" : "2600",
                     TMA = tma.IDNAME,
                     COMBINER_SPLITTER = flag3.FLAG.FLAGID,
                     SEC_COMBINER_SPLITTER = flag4.FLAG.FLAGID,
                     CoLocation = flag5.FLAG.FLAGID,
                     ANTENNA_MOUNT = flag6.FLAG.FLAGID,
                     CellID = cell.CELLID
                 }

                 ).Distinct().ToList();

                foreach (var lte in outputLTE)
                    if (lte.Bandwidth != null && lte.LTE_Pwr_per_TRX != null)
                        lte.LTE_Pwr_per_TRX = (decimal?)SupportFunc.ConvertW_dBm((SupportFunc.ConvertdBm_W((double)lte.LTE_Pwr_per_TRX) * 5 * (double)lte.Bandwidth * 12)); 
             
                return outputLTE;
            }
        }

        //public List<ViewModelTechnology> Get5GSiteAsset()
        //{
        //    var querie = Properties.Resources.QueryAsset10_5G;
        //    querie = querie.Replace("@@@@@@", this.siteID);


        //    using (var db = new Entities())
        //    {

        //        var site_obj = db.Database.SqlQuery<ViewModelTechnology>(querie)
        //       .ToList();

        //        //DSS
        //        foreach (var tech in site_obj)
        //            if (tech.Band == "1800" || tech.Band == "2100")
        //                tech.NR_TRX = 0;

        //        return site_obj;
        //    }
        //}

        //public List<ViewModelTechnology> Get5GSiteAsset_EF()
        //{

        //    using (var contextAsset = new Entities())
        //    {
        //        var outputNR = (

        //         from siteaddress in contextAsset.SITEADDRESSes
        //         from lognode in siteaddress.LOGNODEs
        //         where lognode.PROJECTNO == 1
        //         from logcell in lognode.LOGCELLs
        //         where logcell.PROJECTNO == 1
        //         from cell in logcell.FIVEGCELLPARAMS
        //         where cell.PROJECTNO == 1
        //         from cellcar in logcell.FIVEGCELLCARs
        //         where cellcar.PROJECTNO == 1

        //         //join fiveGfeeder in contextAsset.LOGCELLFEEDERs on cell.LOGCELLFK equals fiveGfeeder.LOGCELLFK
        //         //where fiveGfeeder.PROJECTNO == 1
        //         //join feeder in contextAsset.FEEDERs on fiveGfeeder.FEEDERFK equals feeder.FEEDERKEY
        //         //where feeder.PROJECTNO == 1
        //         //join logAntenna in contextAsset.LOGICALANTENNAs on fiveGfeeder.LOGANTENNAFK equals logAntenna.LOGANTENNAPK
        //         //where logAntenna.PROJECTNO == 1
        //         //join phyAntenna in contextAsset.PHYANTENNAs on logAntenna.PHYANTENNAFK equals phyAntenna.PHYANTENNAPK
        //         //where phyAntenna.PROJECTNO == 1
        //         //join antennaDevice in contextAsset.ANTENNADEVICEs on phyAntenna.DEVICEFK equals antennaDevice.DEVICEPK
        //         //where antennaDevice.PROJECTNO == 1

        //         //join antennaPattern in contextAsset.ANTENNAPATTERNs on logAntenna.ANTTYPEFK equals antennaPattern.PATTERNPK   where antennaPattern.PROJECTNO == 1
        //         //join antennaPattern1 in contextAsset.ANTENNAPATTERNs on phyAntenna.MASTERPATTERN1FK equals antennaPattern1.PATTERNPK   where antennaPattern1.PROJECTNO == 1
        //         //join antennaPattern2 in contextAsset.ANTENNAPATTERNs on phyAntenna.MASTERPATTERN2FK equals antennaPattern2.PATTERNPK   where antennaPattern2.PROJECTNO == 1
        //         //join antennaPattern3 in contextAsset.ANTENNAPATTERNs on phyAntenna.MASTERPATTERN3FK equals antennaPattern3.PATTERNPK   where antennaPattern3.PROJECTNO == 1
        //         //join antennaPattern4 in contextAsset.ANTENNAPATTERNs on phyAntenna.MASTERPATTERN4FK equals antennaPattern4.PATTERNPK   where antennaPattern4.PROJECTNO == 1

        //         //join fiveGNodeCar in contextAsset.FIVEGNODECARs on lognode.LOGNODEPK equals fiveGNodeCar.LOGNODEFK where fiveGNodeCar.PROJECTNO == 1
        //         //join munode in contextAsset.MUNODEs on fiveGNodeCar.LOGNODEFK equals munode.LOGNODEPK where munode.PROJECTNO == 1
        //         //join fiveGCar in contextAsset.FIVEGCARRIERs on fiveGNodeCar.CONFIGCARRIERFK equals fiveGCar.CARRIERKEY where fiveGCar.PROJECTNO == 1


        //         //join flag in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag = lognode.LOGNODEPK } equals new { flag.PROJECTNO, flag = flag.OBJECTKEY }
        //         //join flag2 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag2 = cell.LOGCELLFK } equals new { flag2.PROJECTNO, flag2 = flag2.OBJECTKEY }
        //         //join flag3 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag3 = cell.LOGCELLFK } equals new { flag3.PROJECTNO, flag3 = flag3.OBJECTKEY }
        //         //join flag4 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag4 = cell.LOGCELLFK } equals new { flag4.PROJECTNO, flag4 = flag4.OBJECTKEY }
        //         //join flag5 in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag5 = lognode.LOGNODEPK } equals new { flag5.PROJECTNO, flag5 = flag5.OBJECTKEY }
        //         //join flag6 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag6 = cell.LOGCELLFK } equals new { flag6.PROJECTNO, flag6 = flag6.OBJECTKEY }
        //         //join flag7 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag7 = cell.LOGCELLFK } equals new { flag7.PROJECTNO, flag7 = flag7.OBJECTKEY }

        //         //where lognode.SITEADDRESS.PROJECTNO == 1 && lognode.SITEADDRESS.IDNAME.Contains(siteID)
        //         //&& flag.FLAGGROUP.FLAGGROUPID == "Candidate" && flag.FLAG.FLAGID == "Accepted"
        //         //&& flag2.FLAGGROUP.FLAGGROUPID == "RRU type"
        //         //&& flag3.FLAGGROUP.FLAGGROUPID == "Combiner/Splitter"
        //         //&& flag4.FLAGGROUP.FLAGGROUPID == "2nd Combiner"
        //         //&& flag5.FLAGGROUP.FLAGGROUPID == "Co-Location"
        //         //&& flag6.FLAGGROUP.FLAGGROUPID == "Antenna Mounting"
        //         //&& flag7.FLAGGROUP.FLAGGROUPID == "TMA"

        //         select new ViewModelTechnology
        //         {
        //             Controler = " ",
        //             SiteID = siteaddress.IDNAME,
        //             Candidate = lognode.SITEADDRESS.IDNAME.Substring(10, 1),
        //             SiteName = lognode.NAME,
        //             SiteAddress = siteaddress.TOWN,
        //             SiteAddress1 = siteaddress.ADDRESS1,
        //             SiteAddress2 = siteaddress.ADDRESS2,
        //             //AntennaType = antennaDevice.IDNAME,
        //             //PHYINDEX = phyAntenna.PHYINDEX,
        //             //Azimuth = phyAntenna.AZIMUTH,
        //             //AGL = phyAntenna.HEIGHT,
        //             //ARTL = phyAntenna.HEIGHTOFFSET,
        //             //MECHANICAL_TILT = phyAntenna.TILT,

        //             //New Add Port binding 13.02.2018
        //             //LOGINDEX = logAntenna.INDEXNO,
        //             //PortNumber = logAntenna.PORTS,

        //             //Etilt = (logAntenna.INHERITMASTERPATTERN == 0) ? antennaPattern.DOWNTILT:
        //             //(logAntenna.INHERITMASTERPATTERN == 1) ? antennaPattern1.DOWNTILT : 
        //             //(logAntenna.INHERITMASTERPATTERN == 2) ? antennaPattern2.DOWNTILT :
        //             //(logAntenna.INHERITMASTERPATTERN == 3) ? antennaPattern3.DOWNTILT : antennaPattern4.DOWNTILT,

        //             //FEEDERLENGTH = fiveGfeeder.FEEDERLEN,
        //             //FEEDERTYPE = feeder.IDNAME,
        //             CellName = logcell.IDNAME,
        //             Sector = logcell.IDNAME.Substring(4, 1),
        //             //RRU_Type = flag2.FLAG.FLAGID,
        //             GSM_TRX = 0,
        //             GSM_Pwr_per_TRX = 0,
        //             UMTS_TRX = 0,
        //             UMTS_Pwr_per_TRX = 0,
        //             LTE_TRX = 1,
        //             LTE_Pwr_per_TRX = cellcar.MAXTXPOWER - (decimal)3.01,
        //             LAYER_TECHNOLOGY = "NR",
        //             //Band = fiveGCar.IDNAME.Contains("1800") ? "1800" :
        //             //       fiveGCar.IDNAME.Contains("900") ? "900" :
        //             //       fiveGCar.IDNAME.Contains("2100") ? "2100" : "2600",
        //             //TMA = flag7.FLAG.FLAGID,
        //             //COMBINER_SPLITTER = flag3.FLAG.FLAGID,
        //             //SEC_COMBINER_SPLITTER = flag4.FLAG.FLAGID,
        //             //CoLocation = flag5.FLAG.FLAGID,
        //             //ANTENNA_MOUNT = flag6.FLAG.FLAGID,
        //             CellID = cell.CELLID
        //         }

        //         ).Distinct();


        //        return outputNR.ToList();
        //    }


        //}

        public List<ViewModelTechnology> Get5GSiteAsset_Python()
        {
            var pythonAppPath = @"D:\Projects\SV\Python\OracleEtl\";
            var appName = Path.Combine(pythonAppPath, "oracle_etl.py");
            var outputJsonFileName = Path.Combine(pythonAppPath, "result.json");
            var config_path_file = Path.Combine(pythonAppPath, "config.yml");
            var query_path = @"D:\Projects\SV\Shiva_5G\BLL\Resources\QueryAsset10_5G.sql.txt";

            var args = string.Join(" ", new string[] { query_path, outputJsonFileName, config_path_file, this.siteID });

            var output = SupportFunc.RunFromCmd(appName, args);
            var jsonString = File.ReadAllText(outputJsonFileName);
            var lstObj = JsonConvert.DeserializeObject<List<ViewModelTechnology>>(jsonString);

            foreach (var item in lstObj)
            {
                //DSS
                if (item.Band == "1800" || item.Band == "2100")
                    item.NR_TRX = 0;
            }

            return lstObj;
        }

        private List<ViewModelTechnology> CombineAssetSiteInfoAndEtilt(List<ViewModelTechnology> siteInfo, List<ViewModelTechnology> etilts)
        {
            if (siteInfo.Count != etilts.Count)
            {

                var error = $"{Environment.NewLine}Possible error: More than one antenna for one technology.";

                throw new Exception(error);
            }

            foreach (var cell in siteInfo)
            {
                foreach (var etilt in etilts)
                {
                    if (cell.CellName == etilt.CellName && cell.CellID == etilt.CellID && cell.LOGINDEX == etilt.LOGINDEX)
                    {
                        cell.Etilt = etilt.Etilt;
                        break;
                    }

                }
            }
            return siteInfo;
        }

        private List<ViewModelTechnology> CombineAssetSiteInfoAndPorts_2G(List<ViewModelTechnology> siteInfo, List<ViewModelTechnology> ports)
        {
            if (siteInfo.Count != ports.Count)
            {

                var error = $"{Environment.NewLine}Possible error: More than one antenna for one technology.{Environment.NewLine}Asset EF query siteinfo count - {siteInfo.Count} is different than count - {ports.Count} for ports for  {siteInfo.FirstOrDefault().LAYER_TECHNOLOGY}.{Environment.NewLine}";

                foreach (var item in siteInfo)
                    error += $"CellName = {item.CellName}, Antenna = {item.AntennaType}, Etilt = {item.Etilt}, Feeder = {item.FEEDERTYPE}, Tech = {item.LAYER_TECHNOLOGY}.{Environment.NewLine}";


                throw new Exception(error);
            }


            foreach (var cell in siteInfo)
            {
                foreach (var port in ports)
                {
                    if (port.Band.Contains("900"))
                        port.Band = "900";
                    if (port.Band.Contains("1800"))
                        port.Band = "1800";


                    //GSM
                    if (cell.CellName == port.CellName && cell.CellID == port.CellID && cell.Band == port.Band && cell.LAYER_TECHNOLOGY == "G")
                    {
                        if (!string.IsNullOrEmpty(port.PortNumber))
                            cell.PortNumber = port.PortNumber;
                        else
                            cell.PortNumber = "0";

                        break;
                    }


                }

            }

            return siteInfo;

        }

        private List<ViewModelTechnology> CombineAssetSiteInfoAndPorts_3G_4G(List<ViewModelTechnology> siteInfo, List<ViewModelTechnology> ports)
        {
            if (siteInfo.Count != ports.Count)
            {

                var error = $"{Environment.NewLine}Possible error: More than one antenna for one technology.{Environment.NewLine}Asset EF query siteinfo count - {siteInfo.Count} is different than count - {ports.Count} for ports for  {siteInfo.FirstOrDefault().LAYER_TECHNOLOGY}.{Environment.NewLine}";

                foreach (var item in siteInfo)
                    error += $"CellName = {item.CellName}, Antenna = {item.AntennaType}, Etilt = {item.Etilt}, Feeder = {item.FEEDERTYPE}, Tech = {item.LAYER_TECHNOLOGY}.{Environment.NewLine}";


                throw new Exception(error);
            }


            foreach (var cell in siteInfo)
            {
                foreach (var port in ports)
                {
                    //UMTS and LTE
                    if (cell.CellName == port.CellName && cell.CellID == port.CellID)
                    {
                        if (!string.IsNullOrEmpty(port.PortNumber))
                            cell.PortNumber = port.PortNumber;
                        else
                            cell.PortNumber = "0";

                        break;
                    }

                }

            }

            return siteInfo;

        }

        private string LteBandMap(string lteBand)
        {
            if (lteBand == "3")
                return "1800";
            else if (lteBand == "8")
                return "900";
            else if (lteBand == "1")
                return "2100";
            else if (lteBand == "7")
                return "2600";

            return string.Empty;
        }

        public void GetSiteStructInstallType(out string siteStructureType, out string siteInstallType)//No Changes
        {
            using (var contextAsset = new Entities())
            {
                var siteStructInstallType = (
                from lognode in contextAsset.LOGNODEs
                join flag in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag = lognode.LOGNODEPK } equals new { flag.PROJECTNO, flag = flag.OBJECTKEY }
                join flag1 in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag1 = lognode.LOGNODEPK } equals new { flag1.PROJECTNO, flag1 = flag1.OBJECTKEY }
                join flag2 in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag2 = lognode.LOGNODEPK } equals new { flag2.PROJECTNO, flag2 = flag2.OBJECTKEY }
                where lognode.PROJECTNO == 1 && lognode.IDNAME.Contains(siteID)
               && flag.FLAGGROUP.FLAGGROUPID == "Candidate" && flag.FLAG.FLAGID == "Accepted"
               && flag1.FLAGGROUP.FLAGGROUPID == "Structure type"
               && flag2.FLAGGROUP.FLAGGROUPID == "Installation Type"
                select new { Structure_type = flag1.FLAG.FLAGID, Installation_Type = flag2.FLAG.FLAGID })
               .ToList();

                siteStructureType = siteStructInstallType.Max(n => n.Structure_type);
                siteInstallType = siteStructInstallType.Max(n => n.Installation_Type);
            }

        }

        public void GetSiteCoLocationOwnership(out string coLocation, out string siteOwner)//No Changes
        {
            using (var contextAsset = new Entities())
            {
                var siteCoLocationOwnership = (
                from lognode in contextAsset.LOGNODEs
                join flag in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag = lognode.LOGNODEPK } equals new { flag.PROJECTNO, flag = flag.OBJECTKEY }
                join flag1 in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag1 = lognode.LOGNODEPK } equals new { flag1.PROJECTNO, flag1 = flag1.OBJECTKEY }
                join flag2 in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag2 = lognode.LOGNODEPK } equals new { flag2.PROJECTNO, flag2 = flag2.OBJECTKEY }
                where lognode.PROJECTNO == 1 && lognode.IDNAME.Contains(siteID)
               && flag.FLAGGROUP.FLAGGROUPID == "Candidate" && flag.FLAG.FLAGID == "Accepted"
               && flag1.FLAGGROUP.FLAGGROUPID == "Site Owner"
               && flag2.FLAGGROUP.FLAGGROUPID == "Co-Location"
                select new { Owner = flag1.FLAG.FLAGID, CoLocation = flag2.FLAG.FLAGID })
               .ToList();

                coLocation = siteCoLocationOwnership.Max(n => n.Owner);
                siteOwner = siteCoLocationOwnership.Max(n => n.CoLocation);
            }

        }

        public decimal GetSiteLat()
        {
            using (var contextAsset = new Entities())
            {
                var siteLat = contextAsset.Database.SqlQuery<decimal>($@"select distinct d.COORDS.SDO_POINT.Y as LATITUDE from network_planning.lognode a 							
                    inner join network_planning.SITEADDRESS d  on a.projectno = d.projectno AND a.addressfk = d.addresskey
                    inner join  network_planning.FLAGVALUES z on a.PROJECTNO = z.PROJECTNO and a.LOGNODEPK = z.OBJECTKEY
                    INNER JOIN network_planning.FLAGS y ON y.PROJECTNO = z.PROJECTNO AND y.FLAGGROUPKEY = z.FLAGGROUPKEY AND y.FLAGKEY = z.FLAGKEY
                    INNER JOIN network_planning.FLAGGROUPS x ON x.PROJECTNO = y.PROJECTNO AND x.FLAGGROUPKEY = y.FLAGGROUPKEY

                    where a.idname like  '%{siteID}%'  and(x.flaggroupid = 'Candidate') AND(y.flagid = 'Accepted')").ToList().FirstOrDefault();
                return siteLat;
            }

        }

        public decimal GetSiteLong()
        {
            using (var contextAsset = new Entities())
            {
                var siteLong = contextAsset.Database.SqlQuery<decimal>($@"select distinct d.COORDS.SDO_POINT.X as LONGITUDE from network_planning.lognode a 										
                    inner join network_planning.SITEADDRESS d  on a.projectno = d.projectno AND a.addressfk = d.addresskey
                    inner join  network_planning.FLAGVALUES z on a.PROJECTNO = z.PROJECTNO and a.LOGNODEPK = z.OBJECTKEY
                    INNER JOIN network_planning.FLAGS y ON y.PROJECTNO = z.PROJECTNO AND y.FLAGGROUPKEY = z.FLAGGROUPKEY AND y.FLAGKEY = z.FLAGKEY
                    INNER JOIN network_planning.FLAGGROUPS x ON x.PROJECTNO = y.PROJECTNO AND x.FLAGGROUPKEY = y.FLAGGROUPKEY
                    where a.idname like  '%{siteID}%'  and(x.flaggroupid = 'Candidate') AND(y.flagid = 'Accepted')").ToList().FirstOrDefault();
                return siteLong;
            }

        }

        public int CheckForMoreThanOneCandidateGSM()
        {
            using (var contextAsset = new Entities())
            {
                var count = contextAsset.Database.SqlQuery<int>("select count(distinct cn.IDNAME) as count FROM  network_planning.GSMCELL b " +
                    "inner JOIN     network_planning.LOGCELL a ON b.PROJECTNO = a.PROJECTNO AND a.LOGCELLPK=b.LOGCELLFK " +
                    "inner JOIN     network_planning.munode c ON a.PROJECTNO = c.PROJECTNO AND a.LOGNODEFK = c.LOGNODEPK " +
                    "inner JOIN     network_planning.LOGNODE cn ON c.PROJECTNO = cn.PROJECTNO AND c.LOGNODEPK = cn.LOGNODEPK  " +
                    "inner JOIN    network_planning.FLAGVALUES z ON c.projectno = z.projectno AND c.LOGNODEPK = z.objectkey " +
                    "inner JOIN    network_planning.FLAGS y ON y.PROJECTNO = z.PROJECTNO AND y.FLAGGROUPKEY = z.FLAGGROUPKEY AND  y.FLAGKEY = z.FLAGKEY " +
                    "inner JOIN    network_planning.FLAGGROUPS x ON x.PROJECTNO = y.PROJECTNO AND x.FLAGGROUPKEY = y.FLAGGROUPKEY " +
                    "where cn.IDNAME like :siteID || '%' and     (a.PROJECTNO = 1) AND (b.GSMID > 0)    " +
                    "AND (x.flaggroupid = 'Candidate') AND (y.flagid = 'Accepted')", siteID).ToList().FirstOrDefault();
                return count;
            }
        }

        public int CheckForMoreThanOneCandidateUMTS()
        {
            using (var contextAsset = new Entities())
            {
                var count = contextAsset.Database.SqlQuery<int>("SELECT  count(distinct c1.IDNAME) as count FROM         network_planning.LOGUMTSCELL b1 " +
                    "INNER JOIN   network_planning.LOGCELL c2 ON b1.PROJECTNO = c2.PROJECTNO AND b1.LOGCELLFK = c2.LOGCELLPK " +
                    "INNER JOIN    network_planning.LOGNODE c1 ON c2.PROJECTNO = c1.PROJECTNO AND c2.LOGNODEFK = c1.LOGNODEPK   " +
                    "INNER JOIN  network_planning.FLAGVALUES z ON c1.projectno = z.projectno AND c1.lognodepk = z.objectkey " +
                    "INNER JOIN network_planning.FLAGS y ON y.PROJECTNO = z.PROJECTNO AND y.FLAGGROUPKEY = z.FLAGGROUPKEY AND    y.FLAGKEY = z.FLAGKEY " +
                    "INNER JOIN  network_planning.FLAGGROUPS x ON x.PROJECTNO = y.PROJECTNO AND x.FLAGGROUPKEY = y.FLAGGROUPKEY  " +
                    "WHERE     (b1.PROJECTNO = 1)   AND (x.flaggroupid = 'Candidate') AND (y.flagid = 'Accepted')    " +
                    "and c1.idname like :siteID || '%'  ", siteID).ToList().FirstOrDefault();
                return count;
            }
        }

        public int CheckForMoreThanOneCandidateLTE()
        {
            using (var contextAsset = new Entities())
            {
                var count = contextAsset.Database.SqlQuery<int>("select count(distinct b5.IDNAME) as count  from LOGLTECELL a " +
                    "INNER JOIN LogLteCellCar a1 ON a1.PROJECTNO =a.PROJECTNO AND a1.LOGCELLFK = a.LOGCELLFK    " +
                    "INNER JOIN logcell b4 ON b4.PROJECTNO =a.PROJECTNO AND b4.LOGCELLPK = a.LOGCELLfK   " +
                    "INNER JOIN  LOGNODE b5 ON b4.PROJECTNO = b5.PROJECTNO AND b4.LOGNODEFK = b5.LOGNODEPK  " +
                    "INNER JOIN FLAGVALUES z ON a.projectno = z.projectno AND b5.lognodepk  = z.objectkey " +
                    "INNER JOIN FLAGS y ON y.PROJECTNO = z.PROJECTNO AND y.FLAGGROUPKEY = z.FLAGGROUPKEY AND  y.FLAGKEY = z.FLAGKEY " +
                    "INNER JOIN FLAGGROUPS x ON y.PROJECTNO = x.PROJECTNO AND y.FLAGGROUPKEY = x.FLAGGROUPKEY " +
                    "WHERE     (a.PROJECTNO = 1)   AND (x.flaggroupid = 'Candidate') AND (y.flagid = 'Accepted')  " +
                    "and b5.idname like :siteID || '%'", siteID).ToList().FirstOrDefault();
                return count;
            }
        }

        public List<ModelRRU> Get2GSiteCM()
        {

            var output2G_CM = (
               from power in hwi_db.GBTS_power
               join trxInfo in hwi_db.GTRX_Info on new { power.timestamp, power.Cell_Name, power.TRX_ID } equals
                    new { trxInfo.timestamp, trxInfo.Cell_Name, trxInfo.TRX_ID }
               where power.timestamp == hwi_db.GBTS_power.Select(n => n.timestamp).Max() && power.BTS_Name.Contains(siteID)
               && trxInfo.Active_Status == "ACTIVATED"
               select new ModelRRU
               {
                   Technology = "G",
                   Band = trxInfo.Frequency,
                   SiteID = siteID,
                   Sector = power.Cell_Name.Substring(4, 1),
                   CellName = power.Cell_Name,
                   GSM_TRX = 1,
                   GSM_Pwr_per_TRX = power.eGBTS_Power_Type_01dBm,
                   UMTS_TRX = 0,
                   UMTS_Pwr_per_TRX = "0",
                   LTE_TRX = 0,
                   LTE_Pwr_per_TRX = "0",
                   NR_Pwr_per_TRX = "0",
                   NR_TRX = 0

               }).Distinct().ToList();

            foreach (var item in output2G_CM)
            {
                if (!String.IsNullOrEmpty(item.GSM_Pwr_per_TRX))
                    item.GSM_Pwr_per_TRX = (int.Parse(item.GSM_Pwr_per_TRX) / 10).ToString();
                else
                    item.GSM_Pwr_per_TRX = "";
            }

            return output2G_CM;
        }

        public List<ModelRRU> Get3GSiteCM()
        {
            var output3G_CM = (
               from power in hwi_db.UCELLSETUP_SRAN
               where power.TIMESTAMP == hwi_db.UCELLSETUP_SRAN.Select(n => n.TIMESTAMP).Max()
               && power.NodeBName.Contains(siteID) && power.Validationindication == "ACTIVATED"
               select new ModelRRU
               {
                   Technology = "U",
                   Band = power.BandIndicator == "Band8" ? "900" : "2100",
                   SiteID = siteID,
                   Sector = power.CellName.Substring(4, 1),
                   CellName = power.CellName,
                   GSM_TRX = 0,
                   GSM_Pwr_per_TRX = "0",
                   UMTS_TRX = 1,
                   UMTS_Pwr_per_TRX = power.MaxTransmitPowerofCell,
                   LTE_TRX = 0,
                   LTE_Pwr_per_TRX = "0",
                   NR_Pwr_per_TRX = "0",
                   NR_TRX = 0
               }).ToList();

            foreach (var item in output3G_CM)
            {
                if (!String.IsNullOrEmpty(item.UMTS_Pwr_per_TRX))
                    item.UMTS_Pwr_per_TRX = (int.Parse(item.UMTS_Pwr_per_TRX) / 10).ToString();
                else
                    item.UMTS_Pwr_per_TRX = "";
            }

            return output3G_CM;
        }

        public List<ModelRRU> Get4GSiteCM()
        {

            var output4G_CM_anonym = (
            from power in hwi_db.LCELLs
            where power.timestamp == hwi_db.LCELLs.Select(n => n.timestamp).Max() && power.NE_Name.Contains(siteID)
            && power.Cell_active_state == "CELL_ACTIVE"
            select new
            {
                Technology = "L",
                Band = power.Frequency_band,
                SiteID = siteID,
                Sector = power.Cell_Name.Substring(4, 1),
                CellName = power.Cell_Name,
                power.Reference_signal_power_01dBm,
                power.PB,
                power.Downlink_bandwidth,
                TRX = "1"
            }).ToList();

            List<ModelRRU> output4G_CM = new List<ModelRRU>();


            foreach (var item in output4G_CM_anonym)
            {
                double Pb = 1;

                if (item.PB != "0")
                    Pb = double.Parse(item.PB);

                var bandStr = item.Downlink_bandwidth.Replace("CELL_BW_N", "");

                int.TryParse(bandStr, out int band);


                var model = new ModelRRU();
                model.Technology = item.Technology;
                model.Band = LteBandMap(item.Band);
                model.SiteID = item.SiteID;
                model.Sector = item.Sector;
                model.CellName = item.CellName;
                model.GSM_TRX = 0;
                model.GSM_Pwr_per_TRX = "0";
                model.UMTS_TRX = 0;
                model.UMTS_Pwr_per_TRX = "0";
                model.LTE_TRX = int.Parse(item.TRX);
                model.LTE_Pwr_per_TRX = Math.Round(
                     (Math.Log10(
                       Math.Pow(
                           10.0,
                           double.Parse(item.Reference_signal_power_01dBm) /
                           100.0) *
                       Pb *
                       0.5 *
                       band *
                       12
                       ) * 10), 2)
                       .ToString();
                model.NR_Pwr_per_TRX = "0";
                model.NR_TRX = 0;

                output4G_CM.Add(model);
            }

            return output4G_CM;

        }

        public List<ModelRRU> Get5GSiteCM()
        {
            var output5G_CM = (
            from power in hwi_db.MNSD_5G_Dump
            where power.timestamp == hwi_db.MNSD_5G_Dump.Select(n => n.timestamp).Max()
                  && power.NE_Name.Contains(siteID)
                  && power.Cell_Activate_State == "CELL_ACTIVE"
            select new ModelRRU
            {
                Technology = "NR",
                Band =
                    (power.Frequency_Band == "N1") ? "2100" :
                    (power.Frequency_Band == "N3") ? "1800" :
                    (power.Frequency_Band == "N78") ? "3500" :
                    (power.Frequency_Band == "N28") ? "700" :
                    (power.Frequency_Band == "N20") ? "800" : null,
                SiteID = siteID,
                Sector = power.NR_DU_Cell_Name.Substring(4, 1),
                CellName = power.NR_DU_Cell_Name,
                GSM_TRX = 0,
                GSM_Pwr_per_TRX = "0",
                UMTS_TRX = 0,
                UMTS_Pwr_per_TRX = "0",
                LTE_TRX = 0,
                LTE_Pwr_per_TRX = "0",
                NR_Pwr_per_TRX = power.Max_Transmit_Power_0_1dBm,
                NR_TRX = 1


            }).Distinct().ToList();

            foreach (var item in output5G_CM)
            {
                if (!String.IsNullOrEmpty(item.NR_Pwr_per_TRX))
                    item.NR_Pwr_per_TRX = (int.Parse(item.NR_Pwr_per_TRX) / 10).ToString();
                else
                    item.NR_Pwr_per_TRX = "";

                //DSS
                if (item.Band == "1800" || item.Band == "2100")
                    item.NR_TRX = 0;
            }

            return output5G_CM;
        }

        public IEnumerable<ModelRRU> GetCMallTechRRU_ver2()
        {
            var result = (
               from rru in hwi_db.RRUsPerNodes_5G_included
               where rru.TIMESTAMP == hwi_db.RRUsPerNodes_5G_included.Select(n => n.TIMESTAMP).Max()
                     && rru.SITE.Contains(siteID)
               select new ModelRRU
               {
                   CellName = rru.CellName.ToString(),
                   Technology = rru.TECH,
                   Band = rru.BAND.ToString(),
                   RRU_Type = rru.RRU_type,
                   RRU_SN = rru.RRU_Serial_Number
               }).ToList();

            foreach (var item in result)
            {
                if (item.Technology == "GSM")
                    item.Technology = "G";
                else if (item.Technology == "UMTS")
                    item.Technology = "U";
                else if (item.Technology == "LTE")
                    item.Technology = "L";
                else if (item.Technology == "NR")
                    item.Technology = "NR";
            }

            return result;
        }

        public static Dictionary<int, string[]> GetAntennaPorts(string antenna)
        {
            using (var contextAsset = new Entities())
            {

                var antennaPorts = (

                            from antDevice in contextAsset.ANTENNADEVICEs
                            from port in antDevice.ANTENNAPORTs

                            where antDevice.IDNAME == antenna

                            select new
                            {
                                port.PORTINDEX,
                                port.PORTLOCATION,
                                port.PORTTYPE
                            }
                            ).ToDictionary(n => (int)n.PORTINDEX, n => new string[] { n.PORTLOCATION, "Free", n.PORTTYPE });

                if (antennaPorts.Count == 0)
                    throw new Exception($"Антената {antenna} не може да бъде намерена в Asset - Eqipment Cellular Antennas!");

                return antennaPorts;
            }
        }

        public static List<string> GetAllAntennas()
        {
            using (var contextAsset = new Entities())
            {
                var antennas = contextAsset.ANTENNADEVICEs.Select(n => n.IDNAME).OrderBy(n => n).Distinct().ToList();

                return antennas;
            }
        }

        public static List<DTO.Port> GetAllAntennasPortsBands()
        {
            using (var contextAsset = new Entities())
            {
                var antennaPorts = (

                                   from antDevice in contextAsset.ANTENNADEVICEs
                                   from port in antDevice.ANTENNAPORTs

                                   select new DTO.Port
                                   {
                                       AntennaType = antDevice.IDNAME,
                                       BandRange = port.PORTLOCATION,
                                       BandPosition = (int)port.PORTINDEX,
                                       PortName = port.PORTTYPE
                                   }
                                   ).OrderBy(n => n.BandPosition).ToList();

                var group = antennaPorts.ToList()
                    .GroupBy(n => new
                    {
                        n.AntennaType
                    })
                    .Select(n => new DTO.Port
                    {
                        AntennaType = n.Key.AntennaType,
                        BandRange = String.Join(" ", n.Select(k => k.BandRange)),
                        PortName = String.Join(" ", n.Select(k => k.PortName))
                    }).ToList();

                return group;
            }
        }

        public List<ViewModelTechnology> GetAllTechAsset()
        {
            
            try
            {
                var siteInfo2G = Task<List<ViewModelTechnology>>.Factory.StartNew(() => Get2GSiteAsset());
                var siteInfo3G = Task<List<ViewModelTechnology>>.Factory.StartNew(() => Get3GSiteAsset());
                var siteInfo4G = Task<List<ViewModelTechnology>>.Factory.StartNew(() => Get4GSiteAsset());
                var etilt2G = Task<List<ViewModelTechnology>>.Factory.StartNew(() => EtiltAsset2G());
                var etilt3G = Task<List<ViewModelTechnology>>.Factory.StartNew(() => EtiltAsset3G());
                var etilt4G = Task<List<ViewModelTechnology>>.Factory.StartNew(() => EtiltAsset4G());
                var ports2G = Task<List<ViewModelTechnology>>.Factory.StartNew(() => GetPorts2G());
                var ports3G = Task<List<ViewModelTechnology>>.Factory.StartNew(() => GetPorts3G());
                var ports4G = Task<List<ViewModelTechnology>>.Factory.StartNew(() => GetPorts4G());


                Task.WaitAll(new Task[] { siteInfo2G, siteInfo3G, siteInfo4G, etilt2G, etilt3G, etilt4G, ports2G, ports3G, ports4G });


                var gsm = CombineAssetSiteInfoAndEtilt(siteInfo2G.Result, etilt2G.Result);
                var umts = CombineAssetSiteInfoAndEtilt(siteInfo3G.Result, etilt3G.Result);
                var lte = CombineAssetSiteInfoAndEtilt(siteInfo4G.Result, etilt4G.Result);

                gsm = CombineAssetSiteInfoAndPorts_2G(gsm, ports2G.Result);
                umts = CombineAssetSiteInfoAndPorts_3G_4G(umts, ports3G.Result);
                lte = CombineAssetSiteInfoAndPorts_3G_4G(lte, ports4G.Result);

                var nr = Get5GSiteAsset_Python();
                var union = gsm.Union(umts).Union(lte).Union(nr).ToList();

                //var union = gsm.Union(umts).Union(lte).ToList();


                return union;

            }
            catch (Exception ex)
            {
                throw new Exception($"Method GetAllTechAsset. Error trying to materialise queries from AssetDB. {ex.Message}");
            }

        }

        public string[] GetDomainInfo()
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString().Replace("AD\\", "");
            string[] result = { "", "", "" };
            PrincipalContext context = new PrincipalContext(ContextType.Domain);
            try
            {
                using (UserPrincipal user = UserPrincipal.FindByIdentity(context, userName))
                {
                    DirectoryEntry directoryEntry = user.GetUnderlyingObject() as DirectoryEntry;
                    object directoryPropertyValueCN = directoryEntry.Properties["cn"].Value;
                    object directoryPropertyValueMail = directoryEntry.Properties["mail"].Value;
                    object directoryPropertyValueMobile = directoryEntry.Properties["mobile"].Value;
                    result[0] = directoryPropertyValueCN.ToString();
                    result[1] = directoryPropertyValueMail.ToString();
                    result[2] = directoryPropertyValueMobile.ToString();

                    return result;
                }
            }
            catch (Exception)
            {
                return result;
            }
        }

        public static string GetAntennasWithoutTech(string siteID)
        {
            using (var contextAsset = new Entities())
            {
                var queryObj = (
                from siteaddress in contextAsset.SITEADDRESSes
                from lognode in siteaddress.LOGNODEs

                from phyAntenna in siteaddress.PHYANTENNAs

                join anntenaDevice in contextAsset.ANTENNADEVICEs on new { phyAntenna.PROJECTNO, key = (decimal)phyAntenna.DEVICEFK } equals new { anntenaDevice.PROJECTNO, key = anntenaDevice.DEVICEPK }

                join pattern1 in contextAsset.ANTENNAPATTERNs on new { phyAntenna.PROJECTNO, key = (decimal)phyAntenna.MASTERPATTERN1FK } equals new { pattern1.PROJECTNO, key = pattern1.PATTERNPK }

                join pattern2 in contextAsset.ANTENNAPATTERNs on new { phyAntenna.PROJECTNO, key = (decimal)phyAntenna.MASTERPATTERN2FK } equals new { pattern2.PROJECTNO, key = pattern2.PATTERNPK }

                join pattern3 in contextAsset.ANTENNAPATTERNs on new { phyAntenna.PROJECTNO, key = (decimal)phyAntenna.MASTERPATTERN3FK } equals new { pattern3.PROJECTNO, key = pattern3.PATTERNPK }

                join pattern4 in contextAsset.ANTENNAPATTERNs on new { phyAntenna.PROJECTNO, key = (decimal)phyAntenna.MASTERPATTERN4FK } equals new { pattern4.PROJECTNO, key = pattern4.PATTERNPK }

                join flag in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag = lognode.LOGNODEPK } equals new { flag.PROJECTNO, flag = flag.OBJECTKEY }

                where siteaddress.PROJECTNO == 1 && siteaddress.IDNAME.Contains(siteID)
                && flag.FLAGGROUP.FLAGGROUPID == "Candidate" && flag.FLAG.FLAGID == "Accepted"


                select new
                {
                    SiteID = siteaddress.IDNAME,
                    AntennaType = anntenaDevice.IDNAME,
                    Azimuth = phyAntenna.AZIMUTH,
                    Pattern1 = pattern1.IDNAME,
                    Pattern2 = pattern2.IDNAME,
                    Pattern3 = pattern3.IDNAME,
                    Pattern4 = pattern4.IDNAME,
                }

                ).Distinct().ToList();

                string output = string.Empty;

                foreach (var item in queryObj)
                {
                    if (item.Pattern1 == "Unknown" && item.Pattern2 == "Unknown" && item.Pattern3 == "Unknown" && item.Pattern4 == "Unknown")
                        output += $"Azimuth {item.Azimuth} Antenna {item.AntennaType} doesn't have any technology!{Environment.NewLine}";
                }

                return output;

            }

        }


    }
}
