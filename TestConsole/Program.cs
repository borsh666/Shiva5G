using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using BLL.DTO;
using DAL_Asset10;

namespace TestConsole
{
    class Program
    {

        private static string Test5gQuery()
        {
            return "select  SUBSTR(sa.IDNAME, 1, 20) AS SiteID, " +
                "ln.NAME AS SiteName FROM SITEADDRESS sa " +
                "JOIN LOGNODE ln ON sa.ADDRESSKEY = ln.ADDRESSFK AND ln.PROJECTNO = 1 " +
                "where SUBSTR(sa.IDNAME, 1, 20) like '%SF1041%' ";
        }


        private static string siteID = "VT5450";

        //       private static Dictionary<int, string[]> GetAntennaPorts(string antenna)
        //       {
        //           using (var contextAsset = new Entities())
        //           {

        //               var antennaPorts = (

        //                           from antDevice in contextAsset.ANTENNADEVICE
        //                           from port in antDevice.ANTENNAPORT

        //                           where antDevice.IDNAME == antenna

        //                           select new
        //                           {
        //                               port.PORTINDEX,
        //                               port.PORTLOCATION
        //                           }
        //                           ).ToDictionary(n => (int)n.PORTINDEX, n => new string[] { n.PORTLOCATION, "Free" });

        //               return antennaPorts;
        //           }
        //       }

        //       private static List<ViewModelTechnology> EtiltAsset2G() //9.1 OK 
        //       {
        //           using (var contextAsset = new Entities())
        //           {
        //               var eTilts = contextAsset.Database.SqlQuery<ViewModelTechnology>($@"select distinct  a.IDNAME as CellName,b.GSMID as CellID,

        //                               case when la.INHERITMASTERPATTERN = 0 then ap.downtilt
        //                                    when la.INHERITMASTERPATTERN = 1 then ap1.downtilt
        //                                    when la.INHERITMASTERPATTERN = 2 then ap2.downtilt
        //                                    when la.INHERITMASTERPATTERN = 3 then ap3.downtilt
        //                                    when la.INHERITMASTERPATTERN = 4 then ap4.downtilt
        //                                    end AS Etilt


        //                               FROM network_planning.GSMCELL b
        //                               INNER JOIN network_planning.LOGCELL a ON b.PROJECTNO = a.PROJECTNO AND a.LOGCELLPK = b.LOGCELLPK
        //                               INNER JOIN network_planning.munode c ON a.PROJECTNO = c.PROJECTNO AND a.LOGNODEFK = c.LOGNODEPK
        //                               INNER JOIN network_planning.LOGNODE cn ON c.PROJECTNO = cn.PROJECTNO AND c.LOGNODEPK = cn.LOGNODEPK
        //                               INNER JOIN network_planning.SITEADDRESS d ON cn.projectno = d.projectno AND cn.addressfk = d.addresskey
        //                               INNER JOIN network_planning.CELLAYDATA g ON b.projectno = g.projectno AND b.LOGCELLPK = g.cellkey
        //                               INNER JOIN network_planning.CLDANTENNA k ON g.projectno = k.projectno AND g.cellaydatapk = k.cellaydatafk

        //                               inner join logicalantenna la on la.PROJECTno = k.PROJECTno and la.LOGANTENNAPK = k.LOGANTENNAFK-- ? !? !??
        //                               INNER JOIN phyantenna pa ON pa.projectno = la.projectno and pa.PHYANTENNApK = la.PHYANTENNAfk
        //                               inner join antennadevice ad on ad.PROJECTNO = pa.PROJECTNO and ad.DEVICEPK = pa.DEVICEFK


        //                               INNER JOIN  ANTENNAPATTERN ap ON la.PROJECTno = ap.PROJECTno and la.anttypefk = ap.patternpk
        //                               inner join ANTENNAPATTERN ap1 on ap1.PROJECTNO = pa.PROJECTNO and ap1.PATTERNPK = pa.MASTERPATTERN1FK
        //                               inner join ANTENNAPATTERN ap2 on ap2.PROJECTNO = pa.PROJECTNO and ap2.PATTERNPK = pa.MASTERPATTERN2FK
        //                               inner join ANTENNAPATTERN ap3 on ap3.PROJECTNO = pa.PROJECTNO and ap3.PATTERNPK = pa.MASTERPATTERN3FK
        //                               inner join ANTENNAPATTERN ap4 on ap4.PROJECTNO = pa.PROJECTNO and ap4.PATTERNPK = pa.MASTERPATTERN4FK

        //                               INNER JOIN network_planning.FLAGVALUES z ON c.projectno = z.projectno AND c.LOGNODEPK = z.objectkey
        //                               INNER JOIN network_planning.FLAGS y ON y.PROJECTNO = z.PROJECTNO AND y.FLAGGROUPKEY = z.FLAGGROUPKEY AND y.FLAGKEY = z.FLAGKEY
        //                               INNER JOIN network_planning.FLAGGROUPS x ON x.PROJECTNO = y.PROJECTNO AND x.FLAGGROUPKEY = y.FLAGGROUPKEY


        //                               where(x.flaggroupid = 'Candidate')
        //                               AND(y.flagid = 'Accepted')
        //                               AND(a.PROJECTNO = 1) AND(b.GSMID > 0)
        //                               and d.idname like '%{siteID}%'").ToList();
        //               return eTilts;
        //           }

        //       }

