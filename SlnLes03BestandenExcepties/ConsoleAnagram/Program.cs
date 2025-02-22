using System;
using System.IO;
using System.Linq;

namespace ConsoleAnagram
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Console Anagram");
                Console.WriteLine(new string('=', "Console Anagram".Length));
                Console.WriteLine();

                var woorden = File.ReadAllLines("1000woorden.txt");
                var random = new Random();

                while (true)
                {
                    int aantalLetters;
                    while (true)
                    {
                        Console.Write("Kies het aantal letters (5-15): ");
                        if (int.TryParse(Console.ReadLine(), out aantalLetters) && aantalLetters >= 5 && aantalLetters <= 15)
                        {
                            break;
                        }
                        Console.WriteLine("Ongeldige invoer. Het aantal letters moet tussen 5 en 15 liggen.");
                    }

                    var gefilterdeWoorden = woorden.Where(woord => woord.Length == aantalLetters).ToList();

                    if (gefilterdeWoorden.Count == 0)
                    {
                        Console.WriteLine("Geen woorden gevonden met de opgegeven lengte.");
                        continue;
                    }

                    var gekozenWoord = gefilterdeWoorden[random.Next(gefilterdeWoorden.Count)];
                    var startTijd = DateTime.Now;

                    while (true)
                    {
                        Console.WriteLine();
                        var anagram = new string(gekozenWoord.OrderBy(c => random.Next()).ToArray());
                        Console.WriteLine($"Anagram: {anagram}");

                        Console.Write("Het woord (Druk op ENTER om opnieuw te schudden): ");
                        string gok = Console.ReadLine();

                        if (string.IsNullOrEmpty(gok))
                        {
                            continue;
                        }
                        else if (gok == gekozenWoord)
                        {
                            var eindTijd = DateTime.Now;
                            var tijd = eindTijd - startTijd;
                            string tijdFormaat = $"{tijd.Minutes}m {tijd.Seconds}s {tijd.Milliseconds}ms";
                            Console.WriteLine($"Proficiat! Je hebt het woord geraden in {tijdFormaat}.");
                            return;
                        }
                        else
                        {
                            Console.WriteLine($"Helaas, het woord was '{gekozenWoord}'.");
                            return;
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Het bestand '1000woorden.txt' is niet gevonden.");
            }
            catch (FormatException)
            {
                Console.WriteLine("Ongeldige invoer. Voer een geldig getal in.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Er is een fout opgetreden: {ex.Message}");
            }
        }
    }
}