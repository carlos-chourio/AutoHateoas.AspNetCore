using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using AutoHateoas.AspNetCore.DTOs;
using AutoHateoas.AspNetCore.Common;

namespace AutoHateoas.AspNetCore.Filters {
    /// <summary>
    /// Generates the Hateoas links for a single resource.
    /// </summary>
    /// <typeparam name="TDto">The Dto for the result to be mapped</typeparam>
    public class Hateoas<TDto> : IAsyncResultFilter where TDto : class, IIdentityDto {
        private readonly LinkGenerator linkGenerator;
        private readonly FilterConfiguration filterConfiguration;

        public Hateoas(LinkGenerator linkGenerator, FilterConfiguration filterConfiguration) {
            this.linkGenerator = linkGenerator ?? throw new System.ArgumentNullException(nameof(linkGenerator));
            this.filterConfiguration = filterConfiguration ?? throw new System.ArgumentNullException(nameof(filterConfiguration));
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
            var result = context.Result as ObjectResult;
            if (FiltersHelper.IsResponseSuccesful(result)) {
                TDto dto = result.Value as TDto;
                if (filterConfiguration.SupportsCustomDataType && FiltersHelper.GetValueFromHeader(context, "Accept").Equals(filterConfiguration.CustomDataType)) {
                    var dtoWithLinks = HateoasHelper.CreateLinksForSingleResource(dto, filterConfiguration, linkGenerator, context.Controller.GetType());
                    var envelopDto = new EnvelopDto<TDto>(
                        dto,
                        dtoWithLinks.Links);
                    result.Value = envelopDto;
                } else {
                    result.Value = dto;
                }
            }
            await next();
        }

    }
}