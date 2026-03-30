using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace InchirieriAuto
{
    public class ServiceClienti
    {
        private List<Client> clienti = new List<Client>();
        private StocareClientiFisier stocare;

        public ServiceClienti()
        {
            string caleFisier = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\clienti.txt");

            if (!File.Exists(caleFisier))
                File.Create(caleFisier).Close();

            stocare = new StocareClientiFisier(caleFisier);

            clienti = stocare.CitesteClienti();
        }

        public void AdaugaClient(string nume, string prenume, string cnp)
        {
            Client client = new Client(nume, prenume, cnp);
            clienti.Add(client);
            Console.WriteLine("Client adaugat!");
        }

        public void AfiseazaClienti()
        {
            if (clienti.Count == 0)
            {
                Console.WriteLine("Nu exista clienti!");
                return;
            }

            foreach (var c in clienti)
                Console.WriteLine($"{c.Nume} {c.Prenume} - {c.CNP}");
        }

        public List<Client> CautaClientDupaNume(string nume)
        {
            return clienti
                .Where(c => c.Nume.ToLower().Contains(nume.ToLower()))
                .ToList();
        }

        public void ModificaClient(string nume)
        {
            var client = clienti
                .FirstOrDefault(c => c.Nume.ToLower().Contains(nume.ToLower()));

            if (client == null)
            {
                Console.WriteLine("Clientul nu a fost gasit!");
                return;
            }

            Console.WriteLine("Client gasit:");
            Console.WriteLine($"{client.Nume} {client.Prenume} - {client.CNP}");

            Console.Write("Nume nou: ");
            client.Nume = Console.ReadLine();

            Console.Write("Prenume nou: ");
            client.Prenume = Console.ReadLine();

            string cnp;
            do
            {
                Console.Write("CNP nou: ");
                cnp = Console.ReadLine();
            } while (cnp.Length != 13 || !long.TryParse(cnp, out _));

            client.CNP = cnp;

            Console.WriteLine("Client modificat!");
        }

        public void SalveazaClienti()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(stocare.GetNumeFisier(), false))
                {
                    foreach (var c in clienti)
                    {
                        sw.WriteLine($"{c.Nume};{c.Prenume};{c.CNP}");
                    }
                }

                Console.WriteLine("Clientii au fost salvati in fisier!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare salvare: " + ex.Message);
            }
        }
    }
}