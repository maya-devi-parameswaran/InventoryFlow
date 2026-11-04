using InventoryFlow.Application.Common.Interfaces;
using System.Security.Claims;

namespace InventoryFlow.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _accessor;

        public CurrentUserService(IHttpContextAccessor accessor) => _accessor = accessor;

        public Guid? UserId
        {
            get 
            {
                var user = _accessor.HttpContext?.User;
                // Depending on JWT handler settings, "sub" may or may not be remapped, so check both
                var value = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? user?.FindFirstValue("sub");
                return Guid.TryParse(value, out var id) ? id : null;
            }
        }
    }
}
