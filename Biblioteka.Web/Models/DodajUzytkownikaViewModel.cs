using System;
using System.ComponentModel.DataAnnotations;
using Biblioteka.Web.Data.Entities;

namespace Biblioteka.Web.Models
{
    public class DodajUzytkownikaViewModel
    {
        [Required(ErrorMessage = "Login jest wymagany")]
        [StringLength(20, ErrorMessage = "Login nie może przekraczać 20 znaków")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Login może zawierać tylko litery, cyfry i podkreślnik")]
        public required string Login { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane")]
        [StringLength(50, ErrorMessage = "Imię nie może przekraczać 50 znaków")]
        [RegularExpression(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ ]+$", ErrorMessage = "Imię może zawierać tylko litery")]
        public required string Imie { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [StringLength(50, ErrorMessage = "Nazwisko nie może przekraczać 50 znaków")]
        [RegularExpression(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ \-]+$", ErrorMessage = "Nazwisko może zawierać tylko litery i myślnik")]
        public required string Nazwisko { get; set; }

        [Required(ErrorMessage = "Adres e-mail jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu e-mail")]
        [StringLength(255, ErrorMessage = "Email nie może przekraczać 255 znaków")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Adres e-mail zawiera niedozwolone znaki")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Numer telefonu jest wymagany")]
        [RegularExpression(@"^[0-9]{9}$", ErrorMessage = "Numer telefonu musi składać się z dokładnie 9 cyfr")]
        public required string Telefon { get; set; }

        [Required(ErrorMessage = "Numer PESEL jest wymagany")]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "Numer PESEL musi składać się z 11 cyfr")]
        public required string Pesel { get; set; }

        [Required(ErrorMessage = "Data urodzenia jest wymagana")]
        public DateTime? DataUrodzenia { get; set; }

        [Required(ErrorMessage = "Płeć jest wymagana")]
        public TypPlci? Plec { get; set; }

        [Required(ErrorMessage = "Miejscowość jest wymagana")]
        [StringLength(100, ErrorMessage = "Miejscowość jest zbyt długa")]
        [RegularExpression(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ ]+$", ErrorMessage = "Miejscowość może zawierać tylko litery")]
        public required string Miejscowosc { get; set; }

        [Required(ErrorMessage = "Kod pocztowy jest wymagany")]
        [RegularExpression(@"^[0-9]{2}-[0-9]{3}$", ErrorMessage = "Kod pocztowy musi mieć format 00-000")]
        public required string KodPocztowy { get; set; }

        [Required(ErrorMessage = "Numer posesji jest wymagany")]
        [StringLength(10, ErrorMessage = "Numer posesji jest zbyt długi")]
        [RegularExpression(@"^[a-zA-Z0-9/ ]+$", ErrorMessage = "Nieprawidłowy format numeru posesji")]
        public required string NumerPosesji { get; set; }

        [StringLength(100, ErrorMessage = "Nazwa ulicy jest zbyt długa")]
        public string? Ulica { get; set; }

        [StringLength(10, ErrorMessage = "Numer lokalu jest zbyt długi")]
        public string? NumerLokalu { get; set; }
    }
}