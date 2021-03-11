using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Zusätzliche Imports
using System.Diagnostics;
using System.Linq;

namespace SchiffeVersenken
{
    public partial class SchiffeVersenken : Form
    {
        Button[,] spielerFeldButtons;
        Button[,] gegnerFeldButtons;
        Button gewaehlterButton;

        List<Button> buttonsGegnerischeIntelligenz;

        Spielfeld spielerSpielfeld;
        Spielfeld gegnerSpielfeld;
        int[] verfuegbareSchiffe;

        public SchiffeVersenken()
        {
            InitializeComponent();
        }

        private void starteSpielToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitialisiereSpiel();
        }

        private void buttonSetzeSchiff_Click(object sender, EventArgs e)
        {
            //Sammle alle notwendigen Eigabeaaten
            int schiffsGroesse = int.Parse(textBoxSchiffswahl.Text);
            var ausrichtung = (Schiffsausrichtung)Enum.Parse(typeof(Schiffsausrichtung), comboBoxSchiffsAusrichtung.Text);
            var gewaehlteKoordinaten = WandleButtonInKoordinatenUm(gewaehlterButton, spielerFeldButtons, gegnerFeldButtons);

            try
            {
                if (!IsSchiffInAuswahlMoeglichkeiten(schiffsGroesse, verfuegbareSchiffe))
                {
                    throw new Exception("Bitte wähle ein verfügbares Schiff.");
                }
                spielerSpielfeld.PlatziereSchiff(schiffsGroesse, ausrichtung, gewaehlteKoordinaten.ZeilenKoordinate, gewaehlteKoordinaten.SpaltenKoordinate);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Platzieren nicht möglich: " + ex.Message); //fängt die exception (aus Spielfeld Klasse)
                return; //verlässt die void methode an dieser stelle
            }

            UpdateViewGesetzteSchiffe(spielerFeldButtons, spielerSpielfeld);

            //Entferne Schiff Aus Auswahlmöglichkeiten
            verfuegbareSchiffe = verfuegbareSchiffe.Where(val => val != schiffsGroesse).ToArray();
            AktualisiereAnsichtVerfuegbarerSchiffe(verfuegbareSchiffe);

            //Beende die Möglichkeit Schiffe zu setzen, sobald alle möglichen Schiffe gesetzte wurden
            if (verfuegbareSchiffe == null || verfuegbareSchiffe.Length == 0)
            {
                groupBoxAnweisungen.Visible = false;
                attackButton.Visible = true;
                attackButton.Enabled = true;
                foreach (var button in spielerFeldButtons)
                {
                    button.Enabled = false;
                }
            }
        }

        private bool IsSchiffInAuswahlMoeglichkeiten(int schiffsGroesse, int[] verfuegbareSchiffe)
        {
            foreach (var schiff in verfuegbareSchiffe)
            {
                if (schiff == schiffsGroesse)
                {
                    return true;
                }
            }
            return false;
        }

        private void attackButton_Click(object sender, EventArgs e)
        {
            var attackeKoordinaten = WandleButtonInKoordinatenUm(gewaehlterButton, spielerFeldButtons, gegnerFeldButtons);
            gegnerSpielfeld.SetzeSchuss(attackeKoordinaten.ZeilenKoordinate, attackeKoordinaten.SpaltenKoordinate);
            UpdateViewZuletztGesetztesFeld(attackeKoordinaten.ZeilenKoordinate, attackeKoordinaten.SpaltenKoordinate, gegnerFeldButtons, gegnerSpielfeld);
            attackButton.Enabled = false;
            if (gegnerSpielfeld.IsJedesSchiffGetroffen())
            {
                GameOverMeldung();
            }
            else
            {
                GegnerEvent(sender, e);
            }
        }

