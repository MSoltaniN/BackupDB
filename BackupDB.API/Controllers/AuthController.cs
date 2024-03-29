using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BackupDB.API.Data;
using BackupDB.API.Dtos;
using BackupDB.API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using System.DirectoryServices.AccountManagement;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace BackupDB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IBackUpRepository _repoBackup;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo,IBackUpRepository backupRepo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
            _repoBackup = backupRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("Username already exists");

            string domainName = System.Environment.UserDomainName;
            string domainUserName = System.Environment.UserName;
            bool isValid = false;
            //PrincipalContext pc = new PrincipalContext(ContextType.Domain, domainName, domainUserName, ContextOptions.SimpleBind.ToString());
            //isValid  = pc.ValidateCredentials( user , pass);
            using (var sshClient = new SshClient(userForRegisterDto.Server_IP , userForRegisterDto.Server_Username, userForRegisterDto.Server_Password))
            {
                //Accept Host key
                sshClient.HostKeyReceived += delegate (object sender, HostKeyEventArgs e)
                {
                    e.CanTrust = true;
                };
                try
                {
                    //Start the connection
                    sshClient.Connect();

                    sshClient.Disconnect();
                    isValid = true;
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Permission denied (password)")
                        isValid = false;
                }
            }

            if (isValid)
            {
                var userToCreate = new User
                {
                    Username = userForRegisterDto.Username,

                    ServerUsername = userForRegisterDto.Server_Username,
                    ServerPassword = userForRegisterDto.Server_Password,
                    ServerIP = userForRegisterDto.Server_IP
                };
                var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);
        
                // var Server = new Server
                // {
                //     Username = userForRegisterDto.Server_Username,
                //     Password = userForRegisterDto.Server_Password,
                //     IP = userForRegisterDto.Server_IP,
                //     UserId = createdUser.Id
                // };
                // var createdServer = await _repoBackup.Add(Server);

                return StatusCode(201);
            }
            else
                return BadRequest("نام کاربری یا کلمه عبور سرور میزبان صحیح نیست");

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.Username, userForLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}