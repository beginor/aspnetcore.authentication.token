using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Beginor.AspNetCore.Authentication.Token;
using WebTest.Providers;

namespace WebTest;

public class Startup {

    public Startup(IConfiguration configuration) {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
        services.Configure<UserTokenOptions>(
            Configuration.GetSection("userTokens")
        );
        services.AddSingleton<UserTokenProvider>();
        services.AddAuthentication(TokenOptions.DefaultSchemaName)
            .AddToken(options => {
                options.Events = new TokenEvents {
                    OnTokenReceived = context => {
                        var provider = context.HttpContext
                            .RequestServices
                            .GetService<UserTokenProvider>();
                        var token = provider!.GetById(context.Token!);
                        if (token == null) {
                            context.Fail("Invalid token!");
                            return Task.CompletedTask;
                        }
                        var claims = new List<Claim> {
                            new(ClaimTypes.NameIdentifier, token.Id!),
                            new(ClaimTypes.Name, token.Username!)
                        };
                        claims.AddRange(token.Roles!.Select(role => new Claim(ClaimTypes.Role, role)));
                        var identity = new ClaimsIdentity(claims, context.Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        context.Principal = principal;
                        context.Success();
                        return Task.CompletedTask;
                    }
                };
            });
        services.AddControllers();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
        if (env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => {
            endpoints.MapControllers();
        });
    }

}
