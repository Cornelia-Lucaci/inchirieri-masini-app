using System;
using System.Collections.Generic;
using InchirieriAuto;


namespace InchirieriAuto
{
    public class ServiceInchirieri
    {
        private List<Inchiriere> inchirieri = new List<Inchiriere>();

        public void InchiriazaMasina(Masina masina, Client client, int zile)
        {
            if (!masina.Disponibila)
            {
                Console.WriteLine("Masina nu este disponibila!");
                return;
            }

            Inchiriere inch = new Inchiriere(client, masina, zile);
            inchirieri.Add(inch);
            masina.Disponibila = false;

            Console.WriteLine("Inchiriere realizata!");
        }
    }
}