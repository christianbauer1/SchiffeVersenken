using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SchiffeVersenken //Alle Methoden, die das Spielfeld angehen, werden hier definiert
{
    public class Spielfeld
    {
        private Feld[,] _felder; //Firmenkonvention: Private Klasseninstanzen mit _ !!!

        public Spielfeld(int reihenAnzahl, int spaltenAnzahl) //Konstruktor der Klasse Spielfeld:
        {
            _felder = new Feld[reihenAnzahl, spaltenAnzahl];

            for (int i = 0; i < _felder.GetLength(0); i++)
            {
                for (int u = 0; u < _felder.GetLength(1); u++)
                {
                    _felder[i, u] = new Feld();
                }
            }
        }

        public void PlatziereSchiffeZufaellig(int[] alleSchiffsGroessen)
        {
            var random = new Random();
            Array ausrichtungen = Enum.GetValues(typeof(Schiffsausrichtung)); //Enum.GetValues returns an array with all elements of enum

            foreach (var schiff in alleSchiffsGroessen)
            {
                bool schiffNichtGesetzt = true;
                while (schiffNichtGesetzt)
                {
                    //Versuche das Schiff erfolgreich zu setzen (try). Falls nicht möglich (catch) versuche es nochmal. Solange (while) bis alle Schiffe gesetzt wurden.
                    try
                    {
                        var zufaelligerZeilenIndex = random.Next(_felder.GetLength(0));
                        var zufaelligerSpaltenIndex = random.Next(_felder.GetLength(1));
                        var zufaelligeAusrichtung = (Schiffsausrichtung)ausrichtungen.GetValue(random.Next(ausrichtungen.Length));

                        PlatziereSchiff(schiff, zufaelligeAusrichtung, zufaelligerZeilenIndex, zufaelligerSpaltenIndex);
                        schiffNichtGesetzt = false;
                    }
                    catch //(Exception) //Hier wird wenn throw; aktiv ist, ein Error ausgegeben, den man ggf. noch genauer
                    //definieren könnte
                    {
                        schiffNichtGesetzt = true;
                        //throw; //wird auskommentiert, da throw bedeutet, dass es den Error auswirft
                    }
                }
            }
        }

        public void PlatziereSchiff(int schiffGroesse, Schiffsausrichtung ausrichtung, int reihenIndex, int spaltenIndex)
        {
            var koordinaten = BerechneSchiffskoordinaten(schiffGroesse, ausrichtung, reihenIndex, spaltenIndex);

            if (IsKoordinatenAußerhalbDesSpielfelds(koordinaten))
            {
                throw new Exception("Die Koordinaten liegen außerhalb des Spielfelds.");
            }
            else if (IsSchiffBereitsVorhanden(koordinaten))
            {
                throw new Exception("Auf den gewählten Koordinaten ist bereits ein Schiff vorhanden.");
            }
            else
            {
                foreach (var koordinate in koordinaten)
                {
                    int reihenKoordinate = koordinate.ZeilenKoordinate;
                    int spaltenKoordinate = koordinate.SpaltenKoordinate;
                    _felder[reihenKoordinate, spaltenKoordinate] = new Feld() { IsGesetzt = true };
                }
            }
        }

        private bool IsSchiffBereitsVorhanden(Feldkoordinate[] alleSchiffskoordinaten)
        {
            int reiheAufFeld;
            int spalteAufFeld;
            foreach (var koordinate in alleSchiffskoordinaten)
            {
                reiheAufFeld = koordinate.ZeilenKoordinate;
                spalteAufFeld = koordinate.SpaltenKoordinate;
                if (_felder[reiheAufFeld, spalteAufFeld].IsGesetzt == true)
                {
                    return true;
                }
            }
            return false;
        }

        private Feldkoordinate[] BerechneSchiffskoordinaten(int schiffGroesse, Schiffsausrichtung ausrichtung, int reihenIndex, int spaltenIndex)
        {
            var alleSchiffskoordinaten = new Feldkoordinate[schiffGroesse]; //Instanziere ein neues Array, vom Datentyp Feldkoordinate mit schiffsGroesse Anzahl an Elementen

            for (int i = 0; i < schiffGroesse; i++)
            {
                if (ausrichtung == Schiffsausrichtung.Oben)
                {
                    alleSchiffskoordinaten[i] = new Feldkoordinate() { ZeilenKoordinate = reihenIndex - i, SpaltenKoordinate = spaltenIndex };
                    //An der Stelle i wird jeweils eine neue Feldkoordinate instanziert und mit den jeweiligen Werten gefüllt
                }
                else if (ausrichtung == Schiffsausrichtung.Rechts)
                {
                    alleSchiffskoordinaten[i] = new Feldkoordinate() { ZeilenKoordinate = reihenIndex, SpaltenKoordinate = spaltenIndex + i };
                }
                else if (ausrichtung == Schiffsausrichtung.Unten)
                {
                    alleSchiffskoordinaten[i] = new Feldkoordinate() { ZeilenKoordinate = reihenIndex + i, SpaltenKoordinate = spaltenIndex };
                }
                else //Ausrichtung Links
                {
                    alleSchiffskoordinaten[i] = new Feldkoordinate() { ZeilenKoordinate = reihenIndex, SpaltenKoordinate = spaltenIndex - i };
                }
            }
            return alleSchiffskoordinaten;
        }

        private bool IsKoordinatenAußerhalbDesSpielfelds(Feldkoordinate[] alleKoordinaten)
        {
            for (int i = 0; i < alleKoordinaten.Length; i++)
            {
                bool ueberschreitungOben = alleKoordinaten[i].ZeilenKoordinate < 0;
                bool ueberschreitungRechts = alleKoordinaten[i].SpaltenKoordinate > _felder.GetLength(1) - 1;
                bool ueberschreitungUnten = alleKoordinaten[i].ZeilenKoordinate > _felder.GetLength(0) - 1;
                bool ueberschreitungLinks = alleKoordinaten[i].SpaltenKoordinate < 0;

                if (ueberschreitungOben || ueberschreitungRechts || ueberschreitungUnten || ueberschreitungLinks)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetzeSchuss(int reihenKoordinate, int spaltenKoordinate)
        {
            if (_felder[reihenKoordinate, spaltenKoordinate].IsGesetzt == true)
            {
                _felder[reihenKoordinate, spaltenKoordinate].IsGetroffen = true;
            }
            else
            {
                _felder[reihenKoordinate, spaltenKoordinate].IsGetroffen = false;
            }
        }

        public bool IsJedesSchiffGetroffen()
        {
            //Gute Lösung, da die Methode unabh. von möglicherweise fehlerhaften Inputvariablen ist
            int getroffeneFelder = 0;
            foreach (Feld feld in _felder)
            {
                if (feld.IsGetroffen == true)
                {
                    getroffeneFelder++;
                }
            }
            int gesetzteSchiffe = 0;
            foreach (Feld feld in _felder)
            {
                if (feld.IsGesetzt == true)
                {
                    gesetzteSchiffe++;
                }
            }
            return getroffeneFelder == gesetzteSchiffe;
        }

        public Feld[,] GetFelder()
        {
            return _felder;
        }
    }
}
