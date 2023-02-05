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
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AuditLogServices _auditLogServices;
        private readonly AuthDbContext _authDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ILogger<IndexModel> logger, AuditLogServices auditLogServices, UserManager<ApplicationUser> userManager, AuthDbContext authDbContext)
        {
            _logger = logger;
            _auditLogServices = auditLogServices;
            _userManager = userManager;
            _authDbContext = authDbContext;
        }

        public void OnGet()
        {
            string userId = _userManager.GetUserId(User);
            ApplicationUser? currentUser = _authDbContext.Users.FirstOrDefault(x => x.Id.Equals(userId));
            _auditLogServices.LogActivity(currentUser.Id, "Current page: Index");
        }
    }
}
