using System.ComponentModel.DataAnnotations;

namespace _210081D_Wong_Yee_Jin.ViewModels
{
	public class PasswordHistory
	{
        [Key]
        public int Id { get; set; }
		public string UserId { get; set; }
        public string Password { get; set; } = string.Empty;
        public DateTime TimeOfChange { get; set; } = DateTime.Now;
    }
}