        //       private static List<ViewModelTechnology> EtiltAsset3G() //9.1 OK 
        //       {
        //           using (var contextAsset = new Entities())
        //           {
        //               var eTilts = contextAsset.Database.SqlQuery<ViewModelTechnology>($@"select distinct 

        //                   b1.umtscellid as CellID, c2.IDNAME AS CellName, 

        //                   case when la.INHERITMASTERPATTERN=0 then ap.downtilt
        //                        when la.INHERITMASTERPATTERN=1 then ap1.downtilt 
        //                        when la.INHERITMASTERPATTERN=2 then ap2.downtilt
        //                        when la.INHERITMASTERPATTERN=3 then ap3.downtilt
        //                        when la.INHERITMASTERPATTERN=4 then ap4.downtilt
        //                        end AS Etilt

        //                   from  network_planning.LOGUMTSCELL b1 
        //                   INNER JOIN network_planning.LOGCELL c2 ON b1.PROJECTNO = c2.PROJECTNO AND b1.LOGCELLPK = c2.LOGCELLPK 
        //                   INNER JOIN network_planning.LOGNODE c1 ON c2.PROJECTNO = c1.PROJECTNO AND c2.LOGNODEFK = c1.LOGNODEPK 
        //                   INNER JOIN network_planning.LOGUMTSCELLCAR ucc ON b1.PROJECTNO = ucc.PROJECTNO AND b1.LOGCELLPK = ucc.UMTSCELLFK 
        //                   INNER JOIN network_planning.LOGUMTSCAR uc ON ucc.projectno = uc.projectno AND ucc.CARRIERFK = uc.UMTSCARPK 
        //                   INNER JOIN network_planning.TGCARRIER ut ON uc.projectno = ut.projectno AND uc.tgcarrierfk = ut.carrierkey 
        //                   INNER JOIN network_planning.SITEADDRESS h ON c1.projectno = h.projectno AND c1.addressfk = h.addresskey 
        //                   INNER JOIN network_planning.LOGCONNECTION d1 ON c1.PROJECTNO = d1.PROJECTNO AND c1.LOGNODEPK = d1.LOGNODEBFK 
        //                   INNER JOIN network_planning.LOGRNC e1 ON d1.PROJECTNO = e1.PROJECTNO AND d1.lognodeafk = e1.lognodepk 
        //                   INNER JOIN network_planning.LOGUMTSFEEDER f ON b1.PROJECTNO = f.projectno AND b1.LOGCELLPK = f.umtscellfk 

        //                   inner join logicalantenna la on la.PROJECTno=f.PROJECTno and la.LOGANTENNAPK =f.LOGANTENNAFK
        //                   inner join phyantenna pa on pa.PROJECTNO = la.PROJECTNO and pa.PHYANTENNAPK = la.PHYANTENNAFK
        //                   INNER JOIN  ANTENNAPATTERN ap  on ap.PROJECTNO = la.PROJECTNO and la.anttypefk = ap.patternpk 
        //                   inner join ANTENNAPATTERN ap1 on ap1.PROJECTNO = pa.PROJECTNO and ap1.PATTERNPK = pa.MASTERPATTERN1FK
        //                   inner join ANTENNAPATTERN ap2 on ap2.PROJECTNO = pa.PROJECTNO and ap2.PATTERNPK = pa.MASTERPATTERN2FK
        //                   inner join ANTENNAPATTERN ap3 on ap3.PROJECTNO = pa.PROJECTNO and ap3.PATTERNPK = pa.MASTERPATTERN3FK
        //                   inner join ANTENNAPATTERN ap4 on ap4.PROJECTNO = pa.PROJECTNO and ap4.PATTERNPK = pa.MASTERPATTERN4FK

        //                   LEFT OUTER JOIN network_planning.FEEDER j ON f.feedertypefk = j.feederkey 
        //                   INNER JOIN network_planning.FLAGVALUES z ON c1.projectno = z.projectno AND c1.lognodepk = z.objectkey 
        //                   INNER JOIN network_planning.FLAGS y ON y.PROJECTNO = z.PROJECTNO AND y.FLAGGROUPKEY = z.FLAGGROUPKEY AND y.FLAGKEY = z.FLAGKEY 
        //                   INNER JOIN network_planning.FLAGGROUPS x ON x.PROJECTNO = y.PROJECTNO AND x.FLAGGROUPKEY = y.FLAGGROUPKEY

        //                   INNER JOIN network_planning.FLAGVALUES z1 ON b1.projectno = z1.projectno AND b1.LOGcellpK = z1.objectkey 
        //                   INNER JOIN network_planning.FLAGS y1 ON y1.PROJECTNO = z1.PROJECTNO AND y1.FLAGGROUPKEY = z1.FLAGGROUPKEY AND y1.FLAGKEY = z1.FLAGKEY 
        //                   INNER JOIN network_planning.FLAGGROUPS x1 ON x1.PROJECTNO = y1.PROJECTNO AND x1.FLAGGROUPKEY = y1.FLAGGROUPKEY

        //                   where (x.flaggroupid = 'Candidate')  
        //                   AND (y.flagid ='Accepted') 
        //                   AND (b1.PROJECTNO = 1) 
        //                   and c1.idname like '%{siteID}%'").ToList();
        //               return eTilts;
        //           }

        //       }

        //       private static List<ViewModelTechnology> EtiltAsset4G() //9.1 OK 
        //       {
        //           using (var contextAsset = new Entities())
        //           {
        //               var eTilts = contextAsset.Database.SqlQuery<ViewModelTechnology>($@"select distinct
        //                                    a.cellid as CellID, b4.idname as CellName,

