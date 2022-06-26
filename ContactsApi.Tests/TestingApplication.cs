using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class TestingApplication : WebApplicationFactory<ContactPerson>
{
   protected override IHost CreateHost(IHostBuilder builder)
   {
       builder.ConfigureServices(services =>
       {
           services.AddScoped<IContactService, ContactServiceTests>();
       });

       return base.CreateHost(builder);
   }
}