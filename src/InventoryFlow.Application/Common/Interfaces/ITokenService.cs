using System;
using System.Collections.Generic;

namespace InventoryFlow.Application.Common.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(Guid userId, string email, IList<string> roles);
        string GenerateRefreshToken();
    }
}