        //                                   case when la.INHERITMASTERPATTERN=0 then ap.downtilt
        //                                        when la.INHERITMASTERPATTERN=1 then ap1.downtilt 
        //                                        when la.INHERITMASTERPATTERN=2 then ap2.downtilt
        //                                        when la.INHERITMASTERPATTERN=3 then ap3.downtilt
        //                                        when la.INHERITMASTERPATTERN=4 then ap4.downtilt
        //                                        end AS Etilt



        //                                   from LOGLTECELL a       
        //                                   INNER JOIN LogLteCellCar a1 ON a1.PROJECTNO =a.PROJECTNO AND a1.ltecellfk = a.LOGCELLPK    
        //                                   INNER JOIN logcell b4 ON b4.PROJECTNO =a.PROJECTNO AND b4.LOGCELLPK = a.LOGCELLPK   
        //                                   INNER JOIN LOGNODE b5 ON b4.PROJECTNO = b5.PROJECTNO AND b4.LOGNODEFK = b5.LOGNODEPK
        //                                   inner join network_planning.SITEADDRESS h ON b5.projectno = h.projectno AND b5.addressfk = h.addresskey
        //                                   INNER JOIN logltecar b6 ON b6.PROJECTNO = b5.PROJECTNO AND b6.LOGNODEFK = b5.LOGNODEpK
        //                                   INNER JOIN LTECARRIER b7 ON b6.PROJECTNO = b7.PROJECTNO and b7.CARRIERKEY=b6.lteCARRIERfK 
        //                                   INNER JOIN network_planning.LOGLTEFEEDER f ON a.PROJECTNO = f.projectno AND a.LOGCELLPK = f.ltecellfk 
        //                                   INNER JOIN network_planning.FEEDER j ON f.feedertypefk = j.feederkey 

        //                                   inner join logicalantenna la on la.PROJECTno=f.PROJECTno and la.LOGANTENNAPK =f.LOGANTENNAFK
        //                                   inner join phyantenna pa on pa.PROJECTNO = la.PROJECTNO and pa.PHYANTENNAPK = la.PHYANTENNAFK
        //                                   INNER JOIN  ANTENNAPATTERN ap  on ap.PROJECTNO = la.PROJECTNO and la.anttypefk = ap.patternpk 
        //                                   inner join ANTENNAPATTERN ap1 on ap1.PROJECTNO = pa.PROJECTNO and ap1.PATTERNPK = pa.MASTERPATTERN1FK
        //                                   inner join ANTENNAPATTERN ap2 on ap2.PROJECTNO = pa.PROJECTNO and ap2.PATTERNPK = pa.MASTERPATTERN2FK
        //                                   inner join ANTENNAPATTERN ap3 on ap3.PROJECTNO = pa.PROJECTNO and ap3.PATTERNPK = pa.MASTERPATTERN3FK
        //                                   inner join ANTENNAPATTERN ap4 on ap4.PROJECTNO = pa.PROJECTNO and ap4.PATTERNPK = pa.MASTERPATTERN4FK


        //                                   inner join network_planning.FLAGVALUES z ON b5.projectno = z.projectno AND b5.lognodepk = z.objectkey 
        //                                   INNER JOIN network_planning.FLAGS y ON y.PROJECTNO = z.PROJECTNO AND y.FLAGGROUPKEY = z.FLAGGROUPKEY AND y.FLAGKEY = z.FLAGKEY 
        //                                   INNER JOIN network_planning.FLAGGROUPS x ON x.PROJECTNO = y.PROJECTNO AND x.FLAGGROUPKEY = y.FLAGGROUPKEY

        //                                   inner join network_planning.FLAGVALUES z1 ON a.projectno = z1.projectno AND a.LOGcellPK = z1.objectkey 
        //                                   INNER JOIN network_planning.FLAGS y1 ON y1.PROJECTNO = z1.PROJECTNO AND y1.FLAGGROUPKEY = z1.FLAGGROUPKEY AND y1.FLAGKEY = z1.FLAGKEY 
        //                                   INNER JOIN network_planning.FLAGGROUPS x1 ON x1.PROJECTNO = y1.PROJECTNO AND x1.FLAGGROUPKEY = y1.FLAGGROUPKEY


        //                                   where (x.flaggroupid = 'Candidate')  
        //                                   AND (y.flagid ='Accepted') 
        //                                   AND (a.PROJECTNO = 1) 
        //                                   and b5.idname like '%{siteID}%'").ToList();
        //               return eTilts;
        //           }

        //       }

        //       private static IEnumerable<ViewModelTechnology> Get2GSiteAsset() //9.1 OK 
        //       {
        //           using (var contextAsset = new Entities())
        //           {
        //               var outputGSM = (
        //              from siteaddress in contextAsset.SITEADDRESS
        //              from lognode in siteaddress.LOGNODE
        //              from logcell in lognode.LOGCELL
        //              from cell in logcell.GSMCELL
        //              from cellayData in cell.CELLAYDATA
        //              from cldantenna in cellayData.CLDANTENNA


        //              join feeder in contextAsset.FEEDER on cldantenna.FEEDERKEY equals feeder.FEEDERKEY
        //              join antennaDevice in contextAsset.ANTENNADEVICE on cldantenna.LOGICALANTENNA.PHYANTENNA.DEVICEFK equals antennaDevice.DEVICEPK

        //              //BSC part 
        //              join munode in contextAsset.MUNODE
        //                  on new { prj = logcell.PROJECTNO, lognode = logcell.LOGNODEFK }
        //                  equals new { prj = munode.PROJECTNO, lognode = munode.LOGNODEPK }

