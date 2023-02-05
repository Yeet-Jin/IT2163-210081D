using _210081D_Wong_Yee_Jin.Services;
using _210081D_Wong_Yee_Jin.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;
using System.Web;

namespace _210081D_Wong_Yee_Jin.Pages
{
    public class ForgetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PasswordHistoryService _passwordHistoryService;

        [BindProperty]
        public ForgetPassword FPModel { get; set; }

        public string UserId { get; set; }

        public string user_email { get; set; }

        public List<ApplicationUser> Users { get; set; }

        public ForgetPasswordModel(UserManager<ApplicationUser> userManager, PasswordHistoryService passwordHistoryService)
        {
            _userManager = userManager;
            _passwordHistoryService = passwordHistoryService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(FPModel.Email);
                if (user != null)
                {
                    var checking = _passwordHistoryService.ExpiredPassword(user.Id, user.PasswordHash);
                    if (checking == 5)
                    {
                        ModelState.AddModelError("", "You just added/changed your password. Wait for a moment before you may reset your password.");
                        return Page();
                    }

                    else if (checking == 3 || checking == 4)
                    {
                        ModelState.AddModelError("", "A registered email is required for the link to be sent.");
                        return Page();
                    }

                    var token = HttpUtility.UrlEncode(await _userManager.GeneratePasswordResetTokenAsync(user));
                    string linkToResetPassword = "https://localhost:7012/ResetPassword?userId={0}&token={1}";
                    linkToResetPassword = string.Format(linkToResetPassword, user.Email, token);
                    MailMessage mail = new MailMessage();
                    mail.To.Add(FPModel.Email.ToString().Trim());
                    mail.From = new MailAddress("yeetusfetus845@gmail.com");
                    mail.Subject = "Reset password email";
                    mail.Body = "<p>Reset your password here:<br/>" + linkToResetPassword + "</p>";
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Credentials = new System.Net.NetworkCredential("yeetusfetus845@gmail.com", "vwbulsyuiamfggpn");
                    smtp.Send(mail);
                    Console.WriteLine(linkToResetPassword);
                    return RedirectToPage("EmailSentConfirmed");
                }
                else
                {
                    ModelState.AddModelError("", "A registered email is required for the link to be sent.");
                }
            }
            return Page();
        }
    }
}
