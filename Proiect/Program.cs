using System;
using System.Collections.Generic;

class Masina
{
    public string Marca { get; set; }
    public string Model { get; set; }
    public int An { get; set; }
    public bool Disponibila { get; set; }

    public Masina(string marca, string model, int an)
    {
        Marca = marca;
        Model = model;
        An = an;
        Disponibila = true;
    }


    public override string ToString()
    {
        return $"{Marca} {Model} ({An}) - {(Disponibila ? "Disponibila" : "Indisponibila")}";
    }
}

class Client
{
    public string Nume { get; set; }
    public string Prenume { get; set; }
    public string CNP { get; set; }

    public Client(string nume, string prenume, string cnp)
    {
        Nume = nume;
        Prenume = prenume;
        CNP = cnp;
    }
}

class Inchiriere
{
    public Client Client { get; set; }
    public Masina Masina { get; set; }
    public int Zile { get; set; }

    public Inchiriere(Client client, Masina masina, int zile)
    {
        Client = client;
        Masina = masina;
        Zile = zile;
    }
}

class Program
{
    static void Main()
    {
        //logarea 
        string utilizatorCorect = "admin";
        string passCorect = "4622";

        Console.Write("User: ");
        string utilizator = Console.ReadLine();

        Console.Write("Parola: ");
        string pass = Console.ReadLine();

        if (utilizator != utilizatorCorect || pass != passCorect)
        {
            Console.WriteLine("Acces nepermis!");
            return;
        }

        Console.WriteLine("Logare reusita!\n");

        List<Masina> masini = new List<Masina>();
        List<Inchiriere> inchirieri = new List<Inchiriere>();

        while (true)
        {
            Console.WriteLine("\n --- Meniu ---");
            Console.WriteLine("1. Adaugare masina");
            Console.WriteLine("2. Salvare date masina");
            Console.WriteLine("3. Afisare lista masini");
            Console.WriteLine("4. Cautare masina dupa marca");
            Console.WriteLine("5. Inchiriere masina");
            Console.WriteLine("0. Terminare program");

            Console.Write("Optiune: ");
            int opt = int.Parse(Console.ReadLine());

            switch (opt)
            {
                case 1:
                    Console.Write("Marca masina: ");
                    string marca = Console.ReadLine();

                    Console.Write("Model: ");
                    string model = Console.ReadLine();

                    Console.Write("An fabricatie: ");
                    int an = int.Parse(Console.ReadLine());

                    Masina m = new Masina(marca, model, an);
                    masini.Add(m);

                    Console.WriteLine();
                    break;

                case 2:
                    if (masini.Count == 0)
                    {
                        Console.WriteLine("Nu exista masini de salvat!");
                    }
                    else
                    {
                        Console.WriteLine("Datele despre masini au fost salvate!");
                    }
                    break;


                case 3:
                    Console.WriteLine("\nLista masini:");
                    for (int i = 0; i < masini.Count; i++)
                    {
                        Console.WriteLine($"{i}. {masini[i]}");
                    }
                    break;


                case 4:
                    Console.Write("Introdu marca: ");
                    string cautare = Console.ReadLine();

                    Console.WriteLine("Rezultate:");

                    bool gasit = false;

                    foreach (var masina in masini)
                    {
                        if (masina.Marca.ToLower() == cautare.ToLower())
                        {
                            Console.WriteLine(masina);
                            gasit = true;
                        }
                    }

                    if (!gasit)
                    {
                        Console.WriteLine("Nu exista masini cu aceasta marca!");
                    }
                    break;

                case 5:
                    if (masini.Count == 0)
                    {
                        Console.WriteLine("Nu exista masini!");
                        break;
                    }

                    Console.WriteLine("Masini disponibile:");
                    for (int i = 0; i < masini.Count; i++)
                    {
                        Console.WriteLine($"{i}. {masini[i]}");
                    }

                    Console.Write("Alege index masina: ");
                    int index = int.Parse(Console.ReadLine());

                    if (index < 0 || index >= masini.Count)
                    {
                        Console.WriteLine("Index invalid!");
                        break;
                    }

                    Masina masinaAleasa = masini[index];

                    if (!masinaAleasa.Disponibila)
                    {
                        Console.WriteLine("Masina nu este disponibila!");
                        break;
                    }

                    Console.Write("Nume: ");
                    string nume = Console.ReadLine();

                    Console.Write("Prenume: ");
                    string prenume = Console.ReadLine();

                    Console.Write("CNP: ");
                    string cnp = Console.ReadLine();

                    Console.Write("Numar zile: ");
                    int zile = int.Parse(Console.ReadLine());

                    Client client = new Client(nume, prenume, cnp);
                    Inchiriere inch = new Inchiriere(client, masinaAleasa, zile);

                    inchirieri.Add(inch);
                    masinaAleasa.Disponibila = false;

                    Console.WriteLine("Inchiriere realizata!");
                    break;

                case 0:
                    Console.WriteLine("O zi frumoasa!");
                    return;

                default:
                    Console.WriteLine("Optiune invalida!");
                    break;
            }
        }
    }
}