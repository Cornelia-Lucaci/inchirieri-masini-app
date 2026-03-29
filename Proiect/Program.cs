using System;
using InchirieriAuto;

namespace InchirieriAuto
{
    class Program
    {
        static void Main()
        {
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

            ServiceMasini serviceMasini = new ServiceMasini();
            ServiceInchirieri serviceInchirieri = new ServiceInchirieri();

            List<Masina> masiniSalvate = new List<Masina>();

            while (true)
            {
                Console.WriteLine("\n--- Meniu ---");
                Console.WriteLine("1. Adaugare masina");
                Console.WriteLine("2. Salvare date masina in lista");
                Console.WriteLine("3. Afisare lista masini");
                Console.WriteLine("4. Cautare masina dupa marca");
                Console.WriteLine("5. Inchiriere masina");
                Console.WriteLine("0. Terminare program");

                Console.Write("Optiune: ");
                int opt = int.Parse(Console.ReadLine());

                switch (opt)
                {
                    case 1:
                        {
                            Console.Write("Marca masina: ");
                            string marca = Console.ReadLine();

                            Console.Write("Model: ");
                            string model = Console.ReadLine();

                            //Console.Write("An fabricatie: ");
                            //int an = int.Parse(Console.ReadLine());
                            int an;
                            while (true)
                            {
                                Console.Write("An fabricatie: ");
                                string input = Console.ReadLine();
                                if (int.TryParse(input, out an) && an > 1900 && an <= DateTime.Now.Year)
                                    break;
                                Console.WriteLine("An invalid! Introduceti un numar valid.");
                            }

                            // Culoare
                            Console.WriteLine("Alege culoare:");
                            foreach (var c in Enum.GetValues(typeof(Culoare)))
                                Console.WriteLine($"{(int)c} - {c}");
                            Culoare culoare = (Culoare)int.Parse(Console.ReadLine());

                            // Optiuni
                            Optiuni optiuni = Optiuni.Nimic;

                            Console.WriteLine("Are Trapa Panoramica? (da/nu)");
                            if (Console.ReadLine().ToLower() == "da")
                                optiuni |= Optiuni.TrapaPanoramica;

                            Console.WriteLine("Are Navigatie? (da/nu)");
                            if (Console.ReadLine().ToLower() == "da")
                                optiuni |= Optiuni.Navigatie;

                            Console.WriteLine("Are Suspensie Reglabila? (da/nu)");
                            if (Console.ReadLine().ToLower() == "da")
                                optiuni |= Optiuni.SuspensieReglabila;

                            Console.WriteLine("Are Incalzire Scaune? (da/nu)");
                            if (Console.ReadLine().ToLower() == "da")
                                optiuni |= Optiuni.IncalzireScaune;

                            Console.WriteLine("Are Senzori Parcare? (da/nu)");
                            if (Console.ReadLine().ToLower() == "da")
                                optiuni |= Optiuni.SenzoriParcare;

                            Masina masinaNoua = new Masina(marca, model, an, culoare, optiuni);
                            masiniSalvate.Add(masinaNoua);
                            Console.WriteLine("Masina salvata in lista!");

                            serviceMasini.AdaugaMasina(marca, model, an, culoare, optiuni);
                            break;
                        }

                    case 2:
                        { 
                            Console.WriteLine("Masini salvate:");
                            foreach (var m in masiniSalvate)
                            Console.WriteLine(m);

                            break;
                        }

                    case 3:
                        serviceMasini.AfiseazaMasini();
                        break;

                    case 4:
                        {
                            Console.Write("Introdu marca: ");
                            string cautare = Console.ReadLine();

                            var rezultate = serviceMasini.CautaDupaMarca(cautare);
                            foreach (var m in rezultate)
                                Console.WriteLine(m);

                            break;
                        }

                    case 5:
                        {
                            var masini = serviceMasini.GetMasini();
                            if (masini.Count == 0)
                            {
                                Console.WriteLine("Nu exista masini!");
                                break;
                            }

                            for (int i = 0; i < masini.Count; i++)
                                Console.WriteLine($"{i}. {masini[i]}");

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

                          
                            string cnp;
                            do
                            {
                                Console.Write("CNP: ");
                                cnp = Console.ReadLine();

                                if (cnp.Length != 13 || !long.TryParse(cnp, out _))
                                    Console.WriteLine("CNP invalid! Trebuie sa aiba 13 cifre.");
                            } while (cnp.Length != 13 || !long.TryParse(cnp, out _));

                            Console.Write("Numar zile: ");
                            int zile = int.Parse(Console.ReadLine());

                            Client client = new Client(nume, prenume, cnp);
                            serviceInchirieri.InchiriazaMasina(masinaAleasa, client, zile);
                            break;
                        }

                    case 0:
                        serviceMasini.SalveazaToateMasinile();
                        Console.WriteLine("O zi frumoasa!");
                        return;

                    default:
                        Console.WriteLine("Optiune invalida!");
                        break;
                }
            }
        }
    }
}