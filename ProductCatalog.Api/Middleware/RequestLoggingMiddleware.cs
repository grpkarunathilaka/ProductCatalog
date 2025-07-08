using System.Diagnostics;
using System.Text;

namespace ProductCatalog.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString();

            //Add correlation ID to the context
            context.Items["CorrelationId"] = requestId;

            //Log request
            await LogRequestAsync(context, requestId);

            //Store original response body stream
            var originalResponseBody = context.Response.Body;

            try
            {
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    await _next(context); 
                    stopwatch.Stop();

                    //Log response
                    await LogResponseAsync(context, requestId, stopwatch.ElapsedMilliseconds);

                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalResponseBody);
                }
            }
            catch (Exception)
            {
                stopwatch.Stop();
                _logger.LogError("Request {RequestId} failed after {ElaspsedMilliseconds}ms - {Method} {Path}", 
                    requestId, stopwatch.ElapsedMilliseconds, context.Request.Method, context.Request.Path);
                throw; 
            }
            finally
            {
                context.Response.Body = originalResponseBody; 
            }
        }

        private async Task LogRequestAsync(HttpContext context, string requestId)
        {
           var request = context.Request;

            var requestBody = string.Empty;
            if (request.ContentLength > 0 && request.ContentType?.Contains("application/json") == true)
            {
                request.EnableBuffering();
                var buffer = new byte[Convert.ToInt32(request.ContentLength)];
                await request.Body.ReadAsync(buffer, 0, buffer.Length);
                requestBody = Encoding.UTF8.GetString(buffer);
                request.Body.Position = 0;
            }

            _logger.LogInformation("Request {RequestId} started: {Method} {Path} {QueryString} - ContentType: {ContentType} - Body: {RequestBody}",
                requestId, 
                request.Method,
                request.Path, 
                request.QueryString, 
                request.ContentType, 
                requestBody);
        }

        private async Task LogResponseAsync(HttpContext context, string requestId, long elapsedMilliseconds)
        {
            var response = context.Response;

            var responseBody = string.Empty;

            if (response.ContentType?.Contains("application/json") == true)
            {
                response.Body.Seek(0, SeekOrigin.Begin);
                responseBody = await new StreamReader(response.Body).ReadToEndAsync();
                response.Body.Seek(0, SeekOrigin.Begin);
            }

            _logger.LogInformation("Request {RequestId} completed in {ElaspsedMilliseconds}ms - Status: {StatusCode} - ContentType: {ContentType} - Body: {ResponseBody}",
               requestId,
               elapsedMilliseconds,
               response.StatusCode,
               response.ContentType,
               responseBody);
        }
    }
}
