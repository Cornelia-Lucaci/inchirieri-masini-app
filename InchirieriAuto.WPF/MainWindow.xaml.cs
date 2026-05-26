using InchirieriAuto;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections;

namespace WpfInchirieri
{
    public partial class MainWindow : Window
    {
        private List<MasinaAfisare> listaCompleta = new();
        private ObservableCollection<MasinaAfisare> masiniAfisare = new();
        private MasinaAfisare masinaSelectata;

        private List<ClientAfisare> listaClientiCompleta = new();
        private ObservableCollection<ClientAfisare> clientiAfisare = new();
        private ClientAfisare clientSelectat;

        // constante (CERINȚĂ)
        private const int AN_MIN = 1900;
        private const int AN_MAX = 2026;

        // clasa de afisare
        //Implementare MVVM minim
        public class MasinaAfisare : INotifyPropertyChanged, IDataErrorInfo
        {
            private string marca;
            private string model;
            private int an;

            public string Marca
            {
                get => marca;
                set
                {
                    marca = value;
                    OnPropertyChanged(nameof(Marca));
                }
            }

            public string Model
            {
                get => model;
                set
                {
                    model = value;
                    OnPropertyChanged(nameof(Model));
                }
            }

            public int An
            {
                get => an;
                set
                {
                    an = value;
                    OnPropertyChanged(nameof(An));
                }
            }

