using InchirieriAuto;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfInchirieri
{
    public partial class MainWindow : Window
    {
        private List<MasinaAfisare> listaCompleta = new();
        private ObservableCollection<MasinaAfisare> masiniAfisare = new();
        private MasinaAfisare masinaSelectata;

        // constante (CERINȚĂ)
        private const int AN_MIN = 1900;
        private const int AN_MAX = 2026;

        // clasa de afisare
        public class MasinaAfisare
        {
            public string Marca { get; set; }
            public string Model { get; set; }
            public int An { get; set; }
            public string CuloareMasina { get; set; }
            public string OptiuniMasina { get; set; }
            public bool Disponibila { get; set; }
            public string Transmisie { get; set; }
        }
        public MainWindow()
        {
            InitializeComponent();

            cbCuloare.ItemsSource = Enum.GetValues(typeof(Culoare));

            dgMasini.ItemsSource = masiniAfisare;

            IncarcaDate();

            dgModificare.ItemsSource = masiniAfisare;

            cbCuloareMod.ItemsSource = Enum.GetValues(typeof(Culoare));
        }

        private void GoToAdaugare(object sender, RoutedEventArgs e)
        {
            GridMeniu.Visibility = Visibility.Collapsed;
            GridCautare.Visibility = Visibility.Collapsed;
            GridAdaugare.Visibility = Visibility.Visible;
        }

        private void GoToCautare(object sender, RoutedEventArgs e)
        {
            GridMeniu.Visibility = Visibility.Collapsed;
            GridAdaugare.Visibility = Visibility.Collapsed;
            GridCautare.Visibility = Visibility.Visible;
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            GridAdaugare.Visibility = Visibility.Collapsed;
            GridCautare.Visibility = Visibility.Collapsed;
            GridModificare.Visibility = Visibility.Collapsed;

            GridMeniu.Visibility = Visibility.Visible;
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

            if (chkTrapa.IsChecked == true)
                opt |= Optiuni.TrapaPanoramica;

            if (chkNav.IsChecked == true)
                opt |= Optiuni.Navigatie;

            if (chkSuspensie.IsChecked == true)
                opt |= Optiuni.SuspensieReglabila;

            if (chkIncalzire.IsChecked == true)
                opt |= Optiuni.IncalzireScaune;

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

            string transmisie = rbManual.IsChecked == true ? "Manuala" : "Automata";

            // dacă vrei transmisia în tabel, NU în clasa Masina:
            var masinaAfisare = new MasinaAfisare
            {
                Marca = m.Marca,
                Model = m.Model,
                An = m.An,
                CuloareMasina = m.CuloareMasina.ToString(),
                OptiuniMasina = m.OptiuniMasina.ToString(),
                Disponibila = m.Disponibila,
                Transmisie = transmisie
            };

            listaCompleta.Add(masinaAfisare);
            masiniAfisare.Add(masinaAfisare);

            SalveazaDate();

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
            chkSuspensie.IsChecked = false;
            chkIncalzire.IsChecked = false;
            chkSenzori.IsChecked = false;

            rbManual.IsChecked = true; // reset pentru transmisie
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void FocusAdd(object sender, RoutedEventArgs e)
        {
            txtMarca.Focus();
        }

        private void FocusCautare(object sender, RoutedEventArgs e)
        {
            txtCautareMarca.Focus();
        }

        private void Despre_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
            "Aplicatie Inchirieri Auto\n\n" +
            "Aplicatia permite:\n" +
            "✔ Adaugare masini\n" +
            "✔ Cautare dupa marca\n" +
            "✔ Modificare masina\n" +
            "✔ Afisare masini in tabel\n\n" +
            "Autor: Lucaci Cornelia-Maria",
            "Despre",
            MessageBoxButton.OK,
            MessageBoxImage.Information
            );
        }

        public string TransmisieAfisata { get; set; }

        private void CautaMasina_Click(object sender, RoutedEventArgs e)
        {
            string cautare = txtCautareMarca.Text.ToLower().Trim();

            //dacă nu scrii nimic → afișează tot
            if (string.IsNullOrEmpty(cautare))
            {
                ReseteazaLista_Click(null, null);
                return;
            }

            var rezultate = listaCompleta
                .Where(m => m.Marca != null && m.Marca.ToLower().Contains(cautare))
                .ToList();

            masiniAfisare.Clear();

            foreach (var m in rezultate)
                masiniAfisare.Add(m);

            //forțează refresh
            dgMasini.Items.Refresh();

            if (rezultate.Count == 0)
                MessageBox.Show("Nu s-au gasit masini!");
        }

        private void ReseteazaLista_Click(object sender, RoutedEventArgs e)
        {
            masiniAfisare.Clear();

            foreach (var m in listaCompleta)
                masiniAfisare.Add(m);

            txtCautareMarca.Text = "";

            dgMasini.Items.Refresh();
        }

        private string filePath = "masini.json";

        private void SalveazaDate()
        {
            string json = JsonSerializer.Serialize(listaCompleta);
            File.WriteAllText(filePath, json);
        }

        private void IncarcaDate()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var masini = JsonSerializer.Deserialize<List<MasinaAfisare>>(json);

                if (masini != null)
                {
                    listaCompleta = masini;

                    masiniAfisare.Clear();
                    foreach (var m in listaCompleta)
                        masiniAfisare.Add(m);
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            SalveazaDate();
            base.OnClosed(e);
        }

        private void ActualizeazaListBox()
        {
            lstOptiuni.Items.Clear();

            if (chkTrapa.IsChecked == true)
                lstOptiuni.Items.Add("Trapa panoramica");

            if (chkNav.IsChecked == true)
                lstOptiuni.Items.Add("Navigatie");

            if (chkSuspensie.IsChecked == true)
                lstOptiuni.Items.Add("Suspensie reglabila");

            if (chkIncalzire.IsChecked == true)
                lstOptiuni.Items.Add("Incalzire scaune");

            if (chkSenzori.IsChecked == true)
                lstOptiuni.Items.Add("Senzori parcare");
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            ActualizeazaListBox();
        }

        private void GoToModificare(object sender, RoutedEventArgs e)
        {
            GridMeniu.Visibility = Visibility.Collapsed;
            GridModificare.Visibility = Visibility.Visible;
        }

        private void dgModificare_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgModificare.SelectedItem is MasinaAfisare masina)
            {
                masinaSelectata = masina;

                txtMarcaMod.Text = masina.Marca;
                txtModelMod.Text = masina.Model;
                txtAnMod.Text = masina.An.ToString();

                cbCuloareMod.SelectedItem =
                    Enum.Parse(typeof(Culoare), masina.CuloareMasina);

                rbManualMod.IsChecked = masina.Transmisie == "Manuala";
                rbAutomatMod.IsChecked = masina.Transmisie == "Automata";

                chkTrapaMod.IsChecked = false;
                chkNavMod.IsChecked = false;
                chkSuspensieMod.IsChecked = false;
                chkIncalzireMod.IsChecked = false;
                chkSenzoriMod.IsChecked = false;

                string opt = masina.OptiuniMasina;

                if (opt.Contains("TrapaPanoramica"))
                    chkTrapaMod.IsChecked = true;

                if (opt.Contains("Navigatie"))
                    chkNavMod.IsChecked = true;

                if (opt.Contains("SuspensieReglabila"))
                    chkSuspensieMod.IsChecked = true;

                if (opt.Contains("IncalzireScaune"))
                    chkIncalzireMod.IsChecked = true;

                if (opt.Contains("SenzoriParcare"))
                    chkSenzoriMod.IsChecked = true;
            }
        }

        private void ModificaMasina_Click(object sender, RoutedEventArgs e)
        {
            if (masinaSelectata == null)
            {
                MessageBox.Show("Selectati o masina!");
                return;
            }

            masinaSelectata.Marca = txtMarcaMod.Text;
            masinaSelectata.Model = txtModelMod.Text;
            masinaSelectata.An = int.Parse(txtAnMod.Text);

            masinaSelectata.CuloareMasina =
                cbCuloareMod.SelectedItem.ToString();

            // transmisie
            masinaSelectata.Transmisie =
                rbManualMod.IsChecked == true ? "Manuala" : "Automata";

            // optiuni
            List<string> optiuni = new();

            if (chkTrapaMod.IsChecked == true)
                optiuni.Add("TrapaPanoramica");

            if (chkNavMod.IsChecked == true)
                optiuni.Add("Navigatie");

            if (chkSuspensieMod.IsChecked == true)
                optiuni.Add("SuspensieReglabila");

            if (chkIncalzireMod.IsChecked == true)
                optiuni.Add("IncalzireScaune");

            if (chkSenzoriMod.IsChecked == true)
                optiuni.Add("SenzoriParcare");

            masinaSelectata.OptiuniMasina =
                string.Join(", ", optiuni);

            dgMasini.Items.Refresh();
            dgModificare.Items.Refresh();

            SalveazaDate();

            MessageBox.Show("Masina modificata!");
        }
    }
}