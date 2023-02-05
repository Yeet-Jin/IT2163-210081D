using _210081D_Wong_Yee_Jin.Services;
using _210081D_Wong_Yee_Jin.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace _210081D_Wong_Yee_Jin.Pages
{
    [ValidateAntiForgeryToken]
    public class ResetPasswordModel : PageModel
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PasswordHistoryService _passwordHistoryService;
        private readonly AuditLogServices _auditLogServices;

        [BindProperty]
        public ResetPassword RPModel { get; set; }

        [BindProperty]
        public string UserId { get; set; }

        [BindProperty]
        public string Token { get; set; }

        public ResetPasswordModel(UserManager<ApplicationUser> userManager, PasswordHistoryService passwordHistoryService, AuditLogServices auditLogServices)
        {
            _userManager = userManager;
            _passwordHistoryService = passwordHistoryService;
            _auditLogServices = auditLogServices;
        }

        public IActionResult OnGet(string userId, string token)
        {
            UserId = userId;
            Token = HttpUtility.UrlDecode(token);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (UserId != null)
                {
                    var user = await _userManager.FindByEmailAsync(UserId);
                    if (user != null)
                    {
                        //RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                        //var salt = user.PasswordSalt;

                        //SHA512Managed hashing = new SHA512Managed();
                        //string pwdWithSalt = RPModel.Password + salt;
                        //byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(RPModel.Password));
                        //byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                        //var finalHash = Convert.ToBase64String(hashWithSalt);

                        // check if new password == last password
                        var checker = _passwordHistoryService.CheckPasswordHistory(user.Id, user.PasswordHash);
                        if (checker == "no problem!")
                        {
                            var result = await _userManager.ResetPasswordAsync(user, Token, RPModel.Password);
                            if (result.Succeeded)
                            {
                                _auditLogServices.LogActivity(user.Id, "Account password reset successful");
                                _passwordHistoryService.AddPassword(user.Id, user.PasswordHash);
                                return RedirectToPage("Login");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Cannot reset password. Send another email link.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "A registered email is required to reset password.");
                    }
                }
            }
            return Page();
        }
    }
}
