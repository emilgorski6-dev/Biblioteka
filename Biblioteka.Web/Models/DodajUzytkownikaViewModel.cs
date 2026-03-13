using System;
using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Web.Models
{
    public class DodajUzytkownikaViewModel
    {
        [Required(ErrorMessage = "Pole 'Login' jest wymagane.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Pole 'Adres e-mail' jest wymagane.")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu e-mail.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Pole 'Numer telefonu' jest wymagane.")]
        public string Telefon { get; set; }

        [Required(ErrorMessage = "Pole 'Imię' jest wymagane.")]
        public string Imie { get; set; }

        [Required(ErrorMessage = "Pole 'Nazwisko' jest wymagane.")]
        public string Nazwisko { get; set; }

        [Required(ErrorMessage = "Pole 'Numer PESEL' jest wymagane.")]
        public string Pesel { get; set; }

        [Required(ErrorMessage = "Pole 'Data urodzenia' jest wymagane.")]
        public DateTime? DataUrodzenia { get; set; }

        [Required(ErrorMessage = "Pole 'Płeć' jest wymagane.")]
        public string Plec { get; set; }

        [Required(ErrorMessage = "Pole 'Miejscowość' jest wymagane.")]
        public string Miejscowosc { get; set; }

        [Required(ErrorMessage = "Pole 'Kod pocztowy' jest wymagane.")]
        public string KodPocztowy { get; set; }

        public string? Ulica { get; set; }

        [Required(ErrorMessage = "Pole 'Nr posesji' jest wymagane.")]
        public string NumerPosesji { get; set; }

        public string? NumerLokalu { get; set; }
    }
}