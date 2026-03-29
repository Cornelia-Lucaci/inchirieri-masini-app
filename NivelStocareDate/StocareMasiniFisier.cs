using System;
using System.Collections.Generic;
using System.IO;

namespace InchirieriAuto
{
    public class StocareMasiniFisier
    {
        private readonly string numeFisier;

        public string GetNumeFisier()
        {
            return numeFisier;
        }

        public StocareMasiniFisier(string numeFisier)
        {
            this.numeFisier = numeFisier;
        }

        public void SalveazaMasina(Masina masina)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(numeFisier, true))
                {
                    sw.WriteLine($"{masina.Marca};{masina.Model};{masina.An};{masina.CuloareMasina};{(int)masina.OptiuniMasina};{masina.Disponibila}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare salvare masina: " + ex.Message);
            }
        }

        public List<Masina> CitesteMasini()
        {
            List<Masina> lista = new List<Masina>();

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
                        if (parts.Length != 6) continue;

                        Enum.TryParse(parts[3], out Culoare culoare);

                        //citire optiuni ca text (nu int)
                        Enum.TryParse(parts[4], out Optiuni optiuni);

                        //citire disponibilitate ca text
                        bool disponibil = parts[5] == "Disponibila";

                        Masina m = new Masina(
                            parts[0],
                            parts[1],
                            int.Parse(parts[2]),
                            culoare,
                            optiuni
                        );
                        m.Disponibila = disponibil;
                        lista.Add(m);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare citire fisier: " + ex.Message);
            }

            return lista;
        }
    }
}