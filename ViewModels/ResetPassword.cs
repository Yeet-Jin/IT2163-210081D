using System.ComponentModel.DataAnnotations;

namespace _210081D_Wong_Yee_Jin.ViewModels
{
    public class ResetPassword
    {

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{12,}$",
            ErrorMessage = "Invalid password format")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

    }
}
