using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteka.Web.Data.Entities
{
    public class Uzytkownik
    {
        public int Id { get; set; }

        public required string Login { get; set; }

        [Column("haslo_hash")]
        public string? HasloHash { get; set; }

        public required string Imie { get; set; }

        public required string Nazwisko { get; set; }

        public required string Pesel { get; set; }

        [Column("data_urodzenia")]
        public DateTime DataUrodzenia { get; set; }

        public required string Plec { get; set; }

        public required string Email { get; set; }

        public required string Telefon { get; set; }

        public required string Miejscowosc { get; set; }

        [Column("kod_pocztowy")]
        public required string KodPocztowy { get; set; }

        public string? Ulica { get; set; }

        [Column("numer_posesji")]
        public required string NumerPosesji { get; set; }

        public string? NumerLokalu { get; set; }

        [Column("czy_zablokowany")]
        public bool CzyZablokowany { get; set; } = false;

        [Column("blokada_do")]
        public DateTime? BlokadaDo { get; set; }

        [Column("liczba_blednych_logowan")]
        public int LiczbaBlednychLogowan { get; set; } = 0;

        // RODO
        [Column("czy_zapomniany")]
        public bool CzyZapomniany { get; set; } = false;

        [Column("data_zapomnienia")]
        public DateTime? DataZapomnienia { get; set; }

        [Column("zapomniany_przez_id")]
        public int? ZapomnianyPrzezId { get; set; }

        public virtual ICollection<Uprawnienie> Uprawnienia { get; set; } = new List<Uprawnienie>();

        public virtual ICollection<HistoriaHasla> HistoriaHasel { get; set; } = new List<HistoriaHasla>();
    }
}