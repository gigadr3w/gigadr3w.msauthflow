using gigadr3w.msauthflow.authenticator.iterator.Models;
using gigadr3w.msauthflow.common.Extensions;
using gigadr3w.msauthflow.dataaccess.Interfaces;
using gigadr3w.msauthflow.entities;
using System.Linq.Expressions;

namespace gigadr3w.msauthflow.authenticator.iterator.Services
{
    public interface ILoginService
    {
        Task<UserModel> Authenticate(string userName, string password);
    }

    public class LoginService : ILoginService
    {
        private readonly IDataAccess<User> _users;

        public LoginService(IDataAccess<User> users)
            => _users = users;

        /// <summary>
        /// Throw unauthorized exception
        /// </summary>
        /// <param name="model">User model</param>
        /// <returns></returns>
        public async Task<UserModel> Authenticate(string userName, string password)
        {
            UserModel userModel = new () { Email = userName };

            if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password)) 
            {
                userModel.UnhautorizedMessage = "Email and password are mandatory fields";
                return userModel;
            }

            // Including Roles
            IQueryable<User?> users = await _users.Where(u => u.Email == userModel.Email, new List<Expression<Func<User, object>>> { u => u.Roles });
            
            if(users.Count() > 0) 
            {
                User? user = users.First();
                if (user == null)
                {
                    userModel.UnhautorizedMessage = $"User {userModel.Email} not found";
                }
                else 
                {
                    if (user.Password != password.Hash256()) 
                    {
                        userModel.UnhautorizedMessage = "Password mismatch";
                    }
                    else
                    {
                        //valid user
                        userModel.Id = user.Id;
                        userModel.Roles = user.Roles.Select(r => new RoleModel { Name = r.Name, Description = r.Description, EnabledService = r.EnabledService }).ToList();
                        userModel.IsAuthorized = true;
                    }
                }
                
            }
            else
            {
                userModel.UnhautorizedMessage = $"User {userModel.Email} not found";
            }
            return userModel;
        } 
    }
}
