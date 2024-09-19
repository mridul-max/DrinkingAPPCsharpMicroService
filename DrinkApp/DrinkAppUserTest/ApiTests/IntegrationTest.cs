/*using FluentAssertions.Common;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http.Headers;
using Users;
using Users.DataAccess;

namespace DrinkAppUserTest.ApiTests
{
    public class IntegrationTest
    {
        protected readonly HttpClient _httpClient;
        public IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(DbContext));
                        services.AddDbContext<DbContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestDb");
                        });
                    });
                });
            _httpClient = appFactory.CreateClient();
        }
        public void Dispose()
        {
            _httpClient.Dispose();
        }
       *//* protected async Task AuthenticateAsync()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }
        private async Task<string> GetJwtAsync()
        {
            var response = _httpClient.PostAsJsonAsync(new){

            }
        }*//*
    }
}
*/