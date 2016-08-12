using Microsoft.Extensions.DependencyInjection;
using Microsoft.ProjectOxford.Face;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapi
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddFaceService(this IServiceCollection serviceCollection)
        {
            IFaceServiceClient faceServiceClient = new FaceServiceClient("6eca31c3a55a4968bfae16fc35fb54df");
            serviceCollection.AddSingleton<IFaceServiceClient>(new Func<IServiceProvider, IFaceServiceClient>(p => faceServiceClient));
            return serviceCollection;
        }
    }
}
