using MyFace.Models.Database;

namespace MyFace.Models.Request
{
    public class UpdateUserRoleRequest
    {
        public UserType Role { get; set; }
    }
}