        //              join lognode1 in contextAsset.LOGNODE
        //              on new { prj = munode.PROJECTNO, lognode = munode.LOGNODE.LOGNODEPK }
        //              equals new { prj = lognode1.PROJECTNO, lognode = lognode1.LOGNODEPK }

        //              join logconnection in contextAsset.LOGCONNECTION
        //              on new { prj = lognode1.PROJECTNO, logconnection = lognode1.LOGNODEPK }
        //              equals new { prj = logconnection.PROJECTNO, logconnection = logconnection.LOGNODEBFK }

        //              join bsc in contextAsset.BSC
        //              on new { prj = logconnection.PROJECTNO, lognode = logconnection.LOGNODEAFK }
        //              equals new { prj = bsc.PROJECTNO, lognode = bsc.LOGNODEPK }
        //              //BSC part



        //              join flag in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag = lognode.LOGNODEPK } equals new { flag.PROJECTNO, flag = flag.OBJECTKEY }
        //              join flag1 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag1 = cell.LOGCELLPK } equals new { flag1.PROJECTNO, flag1 = flag1.OBJECTKEY }
        //              join flag2 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag2 = cell.LOGCELLPK } equals new { flag2.PROJECTNO, flag2 = flag2.OBJECTKEY }
        //              join flag3 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag3 = cell.LOGCELLPK } equals new { flag3.PROJECTNO, flag3 = flag3.OBJECTKEY }
        //              join flag4 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag4 = cell.LOGCELLPK } equals new { flag4.PROJECTNO, flag4 = flag4.OBJECTKEY }
        //              join flag5 in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag5 = lognode.LOGNODEPK } equals new { flag5.PROJECTNO, flag5 = flag5.OBJECTKEY }
        //              join flag6 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag6 = cell.LOGCELLPK } equals new { flag6.PROJECTNO, flag6 = flag6.OBJECTKEY }

        //              where siteaddress.PROJECTNO == 1 && siteaddress.IDNAME.Contains(siteID)
        //              && flag.FLAGGROUPS.FLAGGROUPID == "Candidate" && flag.FLAGS.FLAGID == "Accepted"
        //              && flag1.FLAGGROUPS.FLAGGROUPID == "TMA"
        //              && flag2.FLAGGROUPS.FLAGGROUPID == "RRU type"
        //              && flag3.FLAGGROUPS.FLAGGROUPID == "Combiner/Splitter"
        //              && flag4.FLAGGROUPS.FLAGGROUPID == "2nd Combiner"
        //              && flag5.FLAGGROUPS.FLAGGROUPID == "Co-Location"
        //              && flag6.FLAGGROUPS.FLAGGROUPID == "Antenna Mounting"


        //              select new ViewModelTechnology
        //              {
        //                  Controler = bsc.LOGNODE.IDNAME,
        //                  SiteID = siteaddress.IDNAME,
        //                  Candidate = siteaddress.IDNAME.Substring(10, 1),
        //                  SiteName = lognode.NAME,
        //                  SiteAddress = siteaddress.TOWN,
        //                  SiteAddress1 = siteaddress.ADDRESS1,
        //                  SiteAddress2 = siteaddress.ADDRESS2,
        //                  AntennaType = antennaDevice.IDNAME,
        //                  PHYINDEX = cldantenna.LOGICALANTENNA.PHYANTENNA.PHYINDEX,
        //                  Azimuth = cldantenna.LOGICALANTENNA.PHYANTENNA.AZIMUTH,
        //                  AGL = cldantenna.LOGICALANTENNA.PHYANTENNA.HEIGHT,
        //                  ARTL = cldantenna.LOGICALANTENNA.PHYANTENNA.HEIGHTOFFSET,
        //                  MECHANICAL_TILT = cldantenna.LOGICALANTENNA.PHYANTENNA.TILT,



        //                  FEEDERLENGTH = cldantenna.FEEDERLENGTH,
        //                  FEEDERTYPE = feeder.IDNAME,
        //                  CellName = logcell.IDNAME,
        //                  Sector = logcell.IDNAME.Substring(4, 1),
        //                  RRU_Type = flag2.FLAGS.FLAGID,

        //                  //12.04.2017 New way of GSM TRX counting. You don’t have to assign carriers anymore.
        //                  GSM_TRX = cellayData.CARLAYDATA.Sum(n => n.TRXREQUIRED),

        //                  GSM_Pwr_per_TRX = cellayData.OUTPUTPOWER,
        //                  UMTS_TRX = 0,
        //                  UMTS_Pwr_per_TRX = 0,
        //                  LTE_TRX = 0,
        //                  LTE_Pwr_per_TRX = 0,
        //                  LAYER_TECHNOLOGY = "GSM",
        //                  Band = cldantenna.CELLAY.IDNAME.Contains("900") ? "900" : "1800",
        //                  TMA = flag1.FLAGS.FLAGID,
        //                  COMBINER_SPLITTER = flag3.FLAGS.FLAGID,
        //                  SEC_COMBINER_SPLITTER = flag4.FLAGS.FLAGID,
        //                  CoLocation = flag5.FLAGS.FLAGID,
        //                  ANTENNA_MOUNT = flag6.FLAGS.FLAGID,
        //                  CellID = cell.GSMID,


        //              }

        //              ).Distinct();


        //               return outputGSM.ToList();
        //           }
        //       }

