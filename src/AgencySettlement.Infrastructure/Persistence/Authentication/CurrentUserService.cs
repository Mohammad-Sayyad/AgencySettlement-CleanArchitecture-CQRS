using AgencySettlement.Application.Abstractions.Persistence.UserRepository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Infrastructure.Persistence.Authentication
{
    public sealed class CurrentUserService
    : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor =
                httpContextAccessor;
        }

        public long? UserId
        {
            get
            {
                var value =
                    _httpContextAccessor
                        .HttpContext?
                        .User
                        .FindFirstValue(
                            ClaimTypes.NameIdentifier);

                return long.TryParse(
                    value,
                    out var id)
                    ? id
                    : null;
            }
        }

        public string? PhoneNumber
        {
            get
            {
                return _httpContextAccessor
                    .HttpContext?
                    .User
                    .FindFirstValue(
                        ClaimTypes.MobilePhone);
            }
        }
    }
}
