using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        static Stopwatch stopwatch;

        private Entry[,] field;
        //private Hashtable ht = new Hashtable();

        public Page1()
        {
            
            InitializeComponent();
            //Stopwatch Funktion
            stopwatch = new Stopwatch();
            lblStopwatch.Text = "00:00:00.00";

            //Feld mit den Eingabewerten
            field = new Entry[9, 9] {{ E00, E01, E02, E03, E04, E05, E06, E07, E08},
                                     { E10, E11, E12, E13, E14, E15, E16, E17, E18},
                                     { E20, E21, E22, E23, E24, E25, E26, E27, E28},
                                     { E30, E31, E32, E33, E34, E35, E36, E37, E38},
                                     { E40, E41, E42, E43, E44, E45, E46, E47, E48},
                                     { E50, E51, E52, E53, E54, E55, E56, E57, E58},
                                     { E60, E61, E62, E63, E64, E65, E66, E67, E68},
                                     { E70, E71, E72, E73, E74, E75, E76, E77, E78},
                                     { E80, E81, E82, E83, E84, E85, E86, E87, E88}};

           /*if (paused)
              this.Appearing += ToolbarItemActivatedResume;
            protected void override OnResume ()
            {}
            public void override OnBackPressed ()
            {}
              */

            /*Hashtable initialisieren
            for (int i = 1; i < 9; i++)
            {
                ht.Add(i, new List<Tuple<int, int>>());
            }

            List<Tuple<int, int>> list = new List<Tuple<int, int>>();
            Tuple<int, int> t = new Tuple<int, int>(1,2);
            list.Add(Tuple < 2, 3 >);*/
        }

        //Löscht alle Einträge im Feld
        private void EmtpyField()
        {
            int i = 0; int j = 0;

            foreach (Entry entry in field)
            {
                if (j == 9)
                {
                    j = 0;
                    i++;
                }

                entry.Text = string.Empty;
                entry.IsReadOnly = false;
                j++;

            }
        }

        //Generiert das Sudoku
        private void GenerateField(object sender, EventArgs e)
        {

            BackgroundColor = Color.White;
            EmtpyField();

            Sudoku sud = new Sudoku();
            sud.Generate();
            sud.ClearRandomFields(20);

            string[,] gerneratedField = Sudoku.generatedField(sud);

            int i = 0; int j = 0;

            foreach (Entry entry in field)
            {
                if (j == 9)
                {
                    j = 0;
                    i++;
                }
                if (gerneratedField[i, j].Equals("0"))
                {
                    j++;
                    continue;
                }
                entry.Text = gerneratedField[i, j];
                entry.IsReadOnly = true;

                j++;
            }

            if (!stopwatch.IsRunning)
            {
                stopwatch.Restart();
                Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
                {
                    lblStopwatch.Text = stopwatch.Elapsed.ToString();
                    if (!stopwatch.IsRunning)
                        return false;
                    else
                        return true;
                });
            }
            else
                stopwatch.Restart();
        }

        //Funktion für Resum
        private void ToolbarItemActivatedResume(object sender, EventArgs e)
        {
            foreach (Entry entry in field)
            {
                entry.TextColor = Color.Black;
                entry.IsReadOnly = false;
            }


            if (!stopwatch.IsRunning)
            {
                stopwatch.Start();
                Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
                {
                    lblStopwatch.Text = stopwatch.Elapsed.ToString();
                    TimeSpan Time = stopwatch.Elapsed;
                    if (!stopwatch.IsRunning)
                        return false;
                    else
                        return true;
                });
            }

        }


        //Funktion für Stoppen
        public void ToolbarItemActivatedPause(object sender, EventArgs e)
        {
            stopwatch.Stop();
            //paused = true;
            foreach (Entry entry in field)
            {
                entry.TextColor = Color.White;
                entry.IsReadOnly = true;

               //Test HighscorePage
               /*lblStopwatch.Text = stopwatch.Elapsed.ToString();
               Navigation.PushAsync(new HighscorePage(lblStopwatch.Text));*/

            }


        }

        //Funktion fürs Überprüfen (wenn "Feld überprüfen" - Button gedrückt wird)
        private void CheckField(object sender, EventArgs e)
        {
            stopwatch.Stop();
            Sudoku sud = new Sudoku();

            int x = 0; int y = 0;

            foreach (Entry entry in field)
            {
                if (y == 9)
                {
                    y = 0;
                    x++;
                }

                int number = 0;

                string s = entry.Text;

                bool result = int.TryParse(s, out number);

                if (entry.Text == string.Empty || !result)
                {
                    entry.Text = null;
                    entry.IsReadOnly = false;
                }

                sud[x, y] = Convert.ToInt32(entry.Text);
                y++;
            }

            string[,] gerneratedField = Sudoku.generatedField(sud);

            int i = 0; int j = 0;

            foreach (Entry entry in field)
            {
                if (j == 9)
                {
                    j = 0;
                    i++;
                }
                if (gerneratedField[i, j].Equals("0"))
                {
                    j++;
                    continue;
                }
                entry.Text = gerneratedField[i, j];

                j++;
            }

            bool isCorrect;

            isCorrect = sud.IsCorrect();

            if (isCorrect)
            {
                BackgroundColor = Color.Green;

                lblStopwatch.Text = stopwatch.Elapsed.ToString();
                Navigation.PushAsync(new HighscorePage(lblStopwatch.Text));
            }
            else
                BackgroundColor = Color.Red;

            foreach (Entry entry in field)
            {
                if (entry.Text == string.Empty)
                {
                    entry.Text = null;
                    entry.IsReadOnly = false;
                }

            }


        }


        //Falls ein Bestimmtes Feld Fokussiert ist
        private new void Focused(object sender, FocusEventArgs e)
        {
            //Färbe den selben Quadranten
            Quadrant(e.VisualElement, Color.LightYellow);
            //Färbe die selbe Zeile
            Zeile(e.VisualElement, Color.Beige);
            //Färbe die selbe Spalte
            Spalte(e.VisualElement, Color.Beige);
            //Fokussierte Zelle wird nochmal anders gefärbt
            e.VisualElement.BackgroundColor = Color.Yellow;
        }

        private new void Unfocused(object sender, FocusEventArgs e)
        {
            //Falls Zelle nicht mehr fokussiert ist, Färbe alles wieder zurück
            Color standardColor = Color.FloralWhite;
            Quadrant(e.VisualElement, standardColor);
            Zeile(e.VisualElement, standardColor);
            Spalte(e.VisualElement, standardColor);
        }

        private void Quadrant(VisualElement visualElement, Color color)
        {
            if (visualElement.Id == E00.Id || visualElement.Id == E01.Id || visualElement.Id == E02.Id || visualElement.Id == E10.Id || visualElement.Id == E11.Id || visualElement.Id == E12.Id || visualElement.Id == E20.Id || visualElement.Id == E21.Id || visualElement.Id == E22.Id)
            {
                //erste Zeile des Quadranten
                E00.BackgroundColor = color;
                E01.BackgroundColor = color;
                E02.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E10.BackgroundColor = color;
                E11.BackgroundColor = color;
                E12.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E20.BackgroundColor = color;
                E21.BackgroundColor = color;
                E22.BackgroundColor = color;

            }
            else if (visualElement.Id == E30.Id || visualElement.Id == E31.Id || visualElement.Id == E32.Id || visualElement.Id == E40.Id || visualElement.Id == E41.Id || visualElement.Id == E42.Id || visualElement.Id == E50.Id || visualElement.Id == E51.Id || visualElement.Id == E52.Id)
            {
                //erste Zeile des Quadranten
                E30.BackgroundColor = color;
                E31.BackgroundColor = color;
                E32.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E40.BackgroundColor = color;
                E41.BackgroundColor = color;
                E42.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E50.BackgroundColor = color;
                E51.BackgroundColor = color;
                E52.BackgroundColor = color;
            }
            else if (visualElement.Id == E60.Id || visualElement.Id == E61.Id || visualElement.Id == E62.Id || visualElement.Id == E70.Id || visualElement.Id == E71.Id || visualElement.Id == E72.Id || visualElement.Id == E80.Id || visualElement.Id == E81.Id || visualElement.Id == E82.Id)
            {
                //erste Zeile des Quadranten
                E60.BackgroundColor = color;
                E61.BackgroundColor = color;
                E62.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E70.BackgroundColor = color;
                E71.BackgroundColor = color;
                E72.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E80.BackgroundColor = color;
                E81.BackgroundColor = color;
                E82.BackgroundColor = color;
            }
            else if (visualElement.Id == E03.Id || visualElement.Id == E04.Id || visualElement.Id == E05.Id || visualElement.Id == E13.Id || visualElement.Id == E14.Id || visualElement.Id == E15.Id || visualElement.Id == E23.Id || visualElement.Id == E24.Id || visualElement.Id == E25.Id)
            {
                //erste Zeile des Quadranten
                E03.BackgroundColor = color;
                E04.BackgroundColor = color;
                E05.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E13.BackgroundColor = color;
                E14.BackgroundColor = color;
                E15.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E23.BackgroundColor = color;
                E24.BackgroundColor = color;
                E25.BackgroundColor = color;
            }
            else if (visualElement.Id == E33.Id || visualElement.Id == E34.Id || visualElement.Id == E35.Id || visualElement.Id == E43.Id || visualElement.Id == E44.Id || visualElement.Id == E45.Id || visualElement.Id == E53.Id || visualElement.Id == E54.Id || visualElement.Id == E55.Id)
            {
                //erste Zeile des Quadranten
                E33.BackgroundColor = color;
                E34.BackgroundColor = color;
                E35.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E43.BackgroundColor = color;
                E44.BackgroundColor = color;
                E45.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E53.BackgroundColor = color;
                E54.BackgroundColor = color;
                E55.BackgroundColor = color;
            }
            else if (visualElement.Id == E63.Id || visualElement.Id == E64.Id || visualElement.Id == E65.Id || visualElement.Id == E73.Id || visualElement.Id == E74.Id || visualElement.Id == E75.Id || visualElement.Id == E83.Id || visualElement.Id == E84.Id || visualElement.Id == E85.Id)
            {
                //erste Zeile des Quadranten
                E63.BackgroundColor = color;
                E64.BackgroundColor = color;
                E65.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E73.BackgroundColor = color;
                E74.BackgroundColor = color;
                E75.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E83.BackgroundColor = color;
                E84.BackgroundColor = color;
                E85.BackgroundColor = color;
            }
            else if (visualElement.Id == E06.Id || visualElement.Id == E07.Id || visualElement.Id == E08.Id || visualElement.Id == E16.Id || visualElement.Id == E17.Id || visualElement.Id == E18.Id || visualElement.Id == E26.Id || visualElement.Id == E27.Id || visualElement.Id == E28.Id)
            {
                //erste Zeile des Quadranten
                E06.BackgroundColor = color;
                E07.BackgroundColor = color;
                E08.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E16.BackgroundColor = color;
                E17.BackgroundColor = color;
                E18.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E26.BackgroundColor = color;
                E27.BackgroundColor = color;
                E28.BackgroundColor = color;
            }
            else if (visualElement.Id == E36.Id || visualElement.Id == E37.Id || visualElement.Id == E38.Id || visualElement.Id == E46.Id || visualElement.Id == E47.Id || visualElement.Id == E48.Id || visualElement.Id == E56.Id || visualElement.Id == E57.Id || visualElement.Id == E58.Id)
            {
                //erste Zeile des Quadranten
                E36.BackgroundColor = color;
                E37.BackgroundColor = color;
                E38.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E46.BackgroundColor = color;
                E47.BackgroundColor = color;
                E48.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E56.BackgroundColor = color;
                E57.BackgroundColor = color;
                E58.BackgroundColor = color;
            }
            else if (visualElement.Id == E66.Id || visualElement.Id == E67.Id || visualElement.Id == E68.Id || visualElement.Id == E76.Id || visualElement.Id == E77.Id || visualElement.Id == E78.Id || visualElement.Id == E86.Id || visualElement.Id == E87.Id || visualElement.Id == E88.Id)
            {
                //erste Zeile des Quadranten
                E66.BackgroundColor = color;
                E67.BackgroundColor = color;
                E68.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E76.BackgroundColor = color;
                E77.BackgroundColor = color;
                E78.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E86.BackgroundColor = color;
                E87.BackgroundColor = color;
                E88.BackgroundColor = color;
            }
        }

        private void Zeile(VisualElement visualElement, Color color)
        {
            if (visualElement.Id == E00.Id || visualElement.Id == E01.Id || visualElement.Id == E02.Id || visualElement.Id == E03.Id || visualElement.Id == E04.Id || visualElement.Id == E05.Id || visualElement.Id == E06.Id || visualElement.Id == E07.Id || visualElement.Id == E08.Id)
            {
                //erste Zeile des Quadranten
                E00.BackgroundColor = color;
                E01.BackgroundColor = color;
                E02.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E03.BackgroundColor = color;
                E04.BackgroundColor = color;
                E05.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E06.BackgroundColor = color;
                E07.BackgroundColor = color;
                E08.BackgroundColor = color;

            }
            else if (visualElement.Id == E10.Id || visualElement.Id == E11.Id || visualElement.Id == E12.Id || visualElement.Id == E13.Id || visualElement.Id == E14.Id || visualElement.Id == E15.Id || visualElement.Id == E16.Id || visualElement.Id == E17.Id || visualElement.Id == E18.Id)
            {
                //erste Zeile des Quadranten
                E10.BackgroundColor = color;
                E11.BackgroundColor = color;
                E12.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E13.BackgroundColor = color;
                E14.BackgroundColor = color;
                E15.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E16.BackgroundColor = color;
                E17.BackgroundColor = color;
                E18.BackgroundColor = color;
            }
            else if (visualElement.Id == E20.Id || visualElement.Id == E21.Id || visualElement.Id == E22.Id || visualElement.Id == E23.Id || visualElement.Id == E24.Id || visualElement.Id == E25.Id || visualElement.Id == E26.Id || visualElement.Id == E27.Id || visualElement.Id == E28.Id)
            {
                //erste Zeile des Quadranten
                E20.BackgroundColor = color;
                E21.BackgroundColor = color;
                E22.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E23.BackgroundColor = color;
                E24.BackgroundColor = color;
                E25.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E26.BackgroundColor = color;
                E27.BackgroundColor = color;
                E28.BackgroundColor = color;
            }
            else if (visualElement.Id == E30.Id || visualElement.Id == E31.Id || visualElement.Id == E32.Id || visualElement.Id == E33.Id || visualElement.Id == E34.Id || visualElement.Id == E35.Id || visualElement.Id == E36.Id || visualElement.Id == E37.Id || visualElement.Id == E38.Id)
            {
                //erste Zeile des Quadranten
                E30.BackgroundColor = color;
                E31.BackgroundColor = color;
                E32.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E33.BackgroundColor = color;
                E34.BackgroundColor = color;
                E35.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E36.BackgroundColor = color;
                E37.BackgroundColor = color;
                E38.BackgroundColor = color;
            }
            else if (visualElement.Id == E40.Id || visualElement.Id == E41.Id || visualElement.Id == E42.Id || visualElement.Id == E43.Id || visualElement.Id == E44.Id || visualElement.Id == E45.Id || visualElement.Id == E46.Id || visualElement.Id == E47.Id || visualElement.Id == E48.Id)
            {
                //erste Zeile des Quadranten
                E40.BackgroundColor = color;
                E41.BackgroundColor = color;
                E42.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E43.BackgroundColor = color;
                E44.BackgroundColor = color;
                E45.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E46.BackgroundColor = color;
                E47.BackgroundColor = color;
                E48.BackgroundColor = color;
            }
            else if (visualElement.Id == E50.Id || visualElement.Id == E51.Id || visualElement.Id == E52.Id || visualElement.Id == E53.Id || visualElement.Id == E54.Id || visualElement.Id == E55.Id || visualElement.Id == E56.Id || visualElement.Id == E57.Id || visualElement.Id == E58.Id)
            {
                //erste Zeile des Quadranten
                E50.BackgroundColor = color;
                E51.BackgroundColor = color;
                E52.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E53.BackgroundColor = color;
                E54.BackgroundColor = color;
                E55.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E56.BackgroundColor = color;
                E57.BackgroundColor = color;
                E58.BackgroundColor = color;
            }
            else if (visualElement.Id == E60.Id || visualElement.Id == E61.Id || visualElement.Id == E62.Id || visualElement.Id == E63.Id || visualElement.Id == E64.Id || visualElement.Id == E65.Id || visualElement.Id == E66.Id || visualElement.Id == E67.Id || visualElement.Id == E68.Id)
            {
                //erste Zeile des Quadranten
                E60.BackgroundColor = color;
                E61.BackgroundColor = color;
                E62.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E63.BackgroundColor = color;
                E64.BackgroundColor = color;
                E65.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E66.BackgroundColor = color;
                E67.BackgroundColor = color;
                E68.BackgroundColor = color;
            }
            else if (visualElement.Id == E70.Id || visualElement.Id == E71.Id || visualElement.Id == E72.Id || visualElement.Id == E73.Id || visualElement.Id == E74.Id || visualElement.Id == E75.Id || visualElement.Id == E76.Id || visualElement.Id == E77.Id || visualElement.Id == E78.Id)
            {
                //erste Zeile des Quadranten
                E70.BackgroundColor = color;
                E71.BackgroundColor = color;
                E72.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E73.BackgroundColor = color;
                E74.BackgroundColor = color;
                E75.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E76.BackgroundColor = color;
                E77.BackgroundColor = color;
                E78.BackgroundColor = color;
            }
            else if (visualElement.Id == E80.Id || visualElement.Id == E81.Id || visualElement.Id == E82.Id || visualElement.Id == E83.Id || visualElement.Id == E84.Id || visualElement.Id == E85.Id || visualElement.Id == E86.Id || visualElement.Id == E87.Id || visualElement.Id == E88.Id)
            {
                //erste Zeile des Quadranten
                E80.BackgroundColor = color;
                E81.BackgroundColor = color;
                E82.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E83.BackgroundColor = color;
                E84.BackgroundColor = color;
                E85.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E86.BackgroundColor = color;
                E87.BackgroundColor = color;
                E88.BackgroundColor = color;
            }
        }

        private void Spalte(VisualElement visualElement, Color color)
        {
            if (visualElement.Id == E00.Id || visualElement.Id == E10.Id || visualElement.Id == E20.Id || visualElement.Id == E30.Id || visualElement.Id == E40.Id || visualElement.Id == E50.Id || visualElement.Id == E60.Id || visualElement.Id == E70.Id || visualElement.Id == E80.Id)
            {
                //erste Zeile des Quadranten
                E00.BackgroundColor = color;
                E10.BackgroundColor = color;
                E20.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E30.BackgroundColor = color;
                E40.BackgroundColor = color;
                E50.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E60.BackgroundColor = color;
                E70.BackgroundColor = color;
                E80.BackgroundColor = color;

            }
            else if (visualElement.Id == E01.Id || visualElement.Id == E11.Id || visualElement.Id == E21.Id || visualElement.Id == E31.Id || visualElement.Id == E41.Id || visualElement.Id == E51.Id || visualElement.Id == E61.Id || visualElement.Id == E71.Id || visualElement.Id == E81.Id)
            {
                //erste Zeile des Quadranten
                E01.BackgroundColor = color;
                E11.BackgroundColor = color;
                E21.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E31.BackgroundColor = color;
                E41.BackgroundColor = color;
                E51.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E61.BackgroundColor = color;
                E71.BackgroundColor = color;
                E81.BackgroundColor = color;
            }
            else if (visualElement.Id == E02.Id || visualElement.Id == E12.Id || visualElement.Id == E22.Id || visualElement.Id == E32.Id || visualElement.Id == E42.Id || visualElement.Id == E52.Id || visualElement.Id == E62.Id || visualElement.Id == E72.Id || visualElement.Id == E82.Id)
            {
                //erste Zeile des Quadranten
                E02.BackgroundColor = color;
                E12.BackgroundColor = color;
                E22.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E32.BackgroundColor = color;
                E42.BackgroundColor = color;
                E52.BackgroundColor = color;
                //Dritte Zeile des Quadranten
                E62.BackgroundColor = color;
                E72.BackgroundColor = color;
                E82.BackgroundColor = color;
            }
            else if (visualElement.Id == E03.Id || visualElement.Id == E13.Id || visualElement.Id == E23.Id || visualElement.Id == E33.Id || visualElement.Id == E43.Id || visualElement.Id == E53.Id || visualElement.Id == E63.Id || visualElement.Id == E73.Id || visualElement.Id == E83.Id)
            {
                //erste Zeile des Quadranten
                E03.BackgroundColor = color;
                E13.BackgroundColor = color;
                E23.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E33.BackgroundColor = color;
                E43.BackgroundColor = color;
                E53.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E63.BackgroundColor = color;
                E73.BackgroundColor = color;
                E83.BackgroundColor = color;
            }
            else if (visualElement.Id == E04.Id || visualElement.Id == E14.Id || visualElement.Id == E24.Id || visualElement.Id == E34.Id || visualElement.Id == E44.Id || visualElement.Id == E54.Id || visualElement.Id == E64.Id || visualElement.Id == E74.Id || visualElement.Id == E84.Id)
            {
                //erste Zeile des Quadranten
                E04.BackgroundColor = color;
                E14.BackgroundColor = color;
                E24.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E34.BackgroundColor = color;
                E44.BackgroundColor = color;
                E54.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E64.BackgroundColor = color;
                E74.BackgroundColor = color;
                E84.BackgroundColor = color;
            }
            else if (visualElement.Id == E05.Id || visualElement.Id == E15.Id || visualElement.Id == E25.Id || visualElement.Id == E35.Id || visualElement.Id == E45.Id || visualElement.Id == E55.Id || visualElement.Id == E65.Id || visualElement.Id == E75.Id || visualElement.Id == E85.Id)
            {
                //erste Zeile des Quadranten
                E05.BackgroundColor = color;
                E15.BackgroundColor = color;
                E25.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E35.BackgroundColor = color;
                E45.BackgroundColor = color;
                E55.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E65.BackgroundColor = color;
                E75.BackgroundColor = color;
                E85.BackgroundColor = color;
            }
            else if (visualElement.Id == E06.Id || visualElement.Id == E16.Id || visualElement.Id == E26.Id || visualElement.Id == E36.Id || visualElement.Id == E46.Id || visualElement.Id == E56.Id || visualElement.Id == E66.Id || visualElement.Id == E76.Id || visualElement.Id == E86.Id)
            {
                //erste Zeile des Quadranten
                E06.BackgroundColor = color;
                E16.BackgroundColor = color;
                E26.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E36.BackgroundColor = color;
                E46.BackgroundColor = color;
                E56.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E66.BackgroundColor = color;
                E76.BackgroundColor = color;
                E86.BackgroundColor = color;
            }
            else if (visualElement.Id == E07.Id || visualElement.Id == E17.Id || visualElement.Id == E27.Id || visualElement.Id == E37.Id || visualElement.Id == E47.Id || visualElement.Id == E57.Id || visualElement.Id == E67.Id || visualElement.Id == E77.Id || visualElement.Id == E87.Id)
            {
                //erste Zeile des Quadranten
                E07.BackgroundColor = color;
                E17.BackgroundColor = color;
                E27.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E37.BackgroundColor = color;
                E47.BackgroundColor = color;
                E57.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E67.BackgroundColor = color;
                E77.BackgroundColor = color;
                E87.BackgroundColor = color;
            }
            else if (visualElement.Id == E08.Id || visualElement.Id == E18.Id || visualElement.Id == E28.Id || visualElement.Id == E38.Id || visualElement.Id == E48.Id || visualElement.Id == E58.Id || visualElement.Id == E68.Id || visualElement.Id == E78.Id || visualElement.Id == E88.Id)
            {
                //erste Zeile des Quadranten
                E08.BackgroundColor = color;
                E18.BackgroundColor = color;
                E28.BackgroundColor = color;
                //zweite Zeile des Quadranten
                E38.BackgroundColor = color;
                E48.BackgroundColor = color;
                E58.BackgroundColor = color;
                //dritte Zeile des Quadranten
                E68.BackgroundColor = color;
                E78.BackgroundColor = color;
                E88.BackgroundColor = color;
            }
        }

    }
}