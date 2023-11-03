using BLL_Atoll.Enums;

namespace BLL_Atoll
{
    public static class Global
    {
        public const string WORKING_DIRECTORY = @"D:\MEDIATION\Shiva\";

        public const string CONN_STR_ATOLL = @"Server=10.26.43.120;Database=ATOLL_5GMRAT;
                        User Id=atolladmin;Password=atolladmin123;Connection Timeout=600";

        public const string CONN_STR_SHIVAL = @"Server=10.26.43.120;Database=shiva;
                        User Id=atolladmin;Password=atolladmin123;Connection Timeout=600";

        private const string QUERIES_PATH = $@"{WORKING_DIRECTORY}Queries\";

        public const string QUERY_PATH_ALLELEMENTS = $@"{QUERIES_PATH}all_elements.sql";

        public const string QUERY_PATH_FEEDERLOSS = @$"{QUERIES_PATH}feeder_loss.sql";

        public const string QUERY_ANTENNA_PORT_MAP = @$"{QUERIES_PATH}antenna_port_map.sql";
        
        public const string QUERY_SEC_REMOTE_EMPTY_ANTENNA = @$"{QUERIES_PATH}secondary_remote_empty_antenna.sql";

        public const string PATTERN_SITEID = "@SiteID@";

        public const string PATTERN_JOIN_LEFT_RIGHT = "@left_or_right@";

        public static readonly Dictionary<ReportType, string> REPORT_TYPE_VS_RRU_JOIN =
            new()
            {
                { ReportType.IRFC, "left"},
                { ReportType.PSK, "left"},
                { ReportType.SA, "right"},
                { ReportType.SRF, "right"}
            };

        public const string PATTERN_MAX_SECTORS = "@MAX_SECTORS@";

        public const string PATTERN_MAX_ANTENNAS = "@MAX_ANTENNAS@";
       
        public const string DROP_DOWNS = @$"{WORKING_DIRECTORY}DropDowns.json";
       
        public const string COMBINER_SPLITER_LOSS = @$"{WORKING_DIRECTORY}Combiner_Splitter_Loss.txt";
       
        public const string EXPORT_EXCEL_FILE_DIR = @$"{WORKING_DIRECTORY}Output\";
       
        public const string ERRORS_DIR = @$"{WORKING_DIRECTORY}Errors\";
        public static string CurrentTime => DateTime.Now.ToString("yyyyMMddHHmm");

    }
}
