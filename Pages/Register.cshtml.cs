using _210081D_Wong_Yee_Jin.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using _210081D_Wong_Yee_Jin.Services;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography.Xml;
using _210081D_Wong_Yee_Jin.Model;

namespace _210081D_Wong_Yee_Jin.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly AuditLogServices _auditLogServices;
        private readonly PasswordHistoryService _passwordHistoryService;
        private IWebHostEnvironment _environment;
        private readonly AuthDbContext _authDbContext;

        [BindProperty]
        public Register RMemberModel { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment environment, RoleManager<IdentityRole> roleManager, AuditLogServices auditLogServices, AuthDbContext authDbContext, PasswordHistoryService passwordHistoryService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _environment = environment;
            this.roleManager = roleManager;
            _auditLogServices = auditLogServices;
            _authDbContext = authDbContext;
            _passwordHistoryService = passwordHistoryService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (Path.GetExtension(RMemberModel.Resume.FileName) != ".docx" && Path.GetExtension(RMemberModel.Resume.FileName) != ".pdf")
                {
                    ModelState.AddModelError("Resume", "Resume must be in .docx or .pdf format.");
                    return Page();
                }

                // Data Protection Provider
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");

                // Encode
                var encodedFirstName = HttpUtility.HtmlEncode(RMemberModel.FirstName);
                var encodedLastName = HttpUtility.HtmlEncode(RMemberModel.LastName);
                var encodedGender = HttpUtility.HtmlEncode(RMemberModel.Gender);
                var encodedNRIC = HttpUtility.HtmlEncode(RMemberModel.NRIC);
                var encodedWhoAmI = HttpUtility.HtmlEncode(RMemberModel.WhoamI);

                // Resume
                var docFile = Guid.NewGuid() + Path.GetExtension(RMemberModel.Resume.FileName);
                var file = Path.Combine(_environment.ContentRootPath, "wwwroot\\resumes", docFile);
                using var fileStream = new FileStream(file, FileMode.Create);
                await RMemberModel.Resume.CopyToAsync(fileStream);
                var formattedResume = "/resumes/" + docFile;

                // Password + Salt hashing
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];
                rng.GetBytes(saltByte);
                var salt = Convert.ToBase64String(saltByte);

                //SHA512Managed hashing = new SHA512Managed();
                //string pwdWithSalt = RMemberModel.Password + salt;
                //byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(RMemberModel.Password));
                //byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                //var finalHash = Convert.ToBase64String(hashWithSalt);

                //RijndaelManaged cipher = new RijndaelManaged();
                //cipher.GenerateKey();
                //var Key = cipher.Key;
                //var IV = cipher.IV;

                // create new ApplicationUsers
                var member = new ApplicationUser()
                {
                    UserName = RMemberModel.Email,
                    Email = RMemberModel.Email,
                    FirstName = protector.Protect(encodedFirstName),
                    LastName = protector.Protect(encodedLastName),
                    Gender = protector.Protect(encodedGender),
                    NRIC = protector.Protect(encodedNRIC),
                    DateOfBirth = RMemberModel.DateOfBirth,
                    ResumePath = formattedResume,
                    WhoamI = protector.Protect(encodedWhoAmI),
                    PasswordSalt = salt
                };

                //Create the Admin role if NOT exist
                IdentityRole role = await roleManager.FindByIdAsync("Admin");
                if (role == null)
                {
                    IdentityResult result2 = await roleManager.CreateAsync(new IdentityRole("Admin"));
                    if (!result2.Succeeded)
                    {
                        ModelState.AddModelError("", "Unable to create account.");
                    }
                }

                var result = await userManager.CreateAsync(member, RMemberModel.Password);

                if (result.Succeeded)
                {
                    result = await userManager.AddToRoleAsync(member, "Admin");
                    await signInManager.SignInAsync(member, false);
                    ApplicationUser? currentUser = _authDbContext.Users.FirstOrDefault(x => x.Id.Equals(member.Id));
                    if (currentUser != null)
                    {
                        _auditLogServices.LogActivity(currentUser.Id, "Successfully registered an account.");
                        _passwordHistoryService.AddPassword(currentUser.Id, currentUser.PasswordHash);
                        return RedirectToPage("Index");
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }
    }
}
