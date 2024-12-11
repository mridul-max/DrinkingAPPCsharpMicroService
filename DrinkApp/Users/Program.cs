using Castle.Core.Resource;
using FluentValidation;
using MediatR;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Users.AppCrudRepositories;
using Users.DataAccess;
using Users.Repositories;
using Users.Security;
using Users.Security.TokenServices;
using Users.Services;
using Users.Validation;

namespace Users
{
    public class Program
    {
     
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults(worker => worker.UseNewtonsoftJson()
                .UseMiddleware<JwtMiddleware>())
                .ConfigureAppConfiguration(configs =>
                {
                    configs.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureOpenApi()
                .ConfigureServices(services =>
                {
                    
                    services.AddSingleton<ITokenService, TokenService>();         
                    services.AddSingleton<IPatientService, PatientService>();
                    services.AddSingleton<ICareGiverService, CareGiverService>();
                    services.AddSingleton<ILoginService, LoginService>();
                    services.AddSingleton<IPasswordHashService, PasswordHashService>();
                    services.AddSingleton<IEmailService, EmailService>();
                    services.AddValidatorsFromAssemblyContaining(typeof(RegisterPatientDTOValidator));            
                    services.AddSingleton<IPatientRepository, PatientRepository>();
                    services.AddSingleton<ICareGiverRepository, CareGiverRepository>();
                    services.AddDbContext<UserDbContext>();
                    services.AddScoped(typeof(IGenericRepository<>), typeof(EntityBaseRepository<>));
                    services.AddCors(options =>
                    {
                        options.AddDefaultPolicy(builder =>
                        {
                            builder
                                .AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                        });
                    });
                })
                .Build();
            await host.RunAsync();
        }
    }
}
