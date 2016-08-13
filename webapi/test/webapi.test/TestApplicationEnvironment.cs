using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Runtime.Versioning;

namespace webapi.test
{
    internal class TestApplicationEnvironment
    {
        static TestApplicationEnvironment()
        {
            InitTestServer();
        }

        internal static void InitTestServer()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>();
            var testServer = new TestServer(builder);
            Services = builder.Build().Services;
        }

        public static IServiceProvider Services { get; private set; }

        public string ApplicationBasePath { get; set; }

        public string ApplicationName { get; set; }

        public string ApplicationVersion => PlatformServices.Default.Application.ApplicationVersion;

        public FrameworkName RuntimeFramework => PlatformServices.Default.Application.RuntimeFramework;

    }
}
