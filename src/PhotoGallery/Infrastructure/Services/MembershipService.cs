using PhotoGallery.Infrastructure.Repositories.Abstract;
using PhotoGallery.Infrastructure.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhotoGallery.Entities;
using PhotoGallery.Infrastructure.Core;
using System.Security.Principal;

namespace PhotoGallery.Infrastructure.Services
{
    public class MembershipService : IMembershipService
    {
        #region Variables
        private readonly IUserRepository userRepository;

        private readonly IRoleRepository roleRepository;

        private readonly IUserRoleRepository userRoleRepository;

        private readonly IEncryptionService encryptionService;
        #endregion

        #region Constructor
        public MembershipService(IUserRepository userRepository, IRoleRepository roleRepository,
                                    IUserRoleRepository userRoleRepository, IEncryptionService encryptionService)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.userRoleRepository = userRoleRepository;
            this.encryptionService = encryptionService;
        }

        #endregion

        #region IMembershipService Implementation
        public MembershipContext ValidateUser(string username, string password)
        {
            var membershipCtx = new MembershipContext();
            var user = this.userRepository.GetSingleByUsername(username);
            if (user != null && isUserValid(user, password))
            {
                var userRoles = GetUserRoles(user.Username);
                membershipCtx.User = user;

                var identity = new GenericIdentity(user.Username);
                membershipCtx.Principal = new GenericPrincipal(identity, userRoles.Select(x => x.Name).ToArray());
            }

            return membershipCtx;
        }

        public User CreateUser(string username, string email, string password, int[] roles)
        {
            var existingUser = this.userRepository.GetSingleByUsername(username);
            if (existingUser != null)
            {
                throw new Exception("Username is already in use.");
            }

            var passwordSalt = this.encryptionService.CreateSalt();

            var user = new User
            {
                Username = username,
                Salt = passwordSalt,
                Email = email,
                IsLocked = false,
                HashedPassword = this.encryptionService.EncryptPassword(password, passwordSalt),
                DateCreated = DateTime.Now
            };

            this.userRepository.Add(user);
            this.userRepository.Commit();

            if (roles != null || roles.Length > 0)
            {
                foreach (var role in roles)
                {
                    addUserToRole(user, role);
                }
            }

            this.userRepository.Commit();
            return user;
        }

        public User GetUser(int userId)
        {
            return this.userRepository.GetSingle(userId);
        }

        public List<Role> GetUserRoles(string username)
        {
            List<Role> result = new List<Role>();
            var existingUser = this.userRepository.GetSingleByUsername(username);
            if (existingUser != null)
            {
                existingUser.UserRoles.ToList().ForEach(r => result.Add(r.Role));
            }

            return result.Distinct().ToList();
        }
        #endregion

        #region Helper methods
        private void addUserToRole(User user, int roleId)
        {
            var role = this.roleRepository.GetSingle(roleId);
            if (role == null)
            {
                throw new Exception("Role doesn't exist.");
            }

            var userRole = new UserRole
            {
                RoleId = role.Id,
                UserId = user.Id
            };

            this.userRoleRepository.Add(userRole);
            this.userRepository.Commit();
        }

        private bool isPasswordValid(User user, string password)
        {
            return string.Equals(this.encryptionService.EncryptPassword(password, user.Salt), user.HashedPassword);
        }

        private bool isUserValid(User user, string password)
        {
            if (isPasswordValid(user, password))
            {
                return !user.IsLocked;
            }

            return false;
        }
        #endregion
    }
}
