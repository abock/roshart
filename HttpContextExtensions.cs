using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Roshart.Services;

namespace Roshart
{
    static class HttpContextExtensions
    {
        public static bool TryGetShartContext(
            this HttpContext context,
            out ShartContext shartContext)
            => context.RequestServices
                .GetRequiredService<ShartSniffer>()
                .TryGetShartContext(
                    context.Request.Host.Host,
                    out shartContext);
    }
}
