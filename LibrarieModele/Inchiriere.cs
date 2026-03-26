using System;

namespace InchirieriAuto
{
    public class Inchiriere
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
}