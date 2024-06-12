namespace BicardBackend.DTOs
{
    public class UserAppointment
    {
        public string DoctorPhoto { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpeciality { get; set;}
        public string AppointmentDate { get; set; }
        public bool IsConfirmed { get; set;}
    }
}