        //       private static IEnumerable<ViewModelTechnology> Get3GSiteAsset() //9.1 OK  with Distinct()
        //       {

        //           using (var contextAsset = new Entities())
        //           {
        //               var outputUMTS = (

        //                 from siteaddress in contextAsset.SITEADDRESS
        //                 from lognode in siteaddress.LOGNODE
        //                 from logcell in lognode.LOGCELL
        //                 from cell in logcell.LOGUMTSCELL
        //                 from umtsfeeder in cell.LOGUMTSFEEDER

        //                 join feeder in contextAsset.FEEDER on umtsfeeder.FEEDERTYPEFK equals feeder.FEEDERKEY

        //                 join antennaDevice in contextAsset.ANTENNADEVICE on umtsfeeder.LOGICALANTENNA.PHYANTENNA.DEVICEFK equals antennaDevice.DEVICEPK

        //                 from cellcar in cell.LOGUMTSCELLCAR

        //                 join tma in contextAsset.MASTHEADAMP on umtsfeeder.MHAMPTYPEFK equals tma.MHAKEY into tmaLeftJoin
        //                 from tma in tmaLeftJoin.DefaultIfEmpty()

        //                     //RNC part 
        //                 join munode in contextAsset.MUNODE
        //                     on new { prj = logcell.PROJECTNO, lognode = logcell.LOGNODEFK }
        //                     equals new { prj = munode.PROJECTNO, lognode = munode.LOGNODEPK }

        //                 join lognode1 in contextAsset.LOGNODE
        //                 on new { prj = munode.PROJECTNO, lognode = munode.LOGNODE.LOGNODEPK }
        //                 equals new { prj = lognode1.PROJECTNO, lognode = lognode1.LOGNODEPK }

        //                 join logconnection in contextAsset.LOGCONNECTION
        //                 on new { prj = lognode1.PROJECTNO, logconnection = lognode1.LOGNODEPK }
        //                 equals new { prj = logconnection.PROJECTNO, logconnection = logconnection.LOGNODEBFK }

        //                 join rnc in contextAsset.LOGRNC
        //                 on new { prj = logconnection.PROJECTNO, lognode = logconnection.LOGNODEAFK }
        //                 equals new { prj = rnc.PROJECTNO, lognode = rnc.LOGNODEPK }
        //                 //RNC part



        //                 join flag in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag = lognode.LOGNODEPK } equals new { flag.PROJECTNO, flag = flag.OBJECTKEY }
        //                 join flag2 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag2 = cell.LOGCELLPK } equals new { flag2.PROJECTNO, flag2 = flag2.OBJECTKEY }
        //                 join flag3 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag3 = cell.LOGCELLPK } equals new { flag3.PROJECTNO, flag3 = flag3.OBJECTKEY }
        //                 join flag4 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag4 = cell.LOGCELLPK } equals new { flag4.PROJECTNO, flag4 = flag4.OBJECTKEY }
        //                 join flag5 in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag5 = lognode.LOGNODEPK } equals new { flag5.PROJECTNO, flag5 = flag5.OBJECTKEY }
        //                 join flag6 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag6 = cell.LOGCELLPK } equals new { flag6.PROJECTNO, flag6 = flag6.OBJECTKEY }

        //                 where lognode.SITEADDRESS.PROJECTNO == 1 && lognode.SITEADDRESS.IDNAME.Contains(siteID)
        //                 && flag.FLAGGROUPS.FLAGGROUPID == "Candidate" && flag.FLAGS.FLAGID == "Accepted"
        //                 && flag2.FLAGGROUPS.FLAGGROUPID == "RRU type"
        //                 && flag3.FLAGGROUPS.FLAGGROUPID == "Combiner/Splitter"
        //                 && flag4.FLAGGROUPS.FLAGGROUPID == "2nd Combiner"
        //                 && flag5.FLAGGROUPS.FLAGGROUPID == "Co-Location"
        //                 && flag6.FLAGGROUPS.FLAGGROUPID == "Antenna Mounting"


        //                 select new ViewModelTechnology
        //                 {
        //                     Controler = rnc.LOGNODE.IDNAME,
        //                     SiteID = siteaddress.IDNAME,
        //                     Candidate = lognode.SITEADDRESS.IDNAME.Substring(10, 1),
        //                     SiteName = lognode.NAME,
        //                     SiteAddress = siteaddress.TOWN,
        //                     SiteAddress1 = siteaddress.ADDRESS1,
        //                     SiteAddress2 = siteaddress.ADDRESS2,
        //                     AntennaType = antennaDevice.IDNAME,
        //                     PHYINDEX = umtsfeeder.LOGICALANTENNA.PHYANTENNA.PHYINDEX,
        //                     Azimuth = umtsfeeder.LOGICALANTENNA.PHYANTENNA.AZIMUTH,
        //                     AGL = umtsfeeder.LOGICALANTENNA.PHYANTENNA.HEIGHT,
        //                     ARTL = umtsfeeder.LOGICALANTENNA.PHYANTENNA.HEIGHTOFFSET,
        //                     MECHANICAL_TILT = umtsfeeder.LOGICALANTENNA.PHYANTENNA.TILT,


