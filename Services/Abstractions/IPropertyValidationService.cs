namespace CcLibrary.AspNetCore.Services.Abstractions {
    public interface IPropertyValidationService {
        bool ValidateProperties<TType>(string fieldsSeparatedByCommas);
    }
}