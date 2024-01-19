using gigadr3w.msauthflow.authenticator.iterator.Models;
using gigadr3w.msauthflow.common.Extensions;
using gigadr3w.msauthflow.dataaccess.Interfaces;
using gigadr3w.msauthflow.entities;

namespace gigadr3w.msauthflow.authenticator.iterator.Services
{
    public interface IAutehticatorService
    {
        Task<UserModel> Authenticate(UserModel model);
    }

    public class AutehticatorService : IAutehticatorService
    {
        private readonly IDataAccess<User> _users;

        public AutehticatorService(IDataAccess<User> users)
            => _users = users;

        /// <summary>
        /// Throw unauthorized exception
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<UserModel> Authenticate(UserModel model)
        {
            IQueryable<User?> users = await _users.Where(u => u.Email == model.Email);
            if(users.Count() > 0) 
            {
                User? user = users.First();
                if (user == null)
                {
                    model.UnhautorizedMessage = $"User {model.Email} not found";
                }
                else 
                {
                    if (user.Password != user.Password.Hash256()) 
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
