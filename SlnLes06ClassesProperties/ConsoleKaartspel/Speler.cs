using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleKaartspel
{
    public class Speler
    {
        public string Naam { get; private set; }
        public List<Kaart> Kaarten { get; private set; }
        public bool HeeftNogKaarten => Kaarten.Count > 0;

        public Speler(string naam)
        {
            Naam = naam;
            Kaarten = new List<Kaart>();
        }

        public Speler(string naam, List<Kaart> kaarten)
        {
            Naam = naam;
            Kaarten = new List<Kaart>(kaarten);
        }

        public Kaart LegKaart()
        {
            if (!HeeftNogKaarten)
                throw new InvalidOperationException("Speler heeft geen kaarten meer");

            Random rnd = new Random();
            int index = rnd.Next(Kaarten.Count);
            Kaart kaart = Kaarten[index];
            Kaarten.RemoveAt(index);
            return kaart;
        }
    }
}
