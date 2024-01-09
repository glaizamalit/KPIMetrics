using log4net;

namespace KPIMetrics
{
    public class LogManager
    {
        public static readonly ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}