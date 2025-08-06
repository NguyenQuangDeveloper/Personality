using Microsoft.AspNetCore.Http;
using System.Net;

namespace VSLibrary.UIComponent.Common.Services
{
    public class ClientIpService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientIpService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetClientIp()
        {
            var httpContext = _httpContextAccessor?.HttpContext;
            if (httpContext?.Connection?.RemoteIpAddress == null)
            {
                return "Unknown";  // ✅ `null` 방어 로직 강화
            }

            var remoteIp = httpContext.Connection.RemoteIpAddress;

            // ✅ IPv6이면서 `::1`인 경우 `127.0.0.1`로 변환
            if (remoteIp.Equals(IPAddress.IPv6Loopback))
            {
                return "127.0.0.1";
            }

            // ✅ IPv4 변환 (IPv6-mapped IPv4 주소인 경우)
            if (remoteIp.IsIPv4MappedToIPv6)
            {
                remoteIp = remoteIp.MapToIPv4();
            }

            return remoteIp.ToString();
        }
    }
}
