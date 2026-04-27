using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using InchirieriAuto;

namespace WpfInchirieri
{
    public partial class MainWindow : Window
    {
        private List<Masina> masini = new List<Masina>();

        // constante (CERINȚĂ)
        private const int AN_MIN = 1900;
        private const int AN_MAX = 2026;

        public MainWindow()
        {
            InitializeComponent();

            cbCuloare.ItemsSource = Enum.GetValues(typeof(Culoare));
        }

        private void AdaugaMasina_Click(object sender, RoutedEventArgs e)
        {
            ResetCulori();

            bool valid = true;

            string marca = txtMarca.Text;
            string model = txtModel.Text;

            int an;

            //  VALIDARE MARCA
            if (string.IsNullOrWhiteSpace(marca))
            {
                lblMarca.Foreground = Brushes.Red;
                valid = false;
            }

            // VALIDARE MODEL
            if (string.IsNullOrWhiteSpace(model))
            {
                lblModel.Foreground = Brushes.Red;
                valid = false;
            }

            // VALIDARE AN
            if (!int.TryParse(txtAn.Text, out an) || an < AN_MIN || an > AN_MAX)
            {
                lblAn.Foreground = Brushes.Red;
                valid = false;
            }

            // VALIDARE CULOARE
            if (cbCuloare.SelectedItem == null)
            {
                MessageBox.Show("Selectati culoarea!");
                valid = false;
            }

            if (!valid)
            {
                MessageBox.Show("Date invalide!");
                return;
            }

            // optiuni
            Optiuni opt = Optiuni.Nimic;

            if (chkNav.IsChecked == true)
                opt |= Optiuni.Navigatie;

            if (chkTrapa.IsChecked == true)
                opt |= Optiuni.TrapaPanoramica;

            if (chkSenzori.IsChecked == true)
                opt |= Optiuni.SenzoriParcare;

            // creare masina
            Masina m = new Masina(
                marca,
                model,
                an,
                (Culoare)cbCuloare.SelectedItem,
                opt
            );

            masini.Add(m);

            dgMasini.ItemsSource = null;
            dgMasini.ItemsSource = masini;

            MessageBox.Show("Masina adaugata!");

            ClearForm();
        }

        private void ResetCulori()
        {
            lblMarca.Foreground = Brushes.Black;
            lblModel.Foreground = Brushes.Black;
            lblAn.Foreground = Brushes.Black;
        }

        private void ClearForm()
        {
            txtMarca.Text = "";
            txtModel.Text = "";
            txtAn.Text = "";
            cbCuloare.SelectedItem = null;

            chkNav.IsChecked = false;
            chkTrapa.IsChecked = false;
            chkSenzori.IsChecked = false;
        }
    }
}