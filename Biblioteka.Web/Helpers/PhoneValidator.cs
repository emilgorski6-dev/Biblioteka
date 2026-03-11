using System.Linq;

namespace Biblioteka.Web.Helpers
{
    public static class PhoneValidator
    {
        public static bool IsValid(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            if (phone.Length != 9)
                return false;

            if (!phone.All(char.IsDigit))
                return false;

            return true;
        }
    }
}