        //                     FEEDERLENGTH = umtsfeeder.LENGTH,
        //                     FEEDERTYPE = feeder.IDNAME,
        //                     CellName = logcell.IDNAME,
        //                     Sector = logcell.IDNAME.Substring(4, 1),
        //                     RRU_Type = flag2.FLAGS.FLAGID,
        //                     GSM_TRX = 0,
        //                     GSM_Pwr_per_TRX = 0,
        //                     UMTS_TRX = 1,
        //                     UMTS_Pwr_per_TRX = cellcar.MAXTXPOWER,
        //                     LTE_TRX = 0,
        //                     LTE_Pwr_per_TRX = 0,
        //                     LAYER_TECHNOLOGY = "UMTS",
        //                     Band = cellcar.LOGUMTSCAR.TGCARRIER.DOWNLINKCH > 10000 ? "2100" : "900",
        //                     TMA = tma.IDNAME,
        //                     COMBINER_SPLITTER = flag3.FLAGS.FLAGID,
        //                     SEC_COMBINER_SPLITTER = flag4.FLAGS.FLAGID,
        //                     CoLocation = flag5.FLAGS.FLAGID,
        //                     ANTENNA_MOUNT = flag6.FLAGS.FLAGID,
        //                     CellID = cell.UMTSCELLID

        //                 }

        //                 ).Distinct();


        //               return outputUMTS.ToList();
        //           }
        //       }

        //       private static IEnumerable<ViewModelTechnology> Get4GSiteAsset() //9.1 OK  with Distinct()
        //       {

        //           using (var contextAsset = new Entities())
        //           {
        //               var outputLTE = (

        //                from siteaddress in contextAsset.SITEADDRESS
        //                from lognode in siteaddress.LOGNODE
        //                from logcell in lognode.LOGCELL
        //                from cell in logcell.LOGLTECELL
        //                from cellcar in cell.LOGLTECELLCAR

        //                from ltefeeder in cell.LOGLTEFEEDER
        //                join feeder in contextAsset.FEEDER on ltefeeder.FEEDERTYPEFK equals feeder.FEEDERKEY

        //                join antennaDevice in contextAsset.ANTENNADEVICE on ltefeeder.LOGICALANTENNA.PHYANTENNA.DEVICEFK equals antennaDevice.DEVICEPK

        //                join tma in contextAsset.MASTHEADAMP on ltefeeder.MHAMPTYPEFK equals tma.MHAKEY into tmaLeftJoin
        //                from tma in tmaLeftJoin.DefaultIfEmpty()



        //                join flag in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag = lognode.LOGNODEPK } equals new { flag.PROJECTNO, flag = flag.OBJECTKEY }
        //                join flag2 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag2 = cell.LOGCELLPK } equals new { flag2.PROJECTNO, flag2 = flag2.OBJECTKEY }
        //                join flag3 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag3 = cell.LOGCELLPK } equals new { flag3.PROJECTNO, flag3 = flag3.OBJECTKEY }
        //                join flag4 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag4 = cell.LOGCELLPK } equals new { flag4.PROJECTNO, flag4 = flag4.OBJECTKEY }
        //                join flag5 in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag5 = lognode.LOGNODEPK } equals new { flag5.PROJECTNO, flag5 = flag5.OBJECTKEY }
        //                join flag6 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag6 = cell.LOGCELLPK } equals new { flag6.PROJECTNO, flag6 = flag6.OBJECTKEY }

        //                where lognode.SITEADDRESS.PROJECTNO == 1 && lognode.SITEADDRESS.IDNAME.Contains(siteID)
        //                && flag.FLAGGROUPS.FLAGGROUPID == "Candidate" && flag.FLAGS.FLAGID == "Accepted"
        //                && flag2.FLAGGROUPS.FLAGGROUPID == "RRU type"
        //                && flag3.FLAGGROUPS.FLAGGROUPID == "Combiner/Splitter"
        //                && flag4.FLAGGROUPS.FLAGGROUPID == "2nd Combiner"
        //                && flag5.FLAGGROUPS.FLAGGROUPID == "Co-Location"
        //                && flag6.FLAGGROUPS.FLAGGROUPID == "Antenna Mounting"


        //                select new ViewModelTechnology
        //                {
        //                    Controler = " ",
        //                    SiteID = siteaddress.IDNAME,
        //                    Candidate = lognode.SITEADDRESS.IDNAME.Substring(10, 1),
        //                    SiteName = lognode.NAME,
        //                    SiteAddress = siteaddress.TOWN,
        //                    SiteAddress1 = siteaddress.ADDRESS1,
        //                    SiteAddress2 = siteaddress.ADDRESS2,
        //                    AntennaType = antennaDevice.IDNAME,
        //                    PHYINDEX = ltefeeder.LOGICALANTENNA.PHYANTENNA.PHYINDEX,
        //                    Azimuth = ltefeeder.LOGICALANTENNA.PHYANTENNA.AZIMUTH,
        //                    AGL = ltefeeder.LOGICALANTENNA.PHYANTENNA.HEIGHT,
        //                    ARTL = ltefeeder.LOGICALANTENNA.PHYANTENNA.HEIGHTOFFSET,
        //                    MECHANICAL_TILT = ltefeeder.LOGICALANTENNA.PHYANTENNA.TILT,


