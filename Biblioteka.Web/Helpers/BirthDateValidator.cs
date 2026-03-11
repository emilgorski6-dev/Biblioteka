using System;

namespace Biblioteka.Web.Helpers
{
    public static class BirthDateValidator
    {
        public static bool IsValid(DateTime date)
        {
            if (date > DateTime.Now)
                return false;

            if (date < DateTime.Now.AddYears(-120))
                return false;

            return true;
        }
    }
}