using PhotoGallery.Entities;
using PhotoGallery.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery.Infrastructure.Services.Abstract
{
    public interface IMembershipService
    {
        MembershipContext ValidateUser(string username, string password);

        User CreateUser(string username, string email, string password, int[] roles);

        User GetUser(int userId);

        List<Role> GetUserRoles(string username);
    }
}