        //                    FEEDERLENGTH = ltefeeder.LENGTH,
        //                    FEEDERTYPE = feeder.IDNAME,
        //                    CellName = logcell.IDNAME,
        //                    Sector = logcell.IDNAME.Substring(4, 1),
        //                    RRU_Type = flag2.FLAGS.FLAGID,
        //                    GSM_TRX = 0,
        //                    GSM_Pwr_per_TRX = 0,
        //                    UMTS_TRX = 0,
        //                    UMTS_Pwr_per_TRX = 0,
        //                    LTE_TRX = 1,
        //                    LTE_Pwr_per_TRX = cellcar.MAXTXPOWER - (decimal)3.01,
        //                    LAYER_TECHNOLOGY = "LTE",
        //                    Band = cellcar.LOGLTECAR.LTECARRIER.IDNAME.Contains("1800") ? "1800" :
        //                           cellcar.LOGLTECAR.LTECARRIER.IDNAME.Contains("900") ? "900" : "2100",
        //                    TMA = tma.IDNAME,
        //                    COMBINER_SPLITTER = flag3.FLAGS.FLAGID,
        //                    SEC_COMBINER_SPLITTER = flag4.FLAGS.FLAGID,
        //                    CoLocation = flag5.FLAGS.FLAGID,
        //                    ANTENNA_MOUNT = flag6.FLAGS.FLAGID,
        //                    CellID = cell.CELLID

        //                }

        //                ).Distinct();

        //               return outputLTE.ToList();
        //           }


        //       }

        //       public static List<string> GetSites()
        //       {
        //           using (var contextAsset = new Entities())
        //           {
        //               var sites = contextAsset.Database.SqlQuery<string>(@"select distinct  substr(a.IDNAME,0,6) as site  from  network_planning.lognode a
        //inner join network_planning.SITEADDRESS d  on a.projectno = d.projectno AND a.addressfk = d.addresskey
        //inner join  network_planning.FLAGVALUES z on a.PROJECTNO = z.PROJECTNO and a.LOGNODEPK = z.OBJECTKEY
        //INNER JOIN network_planning.FLAGS y ON y.PROJECTNO = z.PROJECTNO AND y.FLAGGROUPKEY = z.FLAGGROUPKEY AND y.FLAGKEY = z.FLAGKEY
        //INNER JOIN network_planning.FLAGGROUPS x ON x.PROJECTNO = y.PROJECTNO AND x.FLAGGROUPKEY = y.FLAGGROUPKEY
        //where
        // (x.flaggroupid = 'Candidate') AND(y.flagid = 'Accepted') and LENGTH(substr(a.IDNAME, 0, 6)) = 6").ToList();
        //               return sites;
        //           }

        //       }

