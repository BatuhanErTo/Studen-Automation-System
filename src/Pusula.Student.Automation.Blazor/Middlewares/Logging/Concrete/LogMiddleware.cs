using Microsoft.AspNetCore.Http;
using Pusula.Student.Automation.Blazor.Middlewares.Logging.Abstract;
using Pusula.Student.Automation.Logging;
using Serilog;
using Serilog.Events;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Users;

namespace Pusula.Student.Automation.Blazor.Middlewares.Logging.Concrete;

public class LogMiddleware(ICurrentUser currentUser) : ILogMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var log = Log
            .ForContext(LoggingConsts.SystemLogPropertiesContextRequestBody, await ReadRequestBodyAsync(context))
            .ForContext(LoggingConsts.SystemLogPropertiesContextLoginUser, currentUser.Name ?? currentUser.UserName)
            .ForContext(LoggingConsts.SystemLogPropertiesContextLoginUserId, currentUser.Id)
            .ForContext(LoggingConsts.SystemLogPropertiesContextMethod, context.Request.Method)
            .ForContext(LoggingConsts.SystemLogPropertiesContextPath, context.Request.Path)
            .ForContext(LoggingConsts.SystemLogPropertiesContextQueryString, context.Request.QueryString)
            .ForContext(LoggingConsts.SystemLogPropertiesContextGroup, LoggingConsts.SystemLogGroupContextName);

        var stopwatch = Stopwatch.StartNew();
        var responseBody = await ReadResponseBodyAsync(context, next);
        stopwatch.Stop();
        log
            .ForContext(LoggingConsts.SystemLogPropertiesContextResponseBody, responseBody)
            .ForContext(LoggingConsts.SystemLogPropertiesContextStatusCode, context.Response.StatusCode)
            .ForContext(LoggingConsts.SystemLogPropertiesContextElapsedMilliseconds, stopwatch.ElapsedMilliseconds)
            .Write(GetLevelForStatusCode(context.Response.StatusCode), context.Request.Path);
    }

    private static async Task<string> ReadRequestBodyAsync(HttpContext context)
    {
        if (context.Request.ContentLength is null or 0)
            return string.Empty;

        context.Request.EnableBuffering();

        try
        {
            context.Request.Body.Position = 0;

            using var reader = new StreamReader(
                context.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);

            var bodyText = await reader.ReadToEndAsync();
            return bodyText ?? string.Empty;
        }
        catch (IOException ex)
        {
            Log.Debug(ex, "Connection lost");
            return string.Empty; 
        }
        finally
        {
            if (context.Request.Body.CanSeek)
                context.Request.Body.Position = 0;
        }
    }


    private static async Task<string> ReadResponseBodyAsync(HttpContext context, RequestDelegate next)
    {
        var originalResponseBody = context.Response.Body;

        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        try
        {
            await next(context);

            memoryStream.Position = 0;
            var responseBodyText = await new StreamReader(memoryStream).ReadToEndAsync();
            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(originalResponseBody);
            return responseBodyText;
        }
        finally
        {
            context.Response.Body = originalResponseBody;
        }    
    }
    private static LogEventLevel GetLevelForStatusCode(int statusCode)
    {
        if (statusCode >= 500)
            return LogEventLevel.Error;

        if (statusCode >= 400)
            return LogEventLevel.Warning;

        return LogEventLevel.Information;
    }
}
