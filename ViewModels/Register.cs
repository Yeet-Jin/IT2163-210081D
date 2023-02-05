using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace _210081D_Wong_Yee_Jin.ViewModels
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string NRIC { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; } = new DateTime(DateTime.Now.Year, 1, 1);

        [Required]
        [DataType(DataType.Text)]
        public string ResumePath { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string WhoamI { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string PasswordSalt { get; set; }
    }

    public class Register
    {
        [Required]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required, MaxLength(1)]
        [DataType(DataType.Text)]
        public string Gender { get; set; }

        [Required, RegularExpression(@"^[STFG]\d{7}[A-Z]$", ErrorMessage = "Invalid NRIC."), MaxLength(9)]
        [DataType(DataType.Text)]
        public string NRIC { get; set; }

        [Key]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required, RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{12,}$", 
            ErrorMessage = "Invalid password format")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; } = new DateTime(DateTime.Now.Year, 1, 1);

        public IFormFile Resume { get; set; }

        [Required, MaxLength(100)]
        [DataType(DataType.Text)]
        public string WhoamI { get; set; }
    }
}
