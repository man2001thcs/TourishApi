﻿using Microsoft.AspNetCore.Mvc;
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

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly AppSetting _appSettings;

        public UserController(MyDbContext context, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _context = context;
            _appSettings = optionsMonitor.CurrentValue;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Validate(LoginModel model)
        {
            var user = _context.Users.SingleOrDefault(p => p.UserName == model.UserName && model.Password == p.Password);
            if (user == null) //không đúng
            {
                return Ok(new Response
                {
                    resultCd = 1,
                    MessageCode = "C001"
                });
            }

            //cấp token
            var token = await GenerateToken(user);

            return Ok(new Response
            {
                resultCd = 0,
                MessageCode = "I000",
                Data = token
            });
        }

        [HttpPost("CheckExist")]
        public async Task<IActionResult> CheckExist(String userName )
        {
            var user = _context.Users.SingleOrDefault(p => p.UserName == userName);
            if (user == null) //không đúng
            {
                return Ok(new Response
                {
                    resultCd = 0,
                    MessageCode = "I010"
                });
            }
            else
            {
                return Ok(new Response
                {
                    resultCd = 1,
                    MessageCode = "C010"
                });
            }     
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(UserModel model)
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
                    Address = model.Address,
                    FullName = model.FullName,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                };
                _context.Users.Add(userInsert);
                _context.SaveChanges();

                return Ok(new Response
                {
                    resultCd = 0,
                    MessageCode = "I010",
                });
            }
            else
            {
                return Ok(new Response
                {
                    resultCd = 1,
                    MessageCode = "C010"
                });
            }

            
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

                Expires = DateTime.UtcNow.AddMinutes(20),
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

        private string[] getPermission(UserRole role)
        {
            var permissions = new List<string>();
            if (role == UserRole.Admin)
            {
                //BOOK policy
                permissions.Add(PolicyTerm.CREATE_TOURISH_PLAN);
                permissions.Add(PolicyTerm.UPDATE_TOURISH_PLAN);
                permissions.Add(PolicyTerm.DELETE_TOURISH_PLAN);

                permissions.Add(PolicyTerm.CREATE_PASSENGER_CAR);
                permissions.Add(PolicyTerm.UPDATE_PASSENGER_CAR);
                permissions.Add(PolicyTerm.DELETE_PASSENGER_CAR);


                permissions.Add(PolicyTerm.CREATE_AIRPLANE);
                permissions.Add(PolicyTerm.UPDATE_AIRPLANE);
                permissions.Add(PolicyTerm.DELETE_AIRPLANE);

                permissions.Add(PolicyTerm.CREATE_HOTEL);
                permissions.Add(PolicyTerm.UPDATE_HOTEL);
                permissions.Add(PolicyTerm.DELETE_HOTEL);

                permissions.Add(PolicyTerm.CREATE_HOME_STAY);
                permissions.Add(PolicyTerm.UPDATE_HOME_STAY);
                permissions.Add(PolicyTerm.DELETE_HOME_STAY);

                permissions.Add(PolicyTerm.CREATE_RESTAURANT);
                permissions.Add(PolicyTerm.UPDATE_RESTAURANT);
                permissions.Add(PolicyTerm.DELETE_RESTAURANT);

                permissions.Add(PolicyTerm.CREATE_RECEIPT);
                permissions.Add(PolicyTerm.UPDATE_RECEIPT);
                permissions.Add(PolicyTerm.DELETE_RECEIPT);
            }

            if (role == UserRole.Staff)
            {
                //BOOK policy
                permissions.Add(PolicyTerm.CREATE_TOURISH_PLAN);
                permissions.Add(PolicyTerm.UPDATE_TOURISH_PLAN);
                permissions.Add(PolicyTerm.DELETE_TOURISH_PLAN);

                permissions.Add(PolicyTerm.CREATE_PASSENGER_CAR);
                permissions.Add(PolicyTerm.UPDATE_PASSENGER_CAR);
                permissions.Add(PolicyTerm.DELETE_PASSENGER_CAR);


                permissions.Add(PolicyTerm.CREATE_AIRPLANE);
                permissions.Add(PolicyTerm.UPDATE_AIRPLANE);
                permissions.Add(PolicyTerm.DELETE_AIRPLANE);

                permissions.Add(PolicyTerm.CREATE_HOTEL);
                permissions.Add(PolicyTerm.UPDATE_HOTEL);
                permissions.Add(PolicyTerm.DELETE_HOTEL);

                permissions.Add(PolicyTerm.CREATE_HOME_STAY);
                permissions.Add(PolicyTerm.UPDATE_HOME_STAY);
                permissions.Add(PolicyTerm.DELETE_HOME_STAY);

                permissions.Add(PolicyTerm.CREATE_RESTAURANT);
                permissions.Add(PolicyTerm.UPDATE_RESTAURANT);
                permissions.Add(PolicyTerm.DELETE_RESTAURANT);

                permissions.Add(PolicyTerm.CREATE_RECEIPT);
                permissions.Add(PolicyTerm.UPDATE_RECEIPT);
                permissions.Add(PolicyTerm.DELETE_RECEIPT);
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

        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(TokenModel tokenModel)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);

            var tokenValidateParam = new TokenValidationParameters
            {
                // Seft-supply token
                ValidateIssuer = false,
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
                        return Ok(new Response
                        {
                            resultCd = 1,
                            MessageCode = "C002",
                            //// Wrong algorithm
                        });

                    }
                }

                // Check 3: Check expired
                var utcExpiredDate = long.Parse(tokenInverification.
                    Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)
                    .Value);

                var expiredDate = ConvertUnixTimeToDateTime(utcExpiredDate);
                if (expiredDate > DateTime.UtcNow)
                {
                    return Ok(new Response
                    {
                        resultCd = 1,
                        MessageCode = "C003",
                        // Not expired yet
                    });
                }

                // Check 4: Check expired
                var expiredDateExt = expiredDate.AddMinutes(40);
                if (expiredDateExt < DateTime.UtcNow)
                {
                    return Ok(new Response
                    {
                        resultCd = 1,
                        MessageCode = "C004",
                        // expired
                    });
                }

                // Check 5: Check if refreshToken exist in db
                var existToken = _context.RefreshTokens.FirstOrDefault(token =>
                token.TokenDescription == tokenModel.RefreshToken
               );

                if (existToken is null)
                {
                    return Ok(new Response
                    {
                        resultCd = 1,
                        MessageCode = "C005",
                        // Refresh token does not exist
                    });
                }

                // Check 6: Check if refreshToken is used or revoked
                if (existToken.IsUsed)
                {
                    return Ok(new Response
                    {
                        resultCd = 1,
                        MessageCode = "C006",
                        // Refresh token has been used
                    });
                }

                if (existToken.IsRevoked)
                {
                    return Ok(new Response
                    {
                        resultCd = 1,
                        MessageCode = "C007",
                        // Refresh token has been revoked
                    });
                }

                // Check 7: Check if accesstoken.id == JwtId in refreshtoken
                var jti = tokenInverification.Claims.FirstOrDefault(token => token.Type == JwtRegisteredClaimNames.Jti).Value;
                if (existToken.JwtId != jti)
                {
                    return Ok(new Response
                    {
                        resultCd = 1,
                        MessageCode = "C008",
                        // Token doesnt match
                    });
                }

                // Update token
                existToken.IsRevoked = true;
                existToken.IsUsed = true;
                _context.Update(existToken);
                await _context.SaveChangesAsync();

                // Create new token
                var user = await _context.Users.SingleOrDefaultAsync(user => user.Id == existToken.Id);
                var token = await GenerateToken(user);

                return Ok(new Response
                {
                    resultCd = 0,
                    MessageCode = "I001",
                    Data = token
                });


            }
            catch (Exception ex)
            {
                return BadRequest(new Response
                {
                    resultCd = 1,
                    MessageCode = "C009",
                    Error = ex.Message
                });
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
