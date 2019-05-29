using CcLibrary.AspNetCore.Validation;
using Microsoft.AspNetCore.Mvc;

namespace CcLibrary.AspNetCore.Common {
    public class PagingModel<TEntity> {
        private readonly int maxPageSize = 20;
        private int pageSize;
        public PagingModel() {
            PageSize = 10;
        }
        [ModelBinder(BinderType = typeof(ArrayModelBinder<int>))]
        public int[] Ids { get; set; }
        public int PageNumber { get; set; } = 1;        
        public string OrderBy { get; set; }
        public string SearchQuery { get; set; }
        [PropertiesExist(nameof(TEntity))]
        public string FieldsRequested { get; set; }
        public int PageSize {
            get { return pageSize > maxPageSize ? maxPageSize : pageSize; }
            set { pageSize = value; }
        }
    }
}