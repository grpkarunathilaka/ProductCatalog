

using System.Text;

namespace ProductCatalog.Api.Middleware
{
    public class AuditLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditLoggingMiddleware> _logger;
        private readonly List<string> _auditablePaths = new() { "/api/products" }; 
        private readonly List<string> _auditableMethods = new() { "POST", "PUT", "DELETE" }; 

        public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var shouldAudit = ShouldAuditRequest(context);

            if (shouldAudit)
            {
                await LogAuditAsync(context);
            }

            await _next(context);
        }

        private async Task LogAuditAsync(HttpContext context)
        {
            var request = context.Request;

            var auditLog = new AuditLog
            {
                Timestamp = DateTime.UtcNow,
                Method = request.Method,
                Path = request.Path,
                QueryString = request.QueryString.ToString(),
                UserAgent = request.Headers.UserAgent.ToString(),
                RemoteIpAddress = context.Connection.RemoteIpAddress?.ToString(),
                CorrelationId = context.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString()
            };

            //Capture request body for POST/PUT operations
            if (request.ContentLength > 0 && request.ContentType?.Contains("application/json") == true)
            {
                request.EnableBuffering();
                var buffer = new byte[Convert.ToInt32(request.ContentLength)];
                await request.Body.ReadAsync(buffer, 0, buffer.Length);
                auditLog.RequestBody = Encoding.UTF8.GetString(buffer);
                request.Body.Position = 0; 
            }

            _logger.LogInformation("Audit Log: {@AuditLog}", auditLog);
        }

        private bool ShouldAuditRequest(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
            var method = context.Request.Method.ToUpperInvariant();

            return _auditableMethods.Contains(method) &&
                   _auditablePaths.Any(auditablePath => path.StartsWith(auditablePath.ToLower()));
        }
    }

    public class AuditLog
    {
        public DateTime Timestamp { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string UserAgent { get; set; }
        public string RemoteIpAddress { get; set; }
        public string CorrelationId { get; set; }
        public string RequestBody { get; set; }
    }
}
