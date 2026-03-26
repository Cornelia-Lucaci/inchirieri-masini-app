using System;

namespace InchirieriAuto
{
    [Flags]
    public enum Optiuni
    {
        Nimic = 0,
        TrapaPanoramica = 1,
        Navigatie = 2,
        SuspensieReglabila = 4,
        IncalzireScaune = 8,
        SenzoriParcare = 16
    }

    public enum Culoare
    {
        Mov,
        Alb,
        Rosu,
        Albastru,
        Roz,
        Verde
    }
}