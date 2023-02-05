using _210081D_Wong_Yee_Jin.Model;
using _210081D_Wong_Yee_Jin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using _210081D_Wong_Yee_Jin.Services;
using Microsoft.AspNetCore.DataProtection;
using System.Web;

namespace _210081D_Wong_Yee_Jin.Pages
{
    [Authorize(Roles = "Admin")]
    public class UserDetailsModel : PageModel
    {
        [BindProperty]
        public ApplicationUser HPModel { get; set; }

        private readonly AuthDbContext _authDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuditLogServices _auditLogServices;

        public UserDetailsModel(AuthDbContext authDbContext, UserManager<ApplicationUser> userManager, AuditLogServices auditLogServices)
        {
            this._authDbContext = authDbContext;
            this._userManager = userManager;
            this._auditLogServices = auditLogServices;
        }
        public IActionResult OnGet()
        {
            
            var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
            var protector = dataProtectionProvider.CreateProtector("MySecretKey");

            string userId = _userManager.GetUserId(User);
            ApplicationUser? currentUser = _authDbContext.Users.FirstOrDefault(x => x.Id.Equals(userId));
            if (currentUser != null)
            {
                var firstName = currentUser.FirstName;
                var lastName = currentUser.LastName;
                var gender = currentUser.Gender;
                var NRIC = currentUser.NRIC;
                var whoamI = currentUser.WhoamI;

                HPModel = new ApplicationUser
                {
                    Email = currentUser.Email,
                    FirstName = HttpUtility.HtmlDecode(protector.Unprotect(firstName)),
                    LastName = HttpUtility.HtmlDecode(protector.Unprotect(lastName)),
                    Gender = HttpUtility.HtmlDecode(protector.Unprotect(gender)),
                    NRIC = HttpUtility.HtmlDecode(protector.Unprotect(NRIC)),
                    DateOfBirth = currentUser.DateOfBirth,
                    ResumePath = currentUser.ResumePath,
                    WhoamI = HttpUtility.HtmlDecode(protector.Unprotect(whoamI))
                };

                _auditLogServices.LogActivity(currentUser.Id, "Current page: User Details");

                return Page();
            }
            else
            {
                return Redirect("/Login");
            }
        }
    }
}
