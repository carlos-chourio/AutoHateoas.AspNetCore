using AutoHateoas.AspNetCore.Validation;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoHateoas.AspNetCore.Common {
    public class ValidatedPaginationModel<TEntity> : PaginationModel {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!PropertiesValidator.PropertiesExistInType(typeof(TEntity), Fields)) {
                yield return new ValidationResult("The fields requested are invalid", new[] { nameof(Fields) });
            }
        }
    }

    public class SamplePaginationModel : PaginationModel {
        public string SampleProperty { get; set; }
    }

    public class PaginationModel : PaginationModelBase, IOrdered, ISearched, IFindByIds, IShaped {
        public PaginationModel() {
            PageSize = 10;
        }
        public string OrderBy { get; set; }
        public string SearchQuery { get; set; }
        public int[] Ids { get; set; }
        public string Fields { get; set; }

        public PaginationModel Clone() {
            return new PaginationModel
            {
                Fields = Fields,
                Ids = Ids,
                OrderBy = OrderBy,
                PageNumber = PageNumber,
                PageSize = PageSize,
                SearchQuery = SearchQuery
            };
        }
    }

    public class PaginationModelBase {
        private readonly int maxPageSize = 20;
        private int pageSize;
        public int PageNumber { get; set; } = 1;
        public int PageSize {
            get { return pageSize > maxPageSize ? maxPageSize : pageSize; }
            set { pageSize = value; }
        }
    }

    public interface IOrdered  {
        string OrderBy { get; set; }
    }

    public interface ISearched {
        string SearchQuery { get; set; }
    }

    public interface IFindByIds {
        [ModelBinder(BinderType = typeof(ArrayModelBinder<int>))]
        int[] Ids { get; set; }
    }

    public interface IShaped {
        string Fields { get; set; }
    }
}
