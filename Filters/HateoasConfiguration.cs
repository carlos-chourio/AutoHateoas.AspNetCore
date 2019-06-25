using System;
using System.Reflection;

namespace AutoHateoas.AspNetCore.Filters {
    public class HateoasConfiguration {
        public string CustomDataType { get; set; }
        public Type[] CustomPaginationModelTypes { get; set; }
        internal Assembly CurrentAssembly { get; set; }
        internal bool SupportsCustomDataType { get; set; }
        internal bool SupportsCustomPaginationModel { get; set; }
    }
}