using LoggerLib.Interfaces;
using LoggerLib.Types;

namespace LoggerLib.Executes;

public class LoggingServiceFactory : ILoggingServiceFactory
{
    public ILoggingService Create(LoggingType? type)
    {
        return type switch
        {
            LoggingType.Serilog => new SerilogLoggingService(),
            LoggingType.NLog => new NLogLoggingService(),
            LoggingType.Custom => new CustomFileLoggingService(),
        };
    }
}