        private void GegnerEvent(object sender, EventArgs e)
        {
            var zufallsKoordinaten = ErzeugeZufaelligeSchussKoordianten(buttonsGegnerischeIntelligenz);
            spielerSpielfeld.SetzeSchuss(zufallsKoordinaten.ZeilenKoordinate, zufallsKoordinaten.SpaltenKoordinate);
            UpdateViewZuletztGesetztesFeld(zufallsKoordinaten.ZeilenKoordinate, zufallsKoordinaten.SpaltenKoordinate, spielerFeldButtons, spielerSpielfeld);
            attackButton.Enabled = true;

            if (spielerSpielfeld.IsJedesSchiffGetroffen())
            {
                GameOverMeldung();
                InitialisiereSpiel();
            }
        }

        public Feldkoordinate ErzeugeZufaelligeSchussKoordianten(List<Button> alleVerfuegbarenButtons)
        {
            //KI schießt ausschließlich auf Felder, auf die noch kein Schuss gesetzt wurde
            var random = new Random();
            Button gewaehlterZufallsButton = alleVerfuegbarenButtons[random.Next(alleVerfuegbarenButtons.Count())];
            alleVerfuegbarenButtons.Remove(gewaehlterZufallsButton);
            return WandleButtonInKoordinatenUm(gewaehlterZufallsButton, spielerFeldButtons, gegnerFeldButtons);
        }

        private void GameOverMeldung()
        {
            if (spielerSpielfeld.IsJedesSchiffGetroffen())
            {
                MessageBox.Show("Der Computer hat das Spiel gewonnen!", "Verloren");
            }
            else
            {
                MessageBox.Show("Du hast das Spiel gewonnen!", "Gewonnen");
            }
        }

        private bool IsGewaehlterButtonVorhanden(Button buttonWahl, Button[,] alleButtons)
        {
            foreach (var button in alleButtons)
            {
                if (button == buttonWahl)
                {
                    return true;
                }
            }
            return false;
        }

        private Feldkoordinate ErmittleKoordinatenDesButtons(Button buttonWahl, Button[,] alleButtons)
        {
            var koordinaten = new Feldkoordinate();
            for (int i = 0; i < alleButtons.GetLength(0); i++)
            {
                for (int u = 0; u < alleButtons.GetLength(1); u++)
                {
                    if (buttonWahl == alleButtons[i, u])
                    {
                        koordinaten.ZeilenKoordinate = i;
                        koordinaten.SpaltenKoordinate = u;
                    }
                }
            }
            return koordinaten;
        }

        private Feldkoordinate WandleButtonInKoordinatenUm(Button buttonWahl, Button[,] alleSpielerButtons, Button[,] alleGegnerButtons)
        {
            if (IsGewaehlterButtonVorhanden(buttonWahl, alleSpielerButtons))
            {
                return ErmittleKoordinatenDesButtons(buttonWahl, alleSpielerButtons);
            }
            else
            {
                return ErmittleKoordinatenDesButtons(buttonWahl, alleGegnerButtons);
            }
        }

        private void spielfeldButtonKlickEvent(object sender, EventArgs e)
        {
            gewaehlterButton = (Button)sender;
            txtGewaehltesFeld.Text = gewaehlterButton.Text;
        }

