using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WebApplication1.Data;
using WebApplication1.Data.Authentication;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Service.InheritanceService
{
    public class UserService
    {
        private readonly MyDbContext _context;
        private readonly AppSetting _appSettings;
        private readonly IUserRepository _userRepository;
        private readonly ISendMailService _sendMailService;

        public UserService(MyDbContext context, IUserRepository userRepository, IOptionsMonitor<AppSetting> optionsMonitor, ISendMailService sendMailService)
        {
            _context = context;
            _appSettings = optionsMonitor.CurrentValue;
            _userRepository = userRepository;
            _sendMailService = sendMailService;
        }

        public async Task<Response> Validate(LoginModel model)
        {
            var user = _context.Users.SingleOrDefault(p => p.UserName == model.UserName && model.Password == p.Password && model.Password != "None");
            if (user == null) //không đúng
            {
                return new Response
                {
                    resultCd = 1,
                    MessageCode = "C001"
                };
            }

            //cấp token
            var token = await GenerateToken(user);

            return new Response
            {
                resultCd = 0,
                MessageCode = "I000",
                Data = token
            };
        }

        public async Task<Response> CheckExist(String userName)
        {
            var user = _context.Users.SingleOrDefault(p => p.UserName == userName);
            if (user == null) //không đúng
            {
                return new Response
                {
                    resultCd = 0,
                    MessageCode = "I010"
                };
            }
            else
            {
                return new Response
                {
                    resultCd = 1,
                    MessageCode = "C010"
                };
            }
        }

        public async Task<Response> GoogleSignIn(UserModel model)
        {
            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(model.GoogleToken);

            var userExist = _context.Users.Count(p => p.Email == model.Email && p.Password == "None");

            if (payload == null)
            {
                return new Response
                {
                    resultCd = 1,
                    MessageCode = "C008"
                };
            }

            if (userExist < 1) //không đúng
            {
                var userInsert = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Password = "None",
                    PhoneNumber = model.PhoneNumber,
                    Role = UserRole.User,
                    Address = model.Address,
                    FullName = model.FullName,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                };
                _context.Users.Add(userInsert);
                await _context.SaveChangesAsync();
            }


            var user = _context.Users.FirstOrDefault(p => p.UserName == model.Email);
            var token = await GenerateToken(user);

            return new Response
            {
                resultCd = 0,
                MessageCode = "I000",
                Data = token
            };
        }


        public async Task<Response> SignIn(UserModel model)
        {
            var userExist = _context.Users.Count(p => p.UserName == model.UserName || p.Email == model.Email);
            if (userExist < 1) //không đúng
            {
                var userInsert = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Password = model.Password,
                    PhoneNumber = model.PhoneNumber,
                    Role = UserRole.New,
                    Address = model.Address,
                    FullName = model.FullName,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                };
                await _context.Users.AddAsync(userInsert);
                await _context.SaveChangesAsync();

                var signInToken = GenerateSignInToken(userInsert);

                var mailContent = new MailContent
                {
                    To = model.Email,
                    Subject = "Roxanne: Tạo tài khoản mới",
                    Body = "Xin chào " + model.FullName + ".\n " +
                    "Chúng tôi đã nhận được yêu cầu tạo tài khoản của bạn, vui lòng truy cập https://tourishapi20240305102130.azurewebsites.net/UserValidateSignIn?token="
                    + signInToken + " để xác nhận tài khoản."
                };

                var mailResult = await _sendMailService.SendMail(mailContent);

                if (mailResult.resultCd == 0)
                {
                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "I010",
                        Data = GenerateSignInToken(userInsert)
                    };
                }
                else return mailResult;               
            }
            else
            {
                return new Response
                {
                    resultCd = 1,
                    MessageCode = "C010"
                };
            }
        }

        public async Task<Response> Update(UserUpdateModel model, string bearer_token)
        {
            Response checkResult = checkIfTokenFormIsValid(bearer_token);
            if (checkResult.resultCd != 0) return checkResult;
            var tokenInverification = (ClaimsPrincipal)checkResult.Data;

            var role = tokenInverification.FindFirstValue("Role");
            var enumValue = (UserRole)Enum.Parse(typeof(UserRole), role);


            var user = _context.Users.SingleOrDefault(p => p.UserName == model.UserName && p.Role == enumValue);
            if (user == null)
            {
                return new Response
                {
                    resultCd = 1,
                    MessageCode = "C011",
                };
            }

            var isSelfUpdate = tokenInverification.FindFirstValue("Id") == user.Id.ToString();

            if (role == "AdminManager")
            {
                var response = await this._userRepository.UpdateInfo(UserRole.AdminManager, isSelfUpdate, model);
                return response;
            }

            if (role == "Admin")
            {
                var response = await this._userRepository.UpdateInfo(UserRole.Admin, isSelfUpdate, model);
                return response;
            }

            else if (role == "User")
            {
                var userName = tokenInverification.FindFirstValue("UserName");
                var updateModel = model;
                updateModel.UserName = userName;

                var response = await this._userRepository.UpdateInfo(UserRole.User, isSelfUpdate, updateModel);
                return response;
            }

            return new Response
            {
                resultCd = 1,
                MessageCode = "C011",
            };
        }

        public async Task<Response> UpdatePassword(UserUpdatePasswordModel model, string bearer_token)
        {
            Response checkResult = checkIfTokenFormIsValid(bearer_token);
            if (checkResult.resultCd != 0) return checkResult;
            var tokenInverification = (ClaimsPrincipal)checkResult.Data;

            var role = tokenInverification.FindFirstValue("Role");

            if (role == "User" || role == "Admin" || role == "AdminManager")
            {
                var userName = tokenInverification.FindFirstValue("UserName");
                var updateModel = model;
                updateModel.UserName = userName;

                var entity = await this._userRepository.UpdatePassword(updateModel);
                return entity;
            }

            return new Response
            {
                resultCd = 1,
                MessageCode = "C012",
            };
        }

        public Response GetUserList(string bearer_token, string? search, int type, string? sortBy, int page = 1, int pageSize = 5)
        {
            Response checkResult = checkIfTokenFormIsValid(bearer_token);
            if (checkResult.resultCd != 0) return checkResult;
            var tokenInverification = (ClaimsPrincipal)checkResult.Data;

            var role = tokenInverification.FindFirstValue("Role");

            if (role == "Admin")
            {
                if (type >= 2)
                {
                    return new Response
                    {
                        resultCd = 1,
                        MessageCode = "C015",
                        Data = null,
                        count = 0
                    };
                }

                var entityList = this._userRepository.GetAll(search, type, sortBy, page, pageSize);
                return entityList;
            }

            if (role == "AdminManager")
            {
                var entityList = this._userRepository.GetAll(search, type, sortBy, page, pageSize);
                return entityList;
            }
            else
            {
                return new Response
                {
                    resultCd = 1,
                    MessageCode = "C013",
                };
            }
        }

        public Response GetUser(Guid id, int type, string bearer_token)
        {
            Response checkResult = checkIfTokenFormIsValid(bearer_token);
            if (checkResult.resultCd != 0) return checkResult;
            var tokenInverification = (ClaimsPrincipal)checkResult.Data;

            var role = tokenInverification.FindFirstValue("Role");

            if (role == "Admin")
            {
                if (type >= 2) return new Response
                {
                    resultCd = 1,
                    MessageCode = "C015",
                };

                var entityList = this._userRepository.getById(id, type);
                return entityList;
            }

            if (role == "AdminManager")
            {
                var entityList = this._userRepository.getById(id, type);
                return entityList;
            }
            else
            {
                return new Response
                {
                    resultCd = 1,
                    MessageCode = "C014",
                };
            }
        }

        public Response SelfGetUser(string bearer_token)
        {
            Response checkResult = checkIfTokenFormIsValid(bearer_token);
            if (checkResult.resultCd != 0) return checkResult;
            var tokenInverification = (ClaimsPrincipal)checkResult.Data;

            var id = tokenInverification.FindFirstValue("Id");
            var role = tokenInverification.FindFirstValue("Role");
            var enumValue = (UserRole)Enum.Parse(typeof(UserRole), role);

            var entity = this._userRepository.getById(new Guid(id), (int)enumValue);
            return entity;
        }

        private async Task<TokenModel> GenerateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);

            var permissions = getPermission(user.Role);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserName", user.UserName),
                    new Claim("Id", user.Id.ToString()),
                    //roles
                    new Claim("Role", user.Role.ToString()),
                    new Claim("Permissions", permissions != null ? JsonSerializer.Serialize(permissions) : string.Empty,JsonClaimValueTypes.JsonArray)

                }),
                Issuer = _appSettings.Issuer,
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            // Save to database
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                UserId = user.Id,
                TokenDescription = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssueDate = DateTime.UtcNow,
                ExpiredDate = DateTime.UtcNow.AddHours(1),
            };

            await _context.RefreshTokens.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }

        public async Task<TokenModel> GenerateSignInToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);

            var permissions = getPermission(user.Role);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserName", user.UserName),
                    new Claim("Id", user.Id.ToString()),
                    new Claim("Purpose", "signIn"),
                    //roles
                    new Claim("Role", user.Role.ToString()),
                    new Claim("Permissions", permissions != null ? JsonSerializer.Serialize(permissions) : string.Empty,JsonClaimValueTypes.JsonArray)

                }),
                Issuer = _appSettings.Issuer,
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = null,
            };
        }

        public async Task<bool> validateSignInToken(string bearerToken)
        {
            var checkResult = checkIfTokenFormIsValid(bearerToken);

            if (checkResult.resultCd == 0)
            {
                var tokenInverification = (ClaimsPrincipal)checkResult.Data;
                var purpose = tokenInverification.FindFirstValue("Purpose");
                if (purpose == "signIn")
                {
                    var userName = tokenInverification.FindFirstValue("UserName");
                    var role = tokenInverification.FindFirstValue("Role");
                    var enumValue = (UserRole)Enum.Parse(typeof(UserRole), role);
                    var user = await _context.Users.SingleOrDefaultAsync(user => user.UserName == userName);
                    if (user != null)
                    {
                        if (enumValue == UserRole.Admin) user.Role = UserRole.AdminTemp;
                        else if (enumValue == UserRole.User) user.Role = UserRole.User;

                        await _context.SaveChangesAsync();
                        return true;
                    }
                }
            }
            return false;
        }

        private string[] getPermission(UserRole role)
        {
            var permissions = new List<string>();
            if (role == UserRole.Admin)
            {
                //BOOK policy
                permissions.Add(PolicyTerm.CREATE_TOURISH_PLAN);
                permissions.Add(PolicyTerm.UPDATE_TOURISH_PLAN);
                permissions.Add(PolicyTerm.DELETE_TOURISH_PLAN);

                permissions.Add(PolicyTerm.CREATE_TOURISH_CATEGORY);
                permissions.Add(PolicyTerm.UPDATE_TOURISH_CATEGORY);
                permissions.Add(PolicyTerm.DELETE_TOURISH_CATEGORY);

                permissions.Add(PolicyTerm.CREATE_TOURISH_COMMENT);
                permissions.Add(PolicyTerm.UPDATE_TOURISH_COMMENT);
                permissions.Add(PolicyTerm.DELETE_TOURISH_COMMENT);

                permissions.Add(PolicyTerm.CREATE_MOVING_CONTACT);
                permissions.Add(PolicyTerm.UPDATE_MOVING_CONTACT);
                permissions.Add(PolicyTerm.DELETE_MOVING_CONTACT);

                permissions.Add(PolicyTerm.CREATE_RESTHOUSE_CONTACT);
                permissions.Add(PolicyTerm.UPDATE_RESTHOUSE_CONTACT);
                permissions.Add(PolicyTerm.DELETE_RESTHOUSE_CONTACT);

                permissions.Add(PolicyTerm.CREATE_RESTAURANT);
                permissions.Add(PolicyTerm.UPDATE_RESTAURANT);
                permissions.Add(PolicyTerm.DELETE_RESTAURANT);

                permissions.Add(PolicyTerm.CREATE_NOTIFICATION);
                permissions.Add(PolicyTerm.UPDATE_NOTIFICATION);
                permissions.Add(PolicyTerm.DELETE_NOTIFICATION);

                permissions.Add(PolicyTerm.CREATE_RECEIPT);
                permissions.Add(PolicyTerm.UPDATE_RECEIPT);
                permissions.Add(PolicyTerm.DELETE_RECEIPT);

                permissions.Add(PolicyTerm.GET_USER_LIST);
                permissions.Add(PolicyTerm.SELF_GET_USER);
                permissions.Add(PolicyTerm.UPDATE_INFO_USER);
                permissions.Add(PolicyTerm.UPDATE_PASSWORD_USER);
            }

            if (role == UserRole.AdminManager)
            {
                //BOOK policy
                permissions.Add(PolicyTerm.CREATE_TOURISH_PLAN);
                permissions.Add(PolicyTerm.UPDATE_TOURISH_PLAN);
                permissions.Add(PolicyTerm.DELETE_TOURISH_PLAN);

                permissions.Add(PolicyTerm.CREATE_TOURISH_CATEGORY);
                permissions.Add(PolicyTerm.UPDATE_TOURISH_CATEGORY);
                permissions.Add(PolicyTerm.DELETE_TOURISH_CATEGORY);

                permissions.Add(PolicyTerm.CREATE_TOURISH_COMMENT);
                permissions.Add(PolicyTerm.UPDATE_TOURISH_COMMENT);
                permissions.Add(PolicyTerm.DELETE_TOURISH_COMMENT);

                permissions.Add(PolicyTerm.CREATE_MOVING_CONTACT);
                permissions.Add(PolicyTerm.UPDATE_MOVING_CONTACT);
                permissions.Add(PolicyTerm.DELETE_MOVING_CONTACT);

                permissions.Add(PolicyTerm.CREATE_RESTHOUSE_CONTACT);
                permissions.Add(PolicyTerm.UPDATE_RESTHOUSE_CONTACT);
                permissions.Add(PolicyTerm.DELETE_RESTHOUSE_CONTACT);

                permissions.Add(PolicyTerm.CREATE_RESTAURANT);
                permissions.Add(PolicyTerm.UPDATE_RESTAURANT);
                permissions.Add(PolicyTerm.DELETE_RESTAURANT);

                permissions.Add(PolicyTerm.CREATE_NOTIFICATION);
                permissions.Add(PolicyTerm.UPDATE_NOTIFICATION);
                permissions.Add(PolicyTerm.DELETE_NOTIFICATION);

                permissions.Add(PolicyTerm.CREATE_RECEIPT);
                permissions.Add(PolicyTerm.UPDATE_RECEIPT);
                permissions.Add(PolicyTerm.DELETE_RECEIPT);

                permissions.Add(PolicyTerm.GET_USER_LIST);
                permissions.Add(PolicyTerm.SELF_GET_USER);
                permissions.Add(PolicyTerm.UPDATE_INFO_USER);
                permissions.Add(PolicyTerm.UPDATE_PASSWORD_USER);
            }

            return permissions.ToArray();
        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }

        public async Task<Response> RenewToken(TokenModel tokenModel)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);

            var tokenValidateParam = new TokenValidationParameters
            {
                // Seft-supply token
                ValidateIssuer = true,
                ValidIssuer = _appSettings.Issuer,
                ValidateAudience = false,

                // Sign token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                ClockSkew = TimeSpan.Zero,

                // No check expired
                ValidateLifetime = false,
            };

            try
            {
                // Check 1: Check if accessToken's format is validated
                var tokenInverification = jwtTokenHandler.ValidateToken(tokenModel.AccessToken,
                    tokenValidateParam, out var validatedToken);

                // Check 2: Check algorithm
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);

                    if (!result)
                    {
                        return new Response
                        {
                            resultCd = 1,
                            MessageCode = "C002",
                            //// Wrong algorithm
                        };

                    }
                }

                // Check 3: Check expired
                var utcExpiredDate = long.Parse(tokenInverification.
                    Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)
                    .Value);

                var expiredDate = ConvertUnixTimeToDateTime(utcExpiredDate);
                if (expiredDate > DateTime.UtcNow)
                {
                    return new Response
                    {
                        resultCd = 1,
                        MessageCode = "C003",
                        // Not expired yet
                    };
                }

                // Check 4: Check expired
                var expiredDateExt = expiredDate.AddHours(1);
                if (expiredDateExt < DateTime.UtcNow)
                {
                    return new Response
                    {
                        resultCd = 1,
                        MessageCode = "C004",
                        // expired
                    };
                }

                // Check 5: Check if refreshToken exist in db
                var existToken = _context.RefreshTokens.FirstOrDefault(token =>
                token.TokenDescription == tokenModel.RefreshToken
               );

                if (existToken is null)
                {
                    return new Response
                    {
                        resultCd = 1,
                        MessageCode = "C005",
                        // Refresh token does not exist
                    };
                }

                // Check 6: Check if refreshToken is used or revoked
                if (existToken.IsUsed)
                {
                    return new Response
                    {
                        resultCd = 1,
                        MessageCode = "C006",
                        // Refresh token has been used
                    };
                }

                if (existToken.IsRevoked)
                {
                    return new Response
                    {
                        resultCd = 1,
                        MessageCode = "C007",
                        // Refresh token has been revoked
                    };
                }

                // Check 7: Check if accesstoken.id == JwtId in refreshtoken
                var jti = tokenInverification.Claims.FirstOrDefault(token => token.Type == JwtRegisteredClaimNames.Jti).Value;
                if (existToken.JwtId != jti)
                {
                    return new Response
                    {
                        resultCd = 1,
                        MessageCode = "C008",
                        // Token doesnt match
                    };
                }

                // Update token
                existToken.IsRevoked = true;
                existToken.IsUsed = true;
                _context.Update(existToken);
                await _context.SaveChangesAsync();

                // Create new token
                var user = await _context.Users.SingleOrDefaultAsync(user => user.Id == existToken.Id);
                var token = await GenerateToken(user);

                return new Response
                {
                    resultCd = 0,
                    MessageCode = "I001",
                    Data = token
                };


            }
            catch (Exception ex)
            {
                return new Response
                {
                    resultCd = 1,
                    MessageCode = "C009",
                    Error = ex.Message
                };
            }

        }

        public Response checkIfTokenFormIsValid(string bearer_token)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);

            var tokenValidateParam = new TokenValidationParameters
            {
                // Seft-supply token
                ValidateIssuer = true,
                ValidIssuer = _appSettings.Issuer,
                ValidateAudience = false,

                // Sign token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                ClockSkew = TimeSpan.Zero,

                // No check expired
                ValidateLifetime = false,
            };

            try
            {
                // Check 1: Check if accessToken's format is validated
                var tokenInverification = jwtTokenHandler.ValidateToken(bearer_token,
                    tokenValidateParam, out var validatedToken);

                // Check 2: Check algorithm
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);

                    if (!result)
                    {
                        return new Response
                        {
                            resultCd = 1,
                            MessageCode = "C002",
                            //// Wrong algorithm
                        };

                    }

                    if (tokenInverification == null)
                    {
                        return new Response
                        {
                            resultCd = 1,
                            MessageCode = "C008",
                        };
                    }
                }

                // Check 3: Check expired
                var utcExpiredDate = long.Parse(tokenInverification.
                    Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)
                    .Value);

                var expiredDate = ConvertUnixTimeToDateTime(utcExpiredDate);
                if (expiredDate > DateTime.UtcNow)
                {
                    return new Response
                    {
                        resultCd = 1,
                        MessageCode = "C003",
                        // Not expired yet
                    };
                }

                return new Response
                {
                    resultCd = 0,
                    Data = tokenInverification
                };

            }
            catch (Exception ex)
            {
                return new Response
                {
                    resultCd = 1,
                    MessageCode = "C009",
                    Error = ex.Message
                };
            }
        }

        private DateTime ConvertUnixTimeToDateTime(long utcExpiredDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpiredDate).ToUniversalTime();

            return dateTimeInterval;
        }
    }
}
