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

                Console.WriteLine("Toate masinile au fost salvate in fisier!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare salvare: " + ex.Message);
            }
        }
    }
}