        private void InitialisiereSpiel()
        {
            //Daten aus Model holen und neue Felder initialisieren
            spielerSpielfeld = new Spielfeld(6, 6);
            gegnerSpielfeld = new Spielfeld(6, 6);
            verfuegbareSchiffe = new Schiffsammlung(3).GetSchiffsammlung();

            //Buttons sammeln
            spielerFeldButtons = new Button[,] {
                { u1, u2, u3, u4, u5, u6 },
                { v1, v2, v3, v4, v5, v6 },
                { w1, w2, w3, w4, w5, w6 },
                { x1, x2, x3, x4, x5, x6 },
                { y1, y2, y3, y4, y5, y6 },
                { z1, z2, z3, z4, z5, z6 }
            };

            gegnerFeldButtons = new Button[,]
            {
                { a1, a2, a3, a4, a5, a6 },
                { b1, b2, b3, b4, b5, b6 },
                { c1, c2, c3, c4, c5, c6 },
                { d1, d2, d3, d4, d5, d6 },
                { e1, e2, e3, e4, e5, e6 },
                { f1, f2, f3, f4, f5, f6 }
            };

            //Erstelle eine Liste mit Buttons für die gegnerische Intelligenz
            buttonsGegnerischeIntelligenz = new List<Button>();
            foreach (var button in spielerFeldButtons)
            {
                buttonsGegnerischeIntelligenz.Add(button);
            }

            //GUI Anweisungen erscheinen
            groupBoxAnweisungen.BringToFront();
            groupBoxAnweisungen.Visible = true;
            AktualisiereAnsichtVerfuegbarerSchiffe(verfuegbareSchiffe);

            Array ausrichtungen = Enum.GetValues(typeof(Schiffsausrichtung));

            comboBoxSchiffsAusrichtung.Items.Clear();
            foreach (var ausrichtung in ausrichtungen)
            {
                comboBoxSchiffsAusrichtung.Items.Add(ausrichtung);
            }

            //Start disabling
            starteSpielToolStripMenuItem.Enabled = false;

            //Enable alle Spielerfelder
            foreach (var button in spielerFeldButtons)
            {
                button.Enabled = true;
            }

            //Attack button verstecken
            attackButton.Visible = false;

            //UpdateView
            UpdateSpielfeldViewNeuesSpiel(spielerFeldButtons);
            UpdateSpielfeldViewNeuesSpiel(gegnerFeldButtons);

            //Init Button
            gewaehlterButton = u1;

            //Platzierung hier notwendig
            gegnerSpielfeld.PlatziereSchiffeZufaellig(verfuegbareSchiffe);
        }

        private void UpdateSpielfeldViewNeuesSpiel(Button[,] alleButtons)
        {
            foreach (var button in alleButtons)
            {
                button.BackColor = Color.White;
                button.BackgroundImage = null;
            }
        }

        private void UpdateViewZuletztGesetztesFeld(int zeilenAuswahl, int spaltenAuswahl, Button[,] alleButtons, Spielfeld spielfeld)
        {
            var aktuelleView = spielfeld.GetFelder();
            if (aktuelleView[zeilenAuswahl, spaltenAuswahl].IsGetroffen)
            {
                alleButtons[zeilenAuswahl, spaltenAuswahl].BackgroundImage = Properties.Resources.fireIcon;
            }
            else
            {
                alleButtons[zeilenAuswahl, spaltenAuswahl].BackgroundImage = Properties.Resources.missIcon;
            }
        }

        private void UpdateViewGesetzteSchiffe(Button[,] alleButtons, Spielfeld spielfeld)
        {
            var aktuelleView = spielfeld.GetFelder();
            for (int i = 0; i < aktuelleView.GetLength(0); i++)
            {
                for (int u = 0; u < aktuelleView.GetLength(1); u++)
                {
                    if (aktuelleView[i, u].IsGesetzt)
                    {
                        alleButtons[i, u].BackColor = Color.Orange;
                    }
                    if (aktuelleView[i, u].IsGesetzt == false)
                    {
                        alleButtons[i, u].BackColor = Color.White;
                    }
                }
            }
        }

        private void AktualisiereAnsichtVerfuegbarerSchiffe(int[] verfuegbareSchiffe)
        {
            string displaySchiffe = "";
            foreach (var schiff in verfuegbareSchiffe)
            {
                displaySchiffe += schiff + "   ";
            }
            txtVerfuegbareSchiffe.Text = displaySchiffe;
        }

        private void ueberSchiffeversenkenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Entwickelt von Christian Bauer\n© IngSoft GmbH 2021", "Über Schiffeversenken");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void schwierigkeitsgradToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Diese Funktion ist noch nicht verfügbar", "Information");
        }

        private void neustartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitialisiereSpiel();
        }


    }
}
