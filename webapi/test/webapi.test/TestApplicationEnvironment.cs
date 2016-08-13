using Microsoft.AspNet.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;

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
            var builder = TestServer.CreateBuilder()
                           .UseStartup<Startup>();
            Services = builder.Build().ApplicationServices;
        }

        public static IServiceProvider Services { get; private set; }
    }
}