        private static List<ViewModelTechnology> Get2GSiteAsset()
        {
            using (var contextAsset = new Entities())
            {
                var outputGSM = (
                from siteaddress in contextAsset.SITEADDRESSes
                from lognode in siteaddress.LOGNODEs
                from logcell in lognode.LOGCELLs
                from cell in logcell.GSMCELLs
                from cellayData in cell.CELLAYDATAs
                from logcellfeeder in logcell.LOGCELLFEEDERs

                join feeder in contextAsset.FEEDERs on logcellfeeder.FEEDERFK equals feeder.FEEDERKEY

                join logicalantenna in contextAsset.LOGICALANTENNAs on logcellfeeder.LOGANTENNAFK equals logicalantenna.LOGANTENNAPK
                join phyantenna in contextAsset.PHYANTENNAs on logicalantenna.PHYANTENNAFK equals phyantenna.PHYANTENNAPK
                join antennaDevice in contextAsset.ANTENNADEVICEs on phyantenna.DEVICEFK equals antennaDevice.DEVICEPK


               //BSC part 
               join munode in contextAsset.MUNODEs
                    on new { prj = logcell.PROJECTNO, lognode = logcell.LOGNODEFK }
                    equals new { prj = munode.PROJECTNO, lognode = munode.LOGNODEPK }

                join lognode1 in contextAsset.LOGNODEs
                on new { prj = munode.PROJECTNO, lognode = munode.LOGNODE.LOGNODEPK }
                equals new { prj = lognode1.PROJECTNO, lognode = lognode1.LOGNODEPK }

                join logconnection in contextAsset.LOGCONNECTIONs
                on new { prj = lognode1.PROJECTNO, logconnection = lognode1.LOGNODEPK }
                equals new { prj = logconnection.PROJECTNO, logconnection = logconnection.LOGNODEBFK }

                join bsc in contextAsset.BSCs
                on new { prj = logconnection.PROJECTNO, lognode = logconnection.LOGNODEAFK }
                equals new { prj = bsc.PROJECTNO, lognode = bsc.LOGNODEPK }
                //BSC part



               join flag in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag = lognode.LOGNODEPK } equals new { flag.PROJECTNO, flag = flag.OBJECTKEY }
                join flag1 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag1 = cell.LOGCELLFK } equals new { flag1.PROJECTNO, flag1 = flag1.OBJECTKEY }
                join flag2 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag2 = cell.LOGCELLFK } equals new { flag2.PROJECTNO, flag2 = flag2.OBJECTKEY }
                join flag3 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag3 = cell.LOGCELLFK } equals new { flag3.PROJECTNO, flag3 = flag3.OBJECTKEY }
                join flag4 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag4 = cell.LOGCELLFK } equals new { flag4.PROJECTNO, flag4 = flag4.OBJECTKEY }
                join flag5 in contextAsset.FLAGVALUES on new { lognode.PROJECTNO, flag5 = lognode.LOGNODEPK } equals new { flag5.PROJECTNO, flag5 = flag5.OBJECTKEY }
                join flag6 in contextAsset.FLAGVALUES on new { cell.PROJECTNO, flag6 = cell.LOGCELLFK } equals new { flag6.PROJECTNO, flag6 = flag6.OBJECTKEY }

                where siteaddress.PROJECTNO == 1 && siteaddress.IDNAME.Contains(siteID)
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
                    SiteID = siteaddress.IDNAME,
                    Candidate = siteaddress.IDNAME.Substring(10, 1),
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
                   LOGINDEX = logicalantenna.INDEXNO,
                    PortNumber = logicalantenna.PORTS,


                    FEEDERLENGTH = logcellfeeder.FEEDERLEN,
                    FEEDERTYPE = feeder.IDNAME,
                    CellName = logcell.IDNAME,
                    Sector = logcell.IDNAME.Substring(4, 1),
                    RRU_Type = flag2.FLAG.FLAGID,

                   //12.04.2017 New way of GSM TRX counting. You don’t have to assign carriers anymore.
                   GSM_TRX = cellayData.CARLAYDATAs.Sum(n => n.TRXREQUIRED),

                    GSM_Pwr_per_TRX = cellayData.OUTPUTPOWER,
                    UMTS_TRX = 0,
                    UMTS_Pwr_per_TRX = 0,
                    LTE_TRX = 0,
                    LTE_Pwr_per_TRX = 0,
                    LAYER_TECHNOLOGY = "GSM",
                    Band = cellayData.CELLAY.IDNAME.Contains("900") ? "900" : "1800",
                    TMA = flag1.FLAG.FLAGID,
                    COMBINER_SPLITTER = flag3.FLAG.FLAGID,
                    SEC_COMBINER_SPLITTER = flag4.FLAG.FLAGID,
                    CoLocation = flag5.FLAG.FLAGID,
                    ANTENNA_MOUNT = flag6.FLAG.FLAGID,
                    CellID = cell.GSMID,

                }

                ).Distinct();
                var result = outputGSM.ToList();

                return result;
            }
        }

        static void Main(string[] args)
        {
            var db = new Entities();
            var queries = new Queries("SF1044");
            var m = queries.Get5GSiteAsset_Python();
            m.ForEach(n=>Console.WriteLine( n.CellID.ToString() ));
            Console.WriteLine("Putka");
            //List<ViewModelTechnology> site5G = queries.Get5GSiteAsset();

            //Console.WriteLine(site5G[0].Sector);
            var x = Get2GSiteAsset();
            Console.WriteLine();

            //   //var deserializedProduct = JsonConvert.DeserializeObject<List<Site>>(File.ReadAllText(@"c:\\data\\data.json"));




            //   JsonSerializer serializer = new JsonSerializer();
            ////serializer.NullValueHandling = NullValueHandling.Ignore;

            //var sites = GetSites();
            //var result = new List<Site>();
            //var error = new StringBuilder();

            //foreach (var siteID in sites)
            //{
            //    try
            //    {
            //        SupportFunc.CheckForCorrectSiteName(siteID);

                   
            //        var dto = new DTO_Load_IRFC(siteID, true);

            //        var site = dto.Site();
            //        result.Add(site);
            //        Console.WriteLine($"Site {siteID} is OK");

                   


            //    }
            //    catch (Exception ex)
            //    {
            //        var msg = $"Site {siteID} is not correct error: {ex.Message}";
            //        Console.WriteLine(msg);
            //        error.AppendLine(msg);
            //        continue;
            //    }

            //}

            //var outputJSON = new StringBuilder();
            //foreach (var item in result)
            //{
            //    outputJSON.Append(JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.Indented) + ",");
            //}

            //File.WriteAllText(@"c:\\data\\data.json", "[" + outputJSON + "]");
            ////foreach (var item in result)
            ////{

            ////    using (StreamWriter sw = new StreamWriter(@"c:\\data\\data.json", append:true))
            ////    using (JsonWriter writer = new JsonTextWriter(sw))
            ////    {
            ////        serializer.Serialize(writer, item);
            ////        // {"ExpiryDate":new Date(1230375600000),"Price":0}
            ////    }
            ////}
           

            //string json = JsonConvert.SerializeObject(result, Formatting.Indented);
            //File.AppendAllText(@"c:\data\siteInv.json", json);

            ////Console.WriteLine();

            //var x = Queries.GetAllAntennasPortsBands().Where(n => n.AntennaType == "80010992");


            ////var context = new Entities();

            ////var tst = context.GSMCELL.Where(n => n.AZIMUTH < 40).ToList();
            //var watch = new Stopwatch();
            //watch.Start();

            ////   Console.WriteLine("Start");
            ////   var t11 = GetAntennaPorts("738447");
            ////   Console.WriteLine("Done Get3GSiteAsset");
            //////   var t21 = EtiltAsset3G();
            ////   Console.WriteLine("Done  EtiltAsset3G");
            //var t31 = Get3GSiteAsset();

            //var t1 = Task<IEnumerable<ViewModelTechnology>>.Factory.StartNew(() => Get2GSiteAsset());
            ////var t2 = Task<IEnumerable<ViewModelTechnology>>.Factory.StartNew(() => Get3GSiteAsset());
            ////var t3 = Task<IEnumerable<ViewModelTechnology>>.Factory.StartNew(() => Get4GSiteAsset());

            ////Task.WaitAll(new Task[] { t1, t2, t3 });


            ////  var union = t1.Result.Union(t2.Result).Union(t3.Result);

            ////var view = Get3GSiteAsset().ToList();
            //Console.WriteLine(watch.Elapsed.TotalSeconds);
            ////watch.Restart();

            ////var test3= Get3GSiteAsset().ToList();
            ////Console.WriteLine(watch.Elapsed.TotalSeconds);
            ////watch.Restart();

            ////var test4 = Get4GSiteAsset().ToList();




        }
    }
}
