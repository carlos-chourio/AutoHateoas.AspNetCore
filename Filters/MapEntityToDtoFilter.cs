using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AutoMapper;
using Microsoft.AspNetCore.Routing;
using CcLibrary.AspNetCore.DTOs;
using CcLibrary.AspNetCore.Extensions;

namespace CcLibrary.AspNetCore.Filters {
    public class MapEntityToDtoFilter<TEntity, TDtoGet> : IAsyncResultFilter 
        where TDtoGet : class, IIdentityDto
        where TEntity : class {

        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;
        private readonly FilterConfiguration filterConfiguration;

        public MapEntityToDtoFilter(IMapper mapper, LinkGenerator linkGenerator, FilterConfiguration filterConfiguration) {
            this.mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
            this.linkGenerator = linkGenerator ?? throw new System.ArgumentNullException(nameof(linkGenerator));
            this.filterConfiguration = filterConfiguration ?? throw new System.ArgumentNullException(nameof(filterConfiguration));
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
            var result = context.Result as ObjectResult;
            if (FiltersHelper.IsResponseSuccesful(result)) {
                TEntity entity = result.Value as TEntity;
                string fieldsRequested = FiltersHelper.GetValueFromQueryString(context, "fieldsRequested");
                if (FiltersHelper.GetValueFromHeader(context, "Accept").Equals(filterConfiguration.CustomDataType)) {
                    // Missing links generation

                } else {
                    var dto = mapper.Map<TDtoGet>(entity);
                    result.Value = dto.ShapeDataWithRequestedFields(fieldsRequested);
                }
            }
            await next();
        }

    }
}