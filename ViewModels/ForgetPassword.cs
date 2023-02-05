using System.ComponentModel.DataAnnotations;

namespace _210081D_Wong_Yee_Jin.ViewModels
{
    public class ForgetPassword
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
