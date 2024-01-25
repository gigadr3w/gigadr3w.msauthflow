using gigadr3w.msauthflow.authenticator.iterator.Models;
using gigadr3w.msauthflow.common.Extensions;
using gigadr3w.msauthflow.dataaccess.Interfaces;
using gigadr3w.msauthflow.entities;
using System.Linq.Expressions;

namespace gigadr3w.msauthflow.authenticator.iterator.Services
{
    public interface ILoginService
    {
        Task<UserModel> Authenticate(UserModel model);
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
        public async Task<UserModel> Authenticate(UserModel model)
        {
            if(model.Password == null || model.Email == null) 
            {
                model.UnhautorizedMessage = "Email and password are mandatory fields";
                return model;
            }

            // Including Roles
            IQueryable<User?> users = await _users.Where(u => u.Email == model.Email, new List<Expression<Func<User, object>>> { u => u.Roles });
            
            if(users.Count() > 0) 
            {
                User? user = users.First();
                if (user == null)
                {
                    model.UnhautorizedMessage = $"User {model.Email} not found";
                }
                else 
                {
                    if (user.Password != model.Password.Hash256()) 
                    {
                        model.UnhautorizedMessage = "Password mismatch";
                    }
                    else
                    {
                        //valid user
                        model.Roles = user.Roles.Select(r => new RoleModel { Name = r.Name, Description = r.Description, EnabledService = r.EnabledService }).ToList();
                        model.IsAuthorized = true;
                    }
                }
                
            }
            else
            {
                model.UnhautorizedMessage = $"User {model.Email} not found";
            }
            return model;
        } 
    }
}
