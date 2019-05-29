namespace CcLibrary.AspNetCore.Common {
    public class ErrorModel {
        public string ErrorMessage { get; set; }
        public ErrorModel(string errorMessage) {
            ErrorMessage = errorMessage;
        }
    }
}
