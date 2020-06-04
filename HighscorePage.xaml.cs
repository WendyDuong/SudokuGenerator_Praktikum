using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace App3
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class Highscore: ContentPage
    {
        const string highscoresFilename = "highscores.xml";
        public static List<HighscoreItem> scores;

        public Highscore()
        {
            InitializeComponent();
            // .ItemsSource = Scores;

            Load();

        }


        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        public void ToolbarItemActivatedStart(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Page1());
        }

        static public void Load()
        {
            scores = new List<HighscoreItem> ();
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();

            // Create empty list if the highscores file does not exist.
            // This is needed when the application is started for the first time.
            if (!store.FileExists(highscoresFilename))
            {
                for (int i = 1; i <= 5; i++)
                {
                    scores.Add(new HighscoreItem(i,
                        new TimeSpan(0, 59, 59)));
                }
                Save();
                return;
            }

            // Open the file and use XmlSerializer to deserialize the xml file into
            // a list of HighscoreItems.
            using (IsolatedStorageFileStream stream = store.OpenFile(highscoresFilename, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    XmlSerializer serializer = new XmlSerializer(scores.GetType());
                    scores = (List <HighscoreItem>)serializer.Deserialize(reader);
                }
            }
        }

        /// <summary>
        /// Saves the highscores to isolated storage.
        /// </summary>
        static public void Save()
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();

            // Open the file and use XmlSerializer to serialize the list into the file
            using (IsolatedStorageFileStream stream = store.CreateFile(highscoresFilename))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    XmlSerializer serializer = new XmlSerializer(scores.GetType());
                    serializer.Serialize(writer, scores);
                    writer.Flush();
                }
            }
        }

        /// <summary>
        /// Checks if given score is a new highscore
        /// </summary>
        /// <param name="score">Score to check. The score should contain at least the solving time and moves.</param>
        /// <returns>The position in highscore list, or zero if the score doesn't make it to the list</returns>
        public static int IsNewHighscore(HighscoreItem score)
        {
            foreach (HighscoreItem item in scores)
            {
                // Check the time, and if the times are the same, check the
                
                if (score.Time < item.Time ||
                    (score.Time == item.Time))
                    return item.Index;
            }

            return 0;
        }

        /// <summary>
        /// Add a new score to highscore list
        /// </summary>
        /// <param name="score">Score to add. All members of the score should be filled.</param>
        public static void AddNewHighscore(HighscoreItem score)
        {
            // Insert the score into the list, remove weakest score from the list
            // and save the list.
            if (score.Index <= 0)
                return;
            scores.Insert(score.Index - 1, score);
            scores.RemoveAt(scores.Count - 1);
            for (int t = score.Index; t < scores.Count; t++)
                scores[t].Index++;
            Save();
        }
    }
}

