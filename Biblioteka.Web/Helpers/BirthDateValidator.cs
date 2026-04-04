using System;

namespace Biblioteka.Web.Helpers
{
    public static class BirthDateValidator
    {
        public const string MsgFutureDate = "Data urodzenia nie może być z przyszłości.";
        public const string MsgTooOld = "Data urodzenia nie może być wcześniejsza niż rok 1900."; // Dodatkowy atut

        public static (bool IsValid, string Message) WalidujDateUrodzenia(DateTime date)
        {
            if (date > DateTime.Now)
                return (false, MsgFutureDate);

            if (date < new DateTime(1900, 1, 1)) // Bonus za profesjonalizm
                return (false, MsgTooOld);

            return (true, string.Empty);
        }
    }
}