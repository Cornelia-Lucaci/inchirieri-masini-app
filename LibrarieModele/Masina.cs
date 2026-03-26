using System;


namespace InchirieriAuto
{
    public class Masina
    {
        public string Marca { get; set; }
        public string Model { get; set; }
        public int An { get; set; }
        public bool Disponibila { get; set; }

        public Culoare CuloareMasina { get; set; }
        public Optiuni OptiuniMasina { get; set; }

        public Masina(string marca, string model, int an, Culoare culoare, Optiuni optiuni)
        {
            Marca = marca;
            Model = model;
            An = an;
            Disponibila = true;
            CuloareMasina = culoare;
            OptiuniMasina = optiuni;
        }

        public override string ToString()
        {
            return $"{Marca} {Model} ({An}) - {CuloareMasina} - {OptiuniMasina} - {(Disponibila ? "Disponibila" : "Indisponibila")}";
        }
    }
}