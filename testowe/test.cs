using System;
using System.Collections.Generic;

namespace PrzykladProgramu
{
    // Klasa reprezentująca pojedyncze zadanie
    public class Zadanie
    {
        public string Nazwa { get; set; }
        public bool CzyWykonane { get; set; }

        public Zadanie(string nazwa)
        {
            Nazwa = nazwa;
            CzyWykonane = false;
        }

        public void OznaczJakoZrobione()
        {
            CzyWykonane = true;
            Console.WriteLine($"Sukces! Zadanie '{Nazwa}' zostało ukończone.");
        }
    }

    // Główna klasa programu
    class Program
    {
        static void Main(string[] args)
        {
            // Tworzymy listę obiektów typu Zadanie
            List<Zadanie> listaZadan = new List<Zadanie>();

            // Dodajemy nowe elementy
            listaZadan.Add(new Zadanie("Kupić kawę"));
            listaZadan.Add(new Zadanie("Nauczyć się C#"));
            listaZadan.Add(new Zadanie("Umyć samochód"));

            // Logika programu
            Console.WriteLine("--- TWOJA LISTA ZADAŃ ---");
            
            // Pętla wyświetlająca zadania
            foreach (var z in listaZadan)
            {
                string status = z.CzyWykonane ? "[X]" : "[ ]";
                Console.WriteLine($"{status} {z.Nazwa}");
            }

            Console.WriteLine("\nAktualizacja statusu...");
            listaZadan[1].OznaczJakoZrobione(); // Wykonujemy drugie zadanie na liście

            Console.WriteLine("\nNaciśnij dowolny klawisz, aby zakończyć.");
            Console.ReadKey();
        }
    }
}