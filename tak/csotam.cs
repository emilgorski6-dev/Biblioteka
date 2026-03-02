using System;

namespace Biblioteka.Example
{
    // Klasa programu
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Witaj w przykładowym programie C#!");

            var książka = new Książka
            {
                Tytuł = "Pan Tadeusz",
                Autor = "Adam Mickiewicz",
                RokWydania = 1834
            };

            Console.WriteLine(książka);
        }
    }

    // Prosty model reprezentujący książkę
    public class Książka
    {
        public string Tytuł { get; set; }
        public string Autor { get; set; }
        public int RokWydania { get; set; }

        public override string ToString()
            => $"{Tytuł} – {Autor} ({RokWydania})";
    }
}