using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken
{
    public class Schiffsammlung
    {
        private int[] _schiffssammlung;

        public Schiffsammlung(int anzahlSchiffe)
        {
            _schiffssammlung = new int[anzahlSchiffe];
            for (int i = 0; i < anzahlSchiffe; i++)
            {
                _schiffssammlung[i] = i+2;
            }
        }

        public int[] GetSchiffsammlung()
        {
            return _schiffssammlung;
        }
    }
}
