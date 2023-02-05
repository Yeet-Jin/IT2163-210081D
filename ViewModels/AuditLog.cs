namespace _210081D_Wong_Yee_Jin.ViewModels
{
    public class AuditLog
    {
        public int id { get; set; }
        public string UserId { get; set; }
        public string activityName { get; set; } = string.Empty;
        public DateTime activityTime { get; set; } = DateTime.Now;
    }
}
