using System;
using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Web.Models
{
    public class EdytujUzytkownikaViewModel
    {
        public int Id { get; set; }

        [Required]
        public required string Login { get; set; }

        [Required]
        public required string Imie { get; set; }

        [Required]
        public required string Nazwisko { get; set; }

        [Required]
        public required string Pesel { get; set; }

        [Required]
        public DateTime? DataUrodzenia { get; set; }

        [Required]
        public required string Plec { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength]
        public required string Email { get; set; }

        [Required]
        public required string Telefon { get; set; }

        [Required]
        public required string Miejscowosc { get; set; }

        [Required]
        public required string KodPocztowy { get; set; }

        public string? Ulica { get; set; }

        [Required]
        public required string NumerPosesji { get; set; }

        public string? NumerLokalu { get; set; }
    }
}