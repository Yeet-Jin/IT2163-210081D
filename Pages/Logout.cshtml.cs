using _210081D_Wong_Yee_Jin.Model;
using _210081D_Wong_Yee_Jin.Services;
using _210081D_Wong_Yee_Jin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _210081D_Wong_Yee_Jin.Pages
{
    [Authorize(Roles = "Admin")]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly AuditLogServices _auditLogServices;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthDbContext _authDbContext;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, AuditLogServices auditLogServices, UserManager<ApplicationUser> userManager, AuthDbContext authDbContext)
        {
            this.signInManager = signInManager;
            this._auditLogServices = auditLogServices;
            this._userManager = userManager;
            this._authDbContext = authDbContext;
        }

        public void OnGet()
        {
            string userId = _userManager.GetUserId(User);
            ApplicationUser? currentUser = _authDbContext.Users.FirstOrDefault(x => x.Id.Equals(userId));
            _auditLogServices.LogActivity(currentUser.Id, "Current page: Logout");
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            string userId = _userManager.GetUserId(User);
            ApplicationUser? currentUser = _authDbContext.Users.FirstOrDefault(x => x.Id.Equals(userId));
            _auditLogServices.LogActivity(currentUser.Id, "Account log out successful");
            Response.Cookies.Delete("MyCookieAuth");
            await signInManager.SignOutAsync();
            return RedirectToPage("Login");
        }

        public async Task<IActionResult> OnPostDontLogoutAsync()
        {
            string userId = _userManager.GetUserId(User);
            ApplicationUser? currentUser = _authDbContext.Users.FirstOrDefault(x => x.Id.Equals(userId));
            _auditLogServices.LogActivity(currentUser.Id, "Account log out unsuccessful");
            return RedirectToPage("Index");
        }
    }
}
