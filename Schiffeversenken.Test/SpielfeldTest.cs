using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchiffeVersenken;

namespace Schiffeversenken.Test
{
    [TestClass]
    public class SpielfeldTest
    {
        //TEST DRIVEN DESIGN!
        //Test zur Spielfeldklasse entwickeln (unabh. von Oberfläche)
        //Alternative: Jeden Buttonklick testen (aufwendig)

        /* Allgemeines Vorgehen beim Unit testing: Triple A-Methode
            1. Arrange: Objekt initialisieren var x = new Klasse();
            2. Act: Auf das Objekt agieren = Methode aufrufen: var result = x.MethodeXY();
            3. Assert: Überprüfen/Behaupten, ob die Aktion aus(2) korrekt ist.
            Naming convention: MethodenName_TestSzenario_ErwartetesVerhalten()
         */

        [TestMethod]
        public void PlatziereSchiff_SpielfeldRandUeberschreitung_ThrowException() 
        {
            var spielfeld = new Spielfeld(6,6);

            //1. Test: Wird eine Fehlermeldung ausgeworfen, wenn das Schiff den Spielfeldrand überschreitet?
            //Zur Syntax: https://visualstudiomagazine.com/blogs/tool-tracker/2018/11/test-exceptions.aspx 
            //Jede einzelne Methode muss getrennt auf Fehlermeldung getestet werden!
            Assert.ThrowsException<Exception>(() =>
            {
                spielfeld.PlatziereSchiff(2, Schiffsausrichtung.Oben, 0, 0);
            });

            Assert.ThrowsException<Exception>(() =>
            {
                spielfeld.PlatziereSchiff(2, Schiffsausrichtung.Rechts, 0, 5);
            });

            Assert.ThrowsException<Exception>(() =>
            {
                spielfeld.PlatziereSchiff(2, Schiffsausrichtung.Links, 5, 0);
            });

            Assert.ThrowsException<Exception>(() =>
            {
                spielfeld.PlatziereSchiff(2, Schiffsausrichtung.Unten, 5, 5);
            });

            Assert.ThrowsException<Exception>(Do);
        }

        //LÖSCHEN NACH DEM RECHERCHIEREN:
        public static void Do() //statische Methode, global verfügbar
        {
            var spielfeld = new Spielfeld(6, 6);
            spielfeld.PlatziereSchiff(2, Schiffsausrichtung.Oben, 0, 0);
        }

        //LÖSCHEN NACH DEM RECHERCHIEREN:
        public static void Do2(Action action)
        {
            action();
            //Stichworte: Action (Funktion ohne Parameter) und Func  (Funktion mit Parameter)
        }

        [TestMethod]
        public void PlatziereSchiff_SchiffeUeberkreuzenSich_ThrowException()
        {
            var spieldfeld = new Spielfeld(6, 6);
            spieldfeld.PlatziereSchiff(2, Schiffsausrichtung.Rechts, 0, 0);
            Assert.ThrowsException<Exception>(() =>
            {
                spieldfeld.PlatziereSchiff(2, Schiffsausrichtung.Oben, 1, 1);
            });

        }

        [TestMethod]
        public void PlatziereSchiff_SchiffeKorrektGesetzt_IsTrue()
        {
            var spielfeld = new Spielfeld(6, 6);

            spielfeld.PlatziereSchiff(2, Schiffsausrichtung.Rechts, 0, 0); //Ich setzte das Schiff nach rechts auf Feld 0,0
            var felder = spielfeld.GetFelder(); //Ich speichere mir alle Felder in der Variablen felder
            Assert.IsTrue(felder[0, 0].IsGesetzt); //Ich behaupte, dass das Schiff auf Feld 0,0 gesetzt ist
            Assert.IsTrue(felder[0, 1].IsGesetzt); //Ich behaupte, dass das Schiff auf Feld 0,1 gesetzt ist

            for (int i = 0; i < felder.GetLength(0); i++)
            {
                for (int u = 0; u < felder.GetLength(1); u++)
                {
                    if (u != 0 && u != 1 && i == 0)
                    {
                        //Ich behaupte, dass auf allen Feldern, außer 0,1 und 0,0 das Schiff nicht gesetzt wurde
                        Assert.IsTrue(!felder[i, u].IsGesetzt);
                    }
                }
            }

            //1. Schritt: Vorbedingung z.B. IsKoordinatenAußerhalbDesSpielfelds() --> Innerhalb der Methode selbst abtesten vor der Ausführung
            //2. Schritt: Unit Tests --> z.B. für Core Komponenten ein paar Fälle ausdenken und testen (Test härter gestalten, weil FUnktion oft im Code gebraucht wird)
            //Wenn Tests sich zu oft ändern würde, ist KEIN Test besser!
            //Test sinnvoll: Funktioniert Methode im Standardfall? Gut wenn Methode selbst ihre Vorbed. absichert.
            //Abwägungssache bei Unit tests

            //Ein weiteres Schiff wird auf das gleiche Spielfeld gesetzt
            spielfeld.PlatziereSchiff(4, Schiffsausrichtung.Links, 5, 5);
            Assert.IsTrue(felder[5, 5].IsGesetzt);
            Assert.IsTrue(felder[5, 4].IsGesetzt);
            Assert.IsTrue(felder[5, 3].IsGesetzt);
            Assert.IsTrue(felder[5, 2].IsGesetzt);
        }

        [TestMethod]
        public void PlatziereSchiffeZufaellig_AnzahlGesetzterSchiffe_IsEqual()
        {
            var spielfeld = new Spielfeld(6, 6);

            var alleSchiffsGroessen = new int[] { 2, 3, 4 };
            var summeSchiffe = 0;

            foreach (var schiff in alleSchiffsGroessen)
            {
                summeSchiffe += schiff;
            }

            spielfeld.PlatziereSchiffeZufaellig(alleSchiffsGroessen);

            int alleGesetztenSchiffe = 0;
            //Teste: Sind wirklich alle Schiffe gesetzt worden ODER sind 9 Schiffe auf dem Feld

            //------------------------------------------------------FRAGE: WARUM GEHT FOLGENDES NICHT:
            //foreach (var feld in spielfeld)
            //{
            //    feld.IsGesetzt;
            //}

            var felder = spielfeld.GetFelder(); //FRAGE: ------------------------------------------------------WARUM MUSS ICH HIER GETFELDER AUFRUFEN?
            foreach (var feld in felder)
            {
                if (feld.IsGesetzt)
                {
                    alleGesetztenSchiffe += 1;
                }
            }

            Assert.AreEqual(summeSchiffe, alleGesetztenSchiffe);
        }

        [TestMethod]
        public void SetzeSchuss_IsGetroffen_IsTrue()
        {
            var spielfeld = new Spielfeld(6, 6);
            spielfeld.PlatziereSchiff(2, Schiffsausrichtung.Unten, 0, 0);
            spielfeld.SetzeSchuss(0, 0);


            Assert.IsTrue(spielfeld.GetFelder()[0, 0].IsGetroffen);

        }

        [TestMethod]
        public void IsJedesSchiffGetroffen()
        {
            var spielfeld = new Spielfeld(6, 6);
            spielfeld.PlatziereSchiff(2, Schiffsausrichtung.Unten, 0, 0);
            spielfeld.SetzeSchuss(0, 0);
            spielfeld.SetzeSchuss(1, 0);

            Assert.IsTrue(spielfeld.IsJedesSchiffGetroffen());
        }

    }
}
