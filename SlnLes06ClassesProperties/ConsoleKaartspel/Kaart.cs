using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleKaartspel
{
    public class Kaart
    {
        private int _nummer;
        private char _kleur;

        public int Nummer
        {
            get => _nummer;
            private set
            {
                if (value < 1 || value > 13)
                    throw new ArgumentException("Kaartnummer moet tussen 1 en 13 liggen");
                _nummer = value;
            }
        }

        public char Kleur
        {
            get => _kleur;
            private set
            {
                if (value != 'C' && value != 'S' && value != 'H' && value != 'D')
                    throw new ArgumentException("Ongeldige kleur. Geldige kleuren: C, S, H, D");
                _kleur = value;
            }
        }

        public Kaart(int nummer, char kleur)
        {
            Nummer = nummer;
            Kleur = kleur;
        }
    }
}
