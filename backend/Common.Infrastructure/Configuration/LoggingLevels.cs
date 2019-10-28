using Serilog.Events;

namespace Common.Infrastructure.Configuration
{
    public class LoggingLevels
    {
        public LogEventLevel Authentication { get; set; }
        public LogEventLevel Database { get; set; }
        public LogEventLevel General { get; set; }
    }
}
