using System;
using System.Collections.Generic;
using System.IO;

namespace InchirieriAuto
{
    public class StocareClientiFisier
    {
        private readonly string numeFisier;

        public string GetNumeFisier()
        {
            return numeFisier;
        }

        public StocareClientiFisier(string numeFisier)
        {
            this.numeFisier = numeFisier;
        }

        public void SalveazaClient(Client client)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(numeFisier, true))
                {
                    sw.WriteLine($"{client.Nume};{client.Prenume};{client.CNP}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare salvare client: " + ex.Message);
            }
        }

        public List<Client> CitesteClienti()
        {
            List<Client> lista = new List<Client>();

            if (!File.Exists(numeFisier))
                return lista;

            try
            {
                using (StreamReader sr = new StreamReader(numeFisier))
                {
                    string linie;
                    while ((linie = sr.ReadLine()) != null)
                    {
                        string[] parts = linie.Split(';');
                        if (parts.Length != 3) continue;

                        Client c = new Client(parts[0], parts[1], parts[2]);
                        lista.Add(c);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare citire clienti: " + ex.Message);
            }

            return lista;
        }
    }
}
