using _210081D_Wong_Yee_Jin.Model;
using _210081D_Wong_Yee_Jin.ViewModels;

namespace _210081D_Wong_Yee_Jin.Services
{
    public class AuditLogServices
    {
        private readonly AuthDbContext _authDbContext;

        public AuditLogServices(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        public void LogActivity(string UserId, string activityName)
        {
            ApplicationUser? currentUser = _authDbContext.Users.FirstOrDefault(x => x.Id.Equals(UserId));
            if (currentUser != null)
            {
                var oneActivity = new AuditLog
                {
                    UserId = currentUser.Id,
                    activityName = activityName
                };
                _authDbContext.Add(oneActivity);
            }
            else
            {
                var oneActivity = new AuditLog
                {
                    UserId = "",
                    activityName = activityName
                };
                _authDbContext.Add(oneActivity);
            }
            _authDbContext.SaveChanges();
        }
    }
}
