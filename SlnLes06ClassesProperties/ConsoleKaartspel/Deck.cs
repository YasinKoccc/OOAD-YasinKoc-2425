using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleKaartspel
{
    public class Deck
    {
        public List<Kaart> Kaarten { get; private set; }

        public Deck()
        {
            Kaarten = new List<Kaart>();
            char[] kleuren = { 'C', 'S', 'H', 'D' };
            foreach (char kleur in kleuren)
            {
                for (int nummer = 1; nummer <= 13; nummer++)
                {
                    Kaarten.Add(new Kaart(nummer, kleur));
                }
            }
        }

        public void Schudden()
        {
            Random rng = new Random();
            int n = Kaarten.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Kaart value = Kaarten[k];
                Kaarten[k] = Kaarten[n];
                Kaarten[n] = value;
            }
        }

        public Kaart NeemKaart()
        {
            if (Kaarten.Count == 0)
                throw new InvalidOperationException("Er zijn geen kaarten meer in het deck");

            Kaart kaart = Kaarten[0];
            Kaarten.RemoveAt(0);
            return kaart;
        }
    }
}
