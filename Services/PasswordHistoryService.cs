using _210081D_Wong_Yee_Jin.Model;
using _210081D_Wong_Yee_Jin.ViewModels;

namespace _210081D_Wong_Yee_Jin.Services
{
	public class PasswordHistoryService
	{

        private readonly AuthDbContext _authDbContext;

        public PasswordHistoryService(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        public void AddPassword(string userId, string password)
        {
            ApplicationUser? currentUser = _authDbContext.Users.FirstOrDefault(x => x.Id.Equals(userId));
            if (currentUser != null)
            {
                var onePassword = new PasswordHistory
                {
                    UserId = userId,
                    Password = password
                };
                _authDbContext.Add(onePassword);
            }
            else
            {
                var onePassword = new PasswordHistory
                {
                    UserId = "",
                    Password = password
                };
                _authDbContext.Add(onePassword);
            }
            _authDbContext.SaveChanges();
        }

        public string CheckPasswordHistory(string userId, string password)
        {
            ApplicationUser? currentUser = _authDbContext.Users.FirstOrDefault(x => x.Id.Equals(userId));
            if (currentUser != null)
            {
                var last_2_passwords = _authDbContext.PasswordHistoryDB.Where(x => x.UserId.Equals(userId)).OrderByDescending(x => x.Id).ToList();
                if (last_2_passwords != null)
                {
                    if (last_2_passwords.Count < 2)
                    {
                        for (int i = 0; i < last_2_passwords.Count; i++)
                        {
                            if (password == last_2_passwords[i].Password)
                            {
                                return "new password found in last 2 passwords";
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            if (password == last_2_passwords[i].Password)
                            {
                                return "new password found in last 2 passwords";
                            }
                        }
                    }
                    return "no problem!";
                }
                else
                {
                    return "issue retrieving passwords";
                }
            }
            else
            {
                return "invalid user";
            }
        }

        public int ExpiredPassword(string userId, string password)
        {
            ApplicationUser? currentUser = _authDbContext.Users.FirstOrDefault(x => x.Id.Equals(userId));
            if (currentUser != null)
            {
                var currentPwd_timeOfChange = _authDbContext.PasswordHistoryDB.Where(x => x.UserId.Equals(currentUser.Id)).OrderBy(x => x.Id).LastOrDefault(x => x.Password.Equals(password));
                if (currentPwd_timeOfChange != null)
                {
                    if (currentPwd_timeOfChange.TimeOfChange.AddMinutes(10) >= DateTime.Now)
                    {
                        // Password age <= 10
                        return 5;
                    }
                    else if (currentPwd_timeOfChange.TimeOfChange.AddMinutes(30) <= DateTime.Now)
                    {
                        // Password age >= 30 minutes (expired)
                        return 1;
                    }
                    else
                    {
                        // 10 min < Password age < 30 min
                        return 2;
                    }
                }
                else
                {
                    // Issue with getting current password's TimeOfChange
                    return 3;
                }
            }
            else
            {
                // User cannot be found
                return 4;
            }
        }
    }
}
