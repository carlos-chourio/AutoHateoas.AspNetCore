using CcLibrary.AspNetCore.Common;
using CcLibrary.AspNetCore.Services;
using CcLibrary.AspNetCore.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CcLibrary.AspNetCore.Extensions {
    public static class IServiceCollectionExtensions {
        public static void AddPagingHelper(this IServiceCollection serviceCollection) {
            serviceCollection.AddTransient(typeof(IPagingHelperService<>), typeof(PagingHelperService<>));
        }
        public static void AddPropertyMappingService(this IServiceCollection serviceCollection) {
            serviceCollection.AddTransient<IPropertyMappingService, PropertyMappingService>();
        }

        public static void AddPropertyValidationService<TProfile>(this IServiceCollection serviceCollection) 
                where TProfile : MappingProfile {
            serviceCollection.AddTransient<MappingProfile, TProfile>();
        }
    }
}