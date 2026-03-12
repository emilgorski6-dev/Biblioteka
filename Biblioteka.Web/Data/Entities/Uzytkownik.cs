using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteka.Web.Data.Entities
{
    
public class Uzytkownik
    {
        public int Id { get; set; }

        public string Login { get; set; }

        [Column("haslo_hash")]
        public string? HasloHash { get; set; }

        public string Imie { get; set; }

        public string Nazwisko { get; set; }

        public string Pesel { get; set; }

        [Column("data_urodzenia")]
        public DateTime DataUrodzenia { get; set; }

        public string Plec { get; set; }

        public string Email { get; set; }

        public string Telefon { get; set; }

        // Adres
        public string Miejscowosc { get; set; }

        [Column("kod_pocztowy")]
        public string KodPocztowy { get; set; }

        public string Ulica { get; set; }

        [Column("numer_posesji")]
        public string NumerPosesji { get; set; }

        public string NumerLokalu { get; set; }

        // Bezpieczeństwo
        [Column("czy_zablokowany")]
        public bool CzyZablokowany { get; set; }

        [Column("blokada_do")]
        public DateTime? BlokadaDo { get; set; }

        [Column("liczba_blednych_logowan")]
        public int LiczbaBlednychLogowan { get; set; }

        // RODO
        [Column("czy_zapomniany")]
        public bool CzyZapomniany { get; set; }

        [Column("data_zapomnienia")]
        public DateTime? DataZapomnienia { get; set; }

        [Column("zapomniany_przez_id")]
        public int? ZapomnianyPrzezId { get; set; }
    }

}