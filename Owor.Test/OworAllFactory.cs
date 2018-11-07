using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Owor.All;
using Owor.ClientLib.Devices;
using System.Linq;
using System;
using Owor.Api.BuilderExtensions;
using Owor.Api.Controllers;
using Microsoft.AspNetCore.TestHost;
using Owor.Shared;
using System.Threading.Tasks;

namespace Owor.Test
{

    public class OworAllFactory<TStartup> : WebApplicationFactory<Startup>
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

        }

    }

}