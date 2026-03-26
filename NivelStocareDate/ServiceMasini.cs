using System;
using System.Collections.Generic;
using InchirieriAuto;
using System.Linq;

namespace InchirieriAuto
{
    public class ServiceMasini
    {
        private List<Masina> masini = new List<Masina>();

        public void AdaugaMasina(string marca, string model, int an, Culoare culoare, Optiuni optiuni)
        {
            masini.Add(new Masina(marca, model, an, culoare, optiuni));
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
            List<Masina> rezultate = new List<Masina>();
            //MODIFICAT CU LINQ
            return masini
                .Where(m => m.Marca.ToLower().Contains(marca.ToLower()))
                .ToList();
        }
    }
}