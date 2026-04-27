using System.Collections.Generic;
using System.Windows;
using InchirieriAuto; // IMPORTANT

namespace WpfInchirieri
{
    public partial class MainWindow : Window
    {
        private List<Masina> masini;

        public MainWindow()
        {
            InitializeComponent();
            IncarcaMasini();
        }

        private void IncarcaMasini()
        {
            masini = new List<Masina>()
            {
                new Masina("BMW", "X5", 2020, Culoare.Alb, Optiuni.Navigatie | Optiuni.SenzoriParcare),
                new Masina("Audi", "A4", 2019, Culoare.Roz, Optiuni.TrapaPanoramica),
                new Masina("Dacia", "Logan", 2018, Culoare.Albastru, Optiuni.Nimic)
            };

            dgMasini.ItemsSource = masini;
        }
        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            IncarcaMasini();
        }
    }
}