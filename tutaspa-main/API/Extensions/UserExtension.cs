using API.Models;

namespace API.Extensions
{
    public static class UserExtension
    {
        public static void CheckUserNull(this User user)
        {
            if (user == null )
            {
                throw new ArgumentNullException("Tài khoản không tồn tại");
            }
        }

        public static void CheckUserExitst(this User user)
        {
            if (user == null || user.IsDeleted)
            {
                throw new ArgumentNullException("Tài khoản không tồn tại");
            }
        }
    }
}
