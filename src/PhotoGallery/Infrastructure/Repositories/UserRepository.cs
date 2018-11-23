using PhotoGallery.Entities;
using PhotoGallery.Infrastructure.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery.Infrastructure.Repositories
{
    public class UserRepository : EntityBaseRepository<User>, IUserRepository
    {
        private IRoleRepository roleReposistory;

        public UserRepository(PhotoGalleryContext context, IRoleRepository roleReposistory)
            : base(context)
        {
            this.roleReposistory = roleReposistory;
        }

        public User GetSingleByUsername(string username)
        {
            return this.GetSingle(x => x.Username == username);
        }
        

        public IEnumerable<Role> GetUserRoles(string username)
        {
            List<Role> roles = null;
            User user = this.GetSingle(u => u.Username == username, u => u.UserRoles);
            if (user != null)
            {
                roles = new List<Role>();
                foreach (var item in user.UserRoles)
                {
                    roles.Add(this.roleReposistory.GetSingle(item.RoleId));
                }
            }

            return roles;
        }
    }
}
