namespace TimePunchService.Models
{
    public enum PunchType
    {
        In = 1,
        Out = 2,
        Lunch = 3,
        Transfer = 4
    }

    public class TimePunch
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Timestamp { get; set; }
        public PunchType PunchType { get; set; }
        public string? Notes { get; set; }
        public Employee? Employee { get; set; }
    }
}