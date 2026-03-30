using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace InchirieriAuto
{
    public class ServiceMasini
    {
        private List<Masina> masini = new List<Masina>();
        private StocareMasiniFisier stocare;

        public ServiceMasini()
        {
            // cale absoluta spre fisierul din proiect (unde l-ai creat)
            string caleFisier = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\masini.txt");

            // creaza fisierul daca nu exista
            if (!File.Exists(caleFisier))
                File.Create(caleFisier).Close();

            // daca exista, asigura-te ca nu e ReadOnly
            FileInfo fi = new FileInfo(caleFisier);
            if (fi.IsReadOnly) fi.IsReadOnly = false;

            stocare = new StocareMasiniFisier(caleFisier);

            // citeste masinile existente
            masini = stocare.CitesteMasini();
        }

        public void AdaugaMasina(string marca, string model, int an, Culoare culoare, Optiuni optiuni)
        {
            Masina masina = new Masina(marca, model, an, culoare, optiuni);
            masini.Add(masina);
            //stocare.SalveazaMasina(masina);
            Console.WriteLine("Masina salvata cu succes!");
        }

        public List<Masina> GetMasini() => masini;

        public void AfiseazaMasini()
        {
            if (masini.Count == 0)
            {
                Console.WriteLine("Nu exista masini!");
                return;
            }
            for (int i = 0; i < masini.Count; i++)
                Console.WriteLine($"{i}. {masini[i]}");
        }

        //linq
        public List<Masina> CautaDupaMarca(string marca)
        {
            return masini
                .Where(m => m.Marca.ToLower().Contains(marca.ToLower()))
                .ToList();
        }

        public void SalveazaToateMasinile()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(stocare.GetNumeFisier(), false))
                {
                    foreach (var masina in masini)
                    {
                        sw.WriteLine($"{masina.Marca};{masina.Model};{masina.An};{masina.CuloareMasina};{masina.OptiuniMasina};{(masina.Disponibila ? "Disponibila" : "Indisponibila")}");
                    }
                }

                Console.WriteLine("Masinile au fost salvate in fisier!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare salvare: " + ex.Message);
            }
        }

        public void ModificaMasinaDupaMarca(string marca)
        {
            var masina = masini
                .FirstOrDefault(m => m.Marca.ToLower().Contains(marca.ToLower()));

            if (masina == null)
            {
                Console.WriteLine("Masina nu a fost gasita!");
                return;
            }

            Console.WriteLine("Masina gasita:");
            Console.WriteLine(masina);

            Console.Write("Model nou: ");
            masina.Model = Console.ReadLine();

            Console.Write("An nou: ");
            masina.An = int.Parse(Console.ReadLine());

            Console.WriteLine("Alege culoare noua:");
            foreach (var c in Enum.GetValues(typeof(Culoare)))
                Console.WriteLine($"{(int)c} - {c}");
            masina.CuloareMasina = (Culoare)int.Parse(Console.ReadLine());

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

            masina.OptiuniMasina = optiuni;


            Console.WriteLine("Masina a fost modificata!");
        }
    }
}