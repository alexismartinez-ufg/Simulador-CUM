namespace SimuladorCUM.Models
{
    public class AsignaturaPensum
    {
        public int Num { get; set; }
        public int Score { get; set; }
        public int Idplan { get; set; }
        public int Ciclo { get; set; }
        public string Idmateria { get; set; }
        public string IdmateriaASU { get; set; }
        public string Materia { get; set; }
        public int UV { get; set; }
        public object Creditos { get; set; } // Usa 'object' si puede ser null u otro tipo en distintos casos
        public string PreReq1 { get; set; }
        public string PreReq2 { get; set; }
        public string PreReq3 { get; set; }
        public string PreReq4 { get; set; }
        public int Estado { get; set; } // Asumo que 1 = aprobado, 0 = no cursado o reprobado
        public double? Notafinal { get; set; } // Nullable porque no siempre hay nota
        public string CicloCursado { get; set; }
        public List<string> Camino { get; set; }
    }
}
