using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HighscorePage : ContentPage
    {
        public HighscorePage(string score)
        {
            InitializeComponent();

            var userScore = this.FindByName<Label>("score");
            userScore.Text += score;

        }
        public void BackButton(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Page1());
        }
    }
}