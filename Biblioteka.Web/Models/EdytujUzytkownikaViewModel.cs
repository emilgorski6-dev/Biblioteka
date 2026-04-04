using System;
using System.ComponentModel.DataAnnotations;
using Biblioteka.Web.Data.Entities;

namespace Biblioteka.Web.Models
{
    public class EdytujUzytkownikaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Login jest wymagany")]
        public required string Login { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ ]+$")]
        public required string Imie { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ \-]+$")]
        public required string Nazwisko { get; set; }

        [Required(ErrorMessage = "PESEL jest wymagany")]
        [RegularExpression(@"^[0-9]{11}$")]
        public required string Pesel { get; set; }

        [Required(ErrorMessage = "Data urodzenia jest wymagana")]
        public DateTime? DataUrodzenia { get; set; }

        [Required(ErrorMessage = "Płeć jest wymagana")]
        public TypPlci? Plec { get; set; } // ZMIANA NA ENUM (to usunie błąd .Value)

        [Required(ErrorMessage = "Email jest wymagany")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Telefon jest wymagany")]
        [RegularExpression(@"^[0-9]{9}$")]
        public required string Telefon { get; set; }

        [Required(ErrorMessage = "Miejscowość jest wymagana")]
        public required string Miejscowosc { get; set; }

        [Required(ErrorMessage = "Kod pocztowy jest wymagany")]
        [RegularExpression(@"^[0-9]{2}-[0-9]{3}$")]
        public required string KodPocztowy { get; set; }

        public string? Ulica { get; set; }
        [Required] public required string NumerPosesji { get; set; }
        public string? NumerLokalu { get; set; }
    }
}