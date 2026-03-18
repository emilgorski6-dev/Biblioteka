using System;
using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Web.Models
{
    public class EdytujUzytkownikaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Login jest wymagany")]
        public required string Login { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane")]
        public required string Imie { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        public required string Nazwisko { get; set; }

        [Required(ErrorMessage = "PESEL jest wymagany")]
        public required string Pesel { get; set; }

        [Required(ErrorMessage = "Data urodzenia jest wymagana")]
        public DateTime? DataUrodzenia { get; set; }

        [Required(ErrorMessage = "Płeć jest wymagana")]
        public required string Plec { get; set; }

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format email")]
        [MaxLength(255, ErrorMessage = "Niepoprawna długość adres email. Adres email powinien zawierać maksymalnie 255 znaków.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Telefon jest wymagany")]
        public required string Telefon { get; set; }

        [Required(ErrorMessage = "Miejscowość jest wymagana")]
        public required string Miejscowosc { get; set; }

        [Required(ErrorMessage = "Kod pocztowy jest wymagany")]
        public required string KodPocztowy { get; set; }

        public string? Ulica { get; set; }

        [Required(ErrorMessage = "Numer posesji jest wymagany")]
        public required string NumerPosesji { get; set; }

        public string? NumerLokalu { get; set; }
    }
}