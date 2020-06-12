using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Roshart.Services;

namespace Roshart
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ShartSnifferOptions>(
                Configuration.GetSection(ShartSnifferOptions.SectionName));

            services.AddSingleton<ShartSniffer>();

            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapGet("/keybase.txt", HandleKeybase);
                endpoints.MapGet("/api/query", HandleApiQuery);
                endpoints.MapGet("/{slug}", HandleShortUrl);
            });
        }

        Task HandleKeybase(HttpContext httpContext)
        {
            var request = httpContext.Request;
            httpContext.Response.StatusCode = 301;
            httpContext.Response.Headers.Add(
                "Location", 
                $"{request.Scheme}://{request.Host}/{request.Host.Host}.keybase.txt");
            return Task.CompletedTask;
        }

        async Task HandleApiQuery(HttpContext httpContext)
        {
            if (!httpContext.TryGetShartContext(out var shartContext))
            {
                httpContext.Response.StatusCode = 404;
                return;
            }

            httpContext.Response.Headers.Add("Cache-Control", "no-store");

            IEnumerable<Shart> query = shartContext.Sharts;
            
            httpContext.Request.Query.TryGetLastString("order", out var order);

            query = order.ToLowerInvariant() switch
            {
                "asc" => query.OrderBy(shart => shart.Created),
                "random" => query.OrderBy(shart => Guid.NewGuid()),
                _ => query // default ordering is descending by timestamp, see ShartSniffer
            };

            if (httpContext.Request.Query.TryGetLastInt32("offset", out var offset) && offset > 0)
                query = query.Skip(offset);

            httpContext.Request.Query.TryGetLastInt32("limit", out var limit);

            var render = httpContext.Request.Query.ContainsKey("render");
            if (render)
            {    
                limit = 1;
                httpContext.Response.StatusCode = 302;
            }
            else
            {
                httpContext.Response.ContentType = "text/plain";
            }

            if (limit > 0)
                query = query.Take(limit);

            foreach (var shart in query)
            {
                var uri = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/{shart.Name}";

                if (render)
                {
                    httpContext.Response.Headers.Add("Location", uri);
                }
                else
                {
                    await httpContext.Response.WriteAsync(uri);
                    await httpContext.Response.WriteAsync("\n");
                }
            }
        }

        async Task HandleShortUrl(HttpContext httpContext)
        {
            if (httpContext.Request.Path == "/favicon.ico")
                return;

            if (httpContext.TryGetShartContext(out var shartContext) &&
                httpContext.Request.RouteValues["slug"] is string slug)
            {
                // all short links are allowed to end with a fake .gif file
                // extension since some [legacy] chat clients wouldn't show
                // the image inline if the URL didn't end in a file extension
                if (slug.EndsWith(".gif", true, CultureInfo.InvariantCulture))
                    slug = slug.Substring(0, slug.Length - 4);

                // we will try two slug variations for legacy compat
                for (var i = 0; i < 2; i++)
                {
                    if (shartContext.Sharts.TryGetShart(slug, out var shart))
                    {
                        httpContext.Response.ContentType = "image/gif";
                        httpContext.Response.ContentLength = shart.Length;
                        await httpContext.Response.SendFileAsync(shart);
                        return;
                    }

                    // In legacy shart, as a hint for the rewriter in nginx, each shart vhost's
                    // short links all started with a path '/<vhostFirstChar>' such as '/<c>SLUG'
                    // for <c>atoverflow.com with the slug SLUG, so support this as well to preserve
                    // any old links.
                    if (slug.Length > 1 &&
                        char.ToLowerInvariant(slug[0]) == char.ToLowerInvariant(httpContext.Request.Host.Host[0]))
                        slug = slug.Substring(1);
                }
            }
        }
    }
}
