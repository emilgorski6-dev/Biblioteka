using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteka.Web.Data.Entities
{
    [Table("Uzytkownicy")]
    public class Uzytkownik
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // --- DANE KONTA ---
        [Required]
        [StringLength(20)]
        [Column("login")]
        public required string Login { get; set; }

        [Column("haslo_hash")]
        public string? HasloHash { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        [Column("email")]
        public required string Email { get; set; }

        [Required]
        [StringLength(9)]
        [Column("telefon")]
        public required string Telefon { get; set; }

        // --- DANE OSOBOWE ---
        [Required]
        [StringLength(50)]
        [Column("imie")]
        public required string Imie { get; set; }

        [Required]
        [StringLength(50)]
        [Column("nazwisko")]
        public required string Nazwisko { get; set; }

        [Required]
        [StringLength(11)]
        [Column("pesel")]
        public required string Pesel { get; set; }

        [Column("data_urodzenia")]
        public DateTime DataUrodzenia { get; set; }

        [Column("plec")] // Dodano brakujący atrybut
        public TypPlci Plec { get; set; }

        // --- ADRES ZAMIESZKANIA ---
        [Required]
        [StringLength(100)]
        [Column("miejscowosc")]
        public required string Miejscowosc { get; set; }

        [Required]
        [StringLength(6)]
        [Column("kod_pocztowy")]
        public required string KodPocztowy { get; set; }

        [StringLength(100)]
        [Column("ulica")] // Dodano brakujący atrybut
        public string? Ulica { get; set; }

        [Required]
        [StringLength(10)]
        [Column("numer_posesji")]
        public required string NumerPosesji { get; set; }

        [StringLength(10)]
        [Column("numer_lokalu")] // Dodano brakujący atrybut
        public string? NumerLokalu { get; set; }

        // --- STATUS I BLOKADY ---
        [Column("czy_zablokowany")]
        public bool CzyZablokowany { get; set; } = false;

        [Column("blokada_do")]
        public DateTime? BlokadaDo { get; set; }

        [Column("liczba_blednych_logowan")]
        public int LiczbaBlednychLogowan { get; set; } = 0;

        [Column("czy_haslo_tymczasowe")]
        public bool CzyHasloTymczasowe { get; set; } = false;

        // --- RODO ---
        [Column("czy_zapomniany")]
        public bool CzyZapomniany { get; set; } = false;

        [Column("data_zapomnienia")]
        public DateTime? DataZapomnienia { get; set; }

        [Column("zapomniany_przez_id")]
        public int? ZapomnianyPrzezId { get; set; }

        // --- RELACJE ---
        // virtual umożliwia tzw. Lazy Loading, co jest dobrą praktyką w EF Core
        public virtual ICollection<Uprawnienie> Uprawnienia { get; set; } = new List<Uprawnienie>();
        public virtual ICollection<HistoriaHasla> HistoriaHasel { get; set; } = new List<HistoriaHasla>();
    }
}