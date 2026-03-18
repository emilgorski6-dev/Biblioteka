using System.Linq;

namespace Biblioteka.Web.Helpers
{
    public static class PhoneValidator
    {
        public const string MsgInvalidFormat = "Numer telefonu musi zawierać dokładnie 9 cyfr.";
        
        public static (bool IsValid, string Message) WalidujNrTelefonu(string phone)
        {
            if (phone.Length != 9 || !phone.All(char.IsDigit))
                return (false, MsgInvalidFormat);

            return (true, string.Empty);
        }
    }
}