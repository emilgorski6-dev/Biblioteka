using System;

namespace Biblioteka.Web.Helpers
{
    public static class BirthDateValidator
    {
        public const string MsgFutureDate = "Data urodzenia nie może być z przyszłości.";

        public static (bool IsValid, string Message) WalidujDateUrodzenia(DateTime date)
        {
            if (date > DateTime.Now)
                return (false, MsgFutureDate);

            return (true, string.Empty);
        }
    }
}