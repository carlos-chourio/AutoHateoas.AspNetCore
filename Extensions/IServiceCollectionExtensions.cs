using AutoHateoas.AspNetCore.Common;
using AutoHateoas.AspNetCore.Filters;
using AutoHateoas.AspNetCore.Services;
using AutoHateoas.AspNetCore.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AutoHateoas.AspNetCore.Extensions {
    public static class IServiceCollectionExtensions {
        public static IServiceCollection AddPropertyMappingService(this IServiceCollection serviceCollection) {
            return serviceCollection.AddTransient<IPropertyMappingService, PropertyMappingService>();
        }

        public static IServiceCollection AddPropertyValidationService<TProfile>(this IServiceCollection serviceCollection) 
                where TProfile : MappingProfile {
            return serviceCollection.AddTransient<MappingProfile, TProfile>();
        }

        public static IServiceCollection AddAutoHateoas(this IServiceCollection serviceCollection) {
            return serviceCollection
                    .AddSingleton<FilterConfiguration>()
                    .AddTransient(typeof(IPaginationHelperService<>), typeof(PaginationHelperService<>))
                    .AddScoped(typeof(HateoasAutoPagination<,>), typeof(HateoasAutoPagination<,>))
                    .AddScoped(typeof(HateoasForCollection<,>), typeof(HateoasForCollection<,>))
                    .AddScoped(typeof(Hateoas<>), typeof(Hateoas<>));
        }
    }
}