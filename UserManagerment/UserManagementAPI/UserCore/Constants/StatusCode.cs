using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCore.Constants
{
    public class StatusCode
    {
        public const int Success = 200;
        public const int Created = 201;
        public const int NoContent = 204;
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int Forbidden = 403;
        public const int NotFound = 404;
        public const int Conflict = 409;
        public const int InternalServerError = 500;
        public const int ServiceUnavailable = 503;
        public const int GatewayTimeout = 504;
        public const int TooManyRequests = 429;

        public const string UserNotFound = "User not found";
        public const string RefreshTokenNotFound = "RefreshToken not found";
        public const string RefreshTokenExpired = "RefreshToken expired";
        public const string InvalidCredentials = "Invalid credentials";
        public const string RegisterFailed = "Register Failed";
    }
}
