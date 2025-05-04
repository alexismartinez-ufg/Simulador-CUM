namespace SimuladorCUM.Models
{
    public class ApiResponse
    {
        public int Status { get; set; }
        public string Msg { get; set; }
        public Estudiante Estudiante { get; set; }
    }
}
