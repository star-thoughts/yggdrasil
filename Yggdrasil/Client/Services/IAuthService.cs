using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Yggdrasil.Identity;

namespace Yggdrasil.Client.Services
{
    public interface IAuthService
    {
        Task DeleteUser(string userName, CancellationToken cancellationToken = default);
        Task<UserInfo> GetUser(string userName, CancellationToken cancellationToken = default);
        Task<GetUsersResponse> GetUsers(CancellationToken cancellationToken = default);
        Task LockUser(string userName, bool lockout, CancellationToken cancellationToken = default);
        Task<LoginResponse> Login(string userName, string password, CancellationToken cancellationToken = default);
        Task OverridePassword(string userName, string password, CancellationToken cancellationToken = default);
        Task OverridePassword(string userName, string originalPassword, string password, CancellationToken cancellationToken = default);
        Task<RegisterResult> Register(string userName, string password, CancellationToken cancellationToken = default);
        Task UpdateUserRoles(string userName, IEnumerable<string> roles, CancellationToken cancellationToken = default);
        Task VerifyUser(string userName, CancellationToken cancellationToken = default);
    }
}