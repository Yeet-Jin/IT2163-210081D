using _210081D_Wong_Yee_Jin.Model;
using _210081D_Wong_Yee_Jin.ViewModels;
using _210081D_Wong_Yee_Jin.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using AspNetCore.ReCaptcha;

namespace _210081D_Wong_Yee_Jin.Pages
{
    [ValidateReCaptcha]
    [ValidateAntiForgeryToken]
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Login LMemberModel { get; set; }

        private readonly SignInManager<ApplicationUser> _signInManager;
        private UserManager<ApplicationUser> _userManager { get; }
        private readonly AuthDbContext _authDbContext;
        private readonly AuditLogServices _auditLogServices;
        private readonly PasswordHistoryService _passwordHistoryService;

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AuthDbContext authDbContext, AuditLogServices auditLogServices, PasswordHistoryService passwordHistoryService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            this._authDbContext = authDbContext;
            _auditLogServices = auditLogServices;
            _passwordHistoryService = passwordHistoryService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl)
        {
            var userEmail = "";
            var userSalt = "";
            var userPwd = "";
            var confirmPwd = "";
            var userId = "";


            //if (ModelState.IsValid)
            //{
            if (LMemberModel.Email == null || LMemberModel.Password == null)
            {
                ModelState.AddModelError("", "");
                return Page();
            }
            ApplicationUser signedUser = await _userManager.FindByEmailAsync(LMemberModel.Email);
            if (signedUser != null)
            {
                if (_passwordHistoryService.ExpiredPassword(signedUser.Id, signedUser.PasswordHash) == 1)
                {
                    return RedirectToPage("ForgetPassword");
                }
                userEmail = signedUser.Email;
                userSalt = signedUser.PasswordSalt;
                userPwd = signedUser.PasswordHash;
                userId = signedUser.Id;
                SHA512Managed hashing = new SHA512Managed();
                string pwdWithSalt = LMemberModel.Password + userSalt;
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                confirmPwd = Convert.ToBase64String(hashWithSalt);
            }
            else
            {
                userEmail = LMemberModel.Email;
                confirmPwd= LMemberModel.Password;
                userId = _userManager.GetUserId(User);
            }

            var identityResult = await _signInManager.PasswordSignInAsync(userEmail, LMemberModel.Password, LMemberModel.RememberMe, true);
            if (identityResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Your account is locked out. Kindly wait for 20 minutes and try again");
                _auditLogServices.LogActivity("", "Account locked out.");
                return Page();
            }
            if (identityResult.Succeeded)
            {

                //Create the security context
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, "c@c.com"),
                    new Claim(ClaimTypes.Email, "c@c.com"),
                    new Claim("Department", "HR")
                };

                var i = new ClaimsIdentity(claims, "MyCookieAuth");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(i);
                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                ApplicationUser? currentUser = _authDbContext.Users.FirstOrDefault(x => x.Id.Equals(userId));
                if (currentUser != null)
                {
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    var salt = currentUser.PasswordSalt;

                    SHA512Managed hashing = new SHA512Managed();
                    string pwdWithSalt = LMemberModel.Password + salt;
                    byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(LMemberModel.Password));
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    var finalHash = Convert.ToBase64String(hashWithSalt);

                    var checking = _passwordHistoryService.ExpiredPassword(currentUser.Id, currentUser.PasswordHash);
                    if (checking == 2 || checking == 5)
                    {
                        _auditLogServices.LogActivity(currentUser.Id, "Successfully logged into web app");
                        return RedirectToPage("Index");
                    }
                }
            }
            ModelState.AddModelError("", "Username or Password incorrect");
            return Page();
            //}
            //return Page();
        }
    }
}
