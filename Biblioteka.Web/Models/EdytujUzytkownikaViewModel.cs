using System;
using System.ComponentModel.DataAnnotations;
using Biblioteka.Web.Data.Entities;

namespace Biblioteka.Web.Models
{
    public class EdytujUzytkownikaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Login jest wymagany")]
        [StringLength(20)]
        public required string Login { get; set; } // Readonly w widoku, ale walidowane przy odbiorze

        [Required(ErrorMessage = "Imię jest wymagane")]
        [StringLength(50, ErrorMessage = "Imię nie może przekraczać 50 znaków")]
        [RegularExpression(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ ]+$", ErrorMessage = "Imię może zawierać tylko litery")]
        public required string Imie { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [StringLength(50, ErrorMessage = "Nazwisko nie może przekraczać 50 znaków")]
        [RegularExpression(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ \-]+$", ErrorMessage = "Nazwisko może zawierać tylko litery i myślnik")]
        public required string Nazwisko { get; set; }

        [Required(ErrorMessage = "PESEL jest wymagany")]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "Numer PESEL musi składać się z 11 cyfr")]
        public required string Pesel { get; set; }

        [Required(ErrorMessage = "Data urodzenia jest wymagana")]
        public DateTime? DataUrodzenia { get; set; }

        [Required(ErrorMessage = "Płeć jest wymagana")]
        public TypPlci? Plec { get; set; }

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu e-mail")]
        [MaxLength(255, ErrorMessage = "Adres email powinien zawierać maksymalnie 255 znaków.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Adres e-mail zawiera niedozwolone znaki")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Telefon jest wymagany")]
        [RegularExpression(@"^[0-9]{9}$", ErrorMessage = "Numer telefonu musi składać się z dokładnie 9 cyfr")]
        public required string Telefon { get; set; }

        [Required(ErrorMessage = "Miejscowość jest wymagana")]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ ]+$", ErrorMessage = "Miejscowość może zawierać tylko litery")]
        public required string Miejscowosc { get; set; }

        [Required(ErrorMessage = "Kod pocztowy jest wymagany")]
        [RegularExpression(@"^[0-9]{2}-[0-9]{3}$", ErrorMessage = "Kod pocztowy musi mieć format 00-000")]
        public required string KodPocztowy { get; set; }

        [Required(ErrorMessage = "Numer posesji jest wymagany")]
        [StringLength(10)]
        [RegularExpression(@"^[a-zA-Z0-9/ ]+$", ErrorMessage = "Nieprawidłowy format numeru posesji")]
        public required string NumerPosesji { get; set; }

        [StringLength(100)]
        public string? Ulica { get; set; }

        [StringLength(10)]
        public string? NumerLokalu { get; set; }
    }
}