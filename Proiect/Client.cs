using System;

namespace InchirieriAuto
{
    public class Client
    {
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public string CNP { get; set; }

        public Client(string nume, string prenume, string cnp)
        {
            Nume = nume;
            Prenume = prenume;

            while (cnp.Length != 13 || !EsteNumeric(cnp))
            {
                Console.WriteLine("CNP invalid!");
                Console.Write("Introdu CNP din nou: ");
                cnp = Console.ReadLine();
            }
            CNP = cnp;
        }
        private bool EsteNumeric(string text)
        {
            foreach (char c in text)
                if (!char.IsDigit(c))
                    return false;
            return true;
        }
    }
}