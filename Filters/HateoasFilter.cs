using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AutoMapper;
using Microsoft.AspNetCore.Routing;
using CcLibrary.AspNetCore.DTOs;
using CcLibrary.AspNetCore.Extensions;
using System.Dynamic;

namespace CcLibrary.AspNetCore.Filters {
    /// <summary>
    /// Maps The entity to the DTO and Generates the Hateoas links for a single resource.
    /// </summary>
    /// <typeparam name="TEntity">The entity of the result</typeparam>
    /// <typeparam name="TDtoGet">The Dto for the result to be mapped</typeparam>
    public class Hateoas<TEntity, TDtoGet> : IAsyncResultFilter 
        where TDtoGet : class, IIdentityDto
        where TEntity : class {

        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;
        private readonly FilterConfiguration filterConfiguration;

        public Hateoas(IMapper mapper, LinkGenerator linkGenerator, FilterConfiguration filterConfiguration) {
            this.mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
            this.linkGenerator = linkGenerator ?? throw new System.ArgumentNullException(nameof(linkGenerator));
            this.filterConfiguration = filterConfiguration ?? throw new System.ArgumentNullException(nameof(filterConfiguration));
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
            var result = context.Result as ObjectResult;
            if (FiltersHelper.IsResponseSuccesful(result)) {
                TEntity entity = result.Value as TEntity;
                string fieldsRequested = FiltersHelper.GetValueFromQueryString(context, "fieldsRequested");
                var dto = mapper.Map<TDtoGet>(entity);
                if (filterConfiguration.SupportsCustomDataType && FiltersHelper.GetValueFromHeader(context, "Accept").Equals(filterConfiguration.CustomDataType)) {
                    var dtoWithLinks = FiltersHelper.CreateLinksForSingleResource(dto, filterConfiguration, linkGenerator, context.Controller.GetType());
                    var envelopDto = new EnvelopDto<ExpandoObject>() {
                        Value = dto.ShapeDataWithRequestedFields(fieldsRequested),
                        Links = dtoWithLinks.Links
                    };
                    result.Value = envelopDto;
                } else {
                    result.Value = dto.ShapeDataWithRequestedFields(fieldsRequested);
                }
            }
            await next();
        }

    }

    /// <summary>
    /// Generates the Hateoas links for a single resource.
    /// </summary>
    /// <typeparam name="TDtoGet">The Type of the ObjectResult </typeparam>
    public class Hateoas<TDtoGet> : IAsyncResultFilter 
        where TDtoGet : class, IIdentityDto
        {

        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;
        private readonly FilterConfiguration filterConfiguration;

        public Hateoas(IMapper mapper, LinkGenerator linkGenerator, FilterConfiguration filterConfiguration) {
            this.mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
            this.linkGenerator = linkGenerator ?? throw new System.ArgumentNullException(nameof(linkGenerator));
            this.filterConfiguration = filterConfiguration ?? throw new System.ArgumentNullException(nameof(filterConfiguration));
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
            var result = context.Result as ObjectResult;
            if (FiltersHelper.IsResponseSuccesful(result)) {
                TDtoGet dto = result.Value as TDtoGet;
                string fieldsRequested = FiltersHelper.GetValueFromQueryString(context, "fieldsRequested");
                if (filterConfiguration.SupportsCustomDataType && FiltersHelper.GetValueFromHeader(context, "Accept").Equals(filterConfiguration.CustomDataType)) {
                    var dtoWithLinks = FiltersHelper.CreateLinksForSingleResource(dto, filterConfiguration, linkGenerator, context.Controller.GetType());
                    var envelopDto = new EnvelopDto<ExpandoObject>() {
                        Value = dto.ShapeDataWithRequestedFields(fieldsRequested),
                        Links = dtoWithLinks.Links
                    };
                    result.Value = envelopDto;
                } else {
                    result.Value = dto.ShapeDataWithRequestedFields(fieldsRequested);
                }
            }
            await next();
        }

    }
}