using CcLibrary.AspNetCore.Common;
using CcLibrary.AspNetCore.Filters;
using CcLibrary.AspNetCore.Services;
using CcLibrary.AspNetCore.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CcLibrary.AspNetCore.Extensions {
    public static class IServiceCollectionExtensions {
        public static IServiceCollection AddPropertyMappingService(this IServiceCollection serviceCollection) {
            return serviceCollection.AddTransient<IPropertyMappingService, PropertyMappingService>();
        }

        public static IServiceCollection AddPropertyValidationService<TProfile>(this IServiceCollection serviceCollection) 
                where TProfile : MappingProfile {
            return serviceCollection.AddTransient<MappingProfile, TProfile>();
        }

        public static IServiceCollection AddAutomaticHateoas(this IServiceCollection serviceCollection) {
            return serviceCollection
                    .AddSingleton<FilterConfiguration>()
                    .AddTransient(typeof(IPaginationHelperService<>), typeof(PaginationHelperService<>))
                    .AddScoped(typeof(AddPaginationHeaderFilter<>), typeof(AddPaginationHeaderFilter<>))
                    .AddScoped(typeof(PerformPaginationFilter<,>), typeof(PerformPaginationFilter<,>))
                    .AddScoped(typeof(Hateoas<,>), typeof(Hateoas<,>));
        }
    }
}