            public string CuloareMasina { get; set; }
            public string OptiuniMasina { get; set; }
            public bool Disponibila { get; set; }
            public string Transmisie { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string nume)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nume));
            }

            //validare prin binding
            public string Error => null;

            public string this[string columnName]
            {
                get
                {
                    switch (columnName)
                    {
                        case nameof(Marca):
                            if (string.IsNullOrWhiteSpace(Marca))
                                return "Marca este obligatorie";
                            break;

                        case nameof(Model):
                            if (string.IsNullOrWhiteSpace(Model))
                                return "Modelul este obligatoriu";
                            break;

                        case nameof(An):
                            if (An < 1900 || An > 2026)
                                return "An invalid";
                            break;
                    }

                    return null;
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            masinaBinding.Marca = "Toyota";
            masinaBinding.An = 2024;
            masinaBinding.Model = "Celica";

            DataContext = masinaBinding;

            cbCuloare.ItemsSource = Enum.GetValues(typeof(Culoare));

            dgMasini.ItemsSource = masiniAfisare;

            IncarcaDate();

            dgModificare.ItemsSource = masiniAfisare;

            cbCuloareMod.ItemsSource = Enum.GetValues(typeof(Culoare));

            dgClientiMod.ItemsSource = clientiAfisare;
            dgClientiDelete.ItemsSource = clientiAfisare;
            dgClientiSearch.ItemsSource = clientiAfisare;
            dgClientiAdd.ItemsSource = clientiAfisare;

            dgStergereMasina.ItemsSource = masiniAfisare;

            IncarcaClienti();
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

        private void GoToStergereMasina(object sender, RoutedEventArgs e)
        {
            GridMeniu.Visibility = Visibility.Collapsed;

            GridAdaugare.Visibility = Visibility.Collapsed;
            GridCautare.Visibility = Visibility.Collapsed;
            GridModificare.Visibility = Visibility.Collapsed;

            GridStergereMasina.Visibility = Visibility.Visible;
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            GridAdaugare.Visibility = Visibility.Collapsed;
            GridCautare.Visibility = Visibility.Collapsed;
            GridModificare.Visibility = Visibility.Collapsed;

            GridClientiAdd.Visibility = Visibility.Collapsed;
            GridClientiMod.Visibility = Visibility.Collapsed;
            GridClientiSearch.Visibility = Visibility.Collapsed;
            GridClientiDelete.Visibility = Visibility.Collapsed;
            GridStergereMasina.Visibility = Visibility.Collapsed;

            GridMeniu.Visibility = Visibility.Visible;
        }
        private void AdaugaMasina_Click(object sender, RoutedEventArgs e)
        {
            ResetCulori();

            bool valid = true;

            string marca = masinaBinding.Marca;
            string model = masinaBinding.Model;
            int an = masinaBinding.An;

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
            if (an < AN_MIN || an > AN_MAX)
            {
                lblAn.Foreground = Brushes.Red;
                valid = false;
            }

            // VALIDARE CULOARE
            if (cbCuloare.SelectedItem == null)
            {
                lblCuloare.Foreground = Brushes.Red;
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
            lblCuloare.Foreground = Brushes.Black;
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

            ResetCulori();
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
            "✔ Cautare masini dupa marca\n" +
            "✔ Modificare masina\n" +
            "✔ Stergere masina\n" +
            "✔ Afisare masini in tabel\n\n" +
            "✔ Adaugare clienti\n" +
            "✔ Cautare clienti dupa nume sau prenume\n" +
            "✔ Modificare client\n" +
            "✔ Stergere client\n" +
            "✔ Afisare clienti in tabel\n\n" +
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
                MessageBox.Show("Nu s-au gasit masini cu aceasta marca!");
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

        private void StergeMasina_Click(object sender, RoutedEventArgs e)
        {
            if (dgStergereMasina.SelectedItem is not MasinaAfisare masina)
            {
                MessageBox.Show("Selectati o masina!");
                return;
            }

            listaCompleta.Remove(masina);
            masiniAfisare.Remove(masina);

            dgMasini.Items.Refresh();
            dgModificare.Items.Refresh();
            dgStergereMasina.Items.Refresh();

            SalveazaDate();

            MessageBox.Show("Masina stearsa!");
        }

        public class ClientAfisare
        {
            public string Nume { get; set; }
            public string Prenume { get; set; }
            public string CNP { get; set; }
        }
        private void HideAllMainPages()
        {
            GridMeniu.Visibility = Visibility.Collapsed;

            GridAdaugare.Visibility = Visibility.Collapsed;
            GridCautare.Visibility = Visibility.Collapsed;
            GridModificare.Visibility = Visibility.Collapsed;

            GridClientiAdd.Visibility = Visibility.Collapsed;
            GridClientiMod.Visibility = Visibility.Collapsed;
            GridClientiSearch.Visibility = Visibility.Collapsed;
            GridClientiDelete.Visibility = Visibility.Collapsed;
        }

        private void GoToClienti(object sender, RoutedEventArgs e)
        {
            HideAllMainPages();

            GridClientiAdd.Visibility = Visibility.Visible;
        }

        private void BackToClientMenu(object sender, RoutedEventArgs e)
        {
            HideAllMainPages();

            GridMeniu.Visibility = Visibility.Visible;
        }

        private void GoToClientAdd(object sender, RoutedEventArgs e)
        {
            HideAllMainPages();

            GridClientiAdd.Visibility = Visibility.Visible;
        }

        private void GoToClientMod(object sender, RoutedEventArgs e)
        {
            HideAllMainPages();

            GridClientiMod.Visibility = Visibility.Visible;
        }

        private void GoToClientSearch(object sender, RoutedEventArgs e)
        {
            HideAllMainPages();

            GridClientiSearch.Visibility = Visibility.Visible;
        }

        private void GoToClientDelete(object sender, RoutedEventArgs e)
        {
            HideAllMainPages();

            GridClientiDelete.Visibility = Visibility.Visible;
        }


        private string filePathClienti = "clienti.json";

        private void SalveazaClienti()
        {
            string json = JsonSerializer.Serialize(listaClientiCompleta);
            File.WriteAllText(filePathClienti, json);
        }

        private void IncarcaClienti()
        {
            if (File.Exists(filePathClienti))
            {
                string json = File.ReadAllText(filePathClienti);
                var clienti = JsonSerializer.Deserialize<List<ClientAfisare>>(json);

                if (clienti != null)
                {
                    listaClientiCompleta = clienti;

                    clientiAfisare.Clear();
                    foreach (var c in clienti)
                        clientiAfisare.Add(c);
                }
            }
        }

        private void AdaugaClient_Click(object sender, RoutedEventArgs e)
        {
            // RESET CULORI
            lblNumeAdd.Foreground = Brushes.Black;
            lblPrenumeAdd.Foreground = Brushes.Black;
            lblCNPAdd.Foreground = Brushes.Black;

            bool valid = true;

            // VALIDARE NUME
            if (string.IsNullOrWhiteSpace(txtNumeAdd.Text))
            {
                lblNumeAdd.Foreground = Brushes.Red;
                valid = false;
            }

            // VALIDARE PRENUME
            if (string.IsNullOrWhiteSpace(txtPrenumeAdd.Text))
            {
                lblPrenumeAdd.Foreground = Brushes.Red;
                valid = false;
            }

            // VALIDARE CNP
            if (txtCNPAdd.Text.Length != 13 ||
                !txtCNPAdd.Text.All(char.IsDigit))
            {
                lblCNPAdd.Foreground = Brushes.Red;
                valid = false;
            }

            if (!valid)
            {
                MessageBox.Show("Date invalide!");
                return;
            }

            var c = new ClientAfisare
            {
                Nume = txtNumeAdd.Text,
                Prenume = txtPrenumeAdd.Text,
                CNP = txtCNPAdd.Text
            };

            listaClientiCompleta.Add(c);
            clientiAfisare.Add(c);

            SalveazaClienti();

            txtNumeAdd.Clear();
            txtPrenumeAdd.Clear();
            txtCNPAdd.Clear();

            MessageBox.Show("Client adaugat!");
        }

        private void dgClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgClientiMod.SelectedItem is ClientAfisare c)
            {
                clientSelectat = c;

                txtNumeMod.Text = c.Nume;
                txtPrenumeMod.Text = c.Prenume;
                txtCNPMod.Text = c.CNP;
            }
        }

        private void ModificaClient_Click(object sender, RoutedEventArgs e)
        {
            if (clientSelectat == null) return;

            clientSelectat.Nume = txtNumeMod.Text;
            clientSelectat.Prenume = txtPrenumeMod.Text;
            clientSelectat.CNP = txtCNPMod.Text;

            dgClientiMod.Items.Refresh();

            SalveazaClienti();

            MessageBox.Show("Client modificat!");
        }

        private void StergeClient_Click(object sender, RoutedEventArgs e)
        {
            if (dgClientiDelete.SelectedItem is not ClientAfisare c)
            {
                MessageBox.Show("Selecteaza un client!");
                return;
            }

            listaClientiCompleta.Remove(c);
            clientiAfisare.Remove(c);

            dgClientiDelete.Items.Refresh();
            dgClientiMod.Items.Refresh();
            dgClientiSearch.Items.Refresh();

            SalveazaClienti();

            MessageBox.Show("Client sters!");
        }

        private void CautaClient_Click(object sender, RoutedEventArgs e)
        {
            string q = txtCautareClient.Text.ToLower().Trim();

            var rezultate = clientiAfisare
                .Where(c => c.Nume.ToLower().Contains(q))
                .ToList();

            dgClientiSearch.ItemsSource = rezultate;

            if (rezultate.Count == 0)
            {
                MessageBox.Show("Nu s-au gasit clienti cu acest nume!");
            }
        }

        private void ResetClienti_Click(object sender, RoutedEventArgs e)
        {
            dgClientiSearch.ItemsSource = clientiAfisare;
            txtCautareClient.Clear();
        }

        //Binding pentru masina
        private MasinaAfisare masinaBinding = new();

    }
}