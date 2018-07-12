using Microsoft.AspNetCore.Builder;

namespace API.Restful.Extensions
{
    public static class RequestResponseLoggingExtension
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<LogRequestMiddleware>();
            builder.UseMiddleware<LogResponseMiddleware>();
            return builder;
        }
    }
}
