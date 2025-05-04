namespace SimuladorCUM.Models
{
    public class Estudiante
    {
        public int IdSede { get; set; }
        public string Sede { get; set; }
        public string Carnet { get; set; }
        public string Nombre { get; set; }
        public string IdCarrera { get; set; }
        public int IdPlan { get; set; }
        public string IdExpe { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string Carrera { get; set; }
        public string Plan { get; set; }
        public string Modalidad { get; set; }
        public bool ASU { get; set; }
        public bool PlanActivo { get; set; }
        public List<int> CiclosRegulares { get; set; }
        public List<int> CiclosElectivas { get; set; }
        public List<AsignaturaPensum> AsignaturasPensum { get; set; }
        public List<string> AsignaturasDesbloqueadasID { get; set; }
    }
}
