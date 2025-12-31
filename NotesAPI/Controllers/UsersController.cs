
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NotesAPI_Business;
using NotesAPI_Data;
using NotesAPI_Security;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;

namespace NotesAPI.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        
        [HttpGet("All", Name = "GetAllUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<UserDTO>> GetAllUsers()
        {

            List<UserDTO> usersList = NotesAPI_Business.clsUser.GetAllUsers();

            if (usersList.Count == 0)
            {
                return NotFound("NotFound");
            }
            return Ok(usersList);

        }


        [HttpGet("ById/{id}", Name = "GetUserById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserDTO> GetUserById(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }


            NotesAPI_Business.clsUser user = NotesAPI_Business.clsUser.FindUserByID(id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");

            }
            UserDTO uDTO = user.DTO;
            return Ok(uDTO);

        }


        [HttpGet("ByUserName/{UserName}", Name = "GetUserByUserName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserDTO> GetUserByUserName(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                return BadRequest($"Not accepted ID {UserName}");
            }

            NotesAPI_Business.clsUser user = NotesAPI_Business.clsUser.FindUserByUserName(UserName);
            if (user == null)
            {
                return NotFound($"User with UserName {UserName} not found.");
            }
            UserDTO uDTO = user.DTO;
            return Ok(uDTO);

        }


        [HttpGet("ByNameandPassword/{UserName}", Name = "GetPasswordByUserName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<bool> GetPasswordByUserName(string UserName, string Password)
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                return BadRequest($"Username and Password are required.");
            }
            bool Found = NotesAPI_Business.clsUser.FindUserByUserNameandPassword(UserName, Password);
            if (!Found)
            {
                return NotFound($"User with UserName {UserName} or Password not found.");
            }
            return Ok(Found);

        }


        [HttpPost("AddNew", Name = "AddNewUserDTO")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<UserDTO> AddNewUserDTO(UserCreateDTO newUser)
        {

            if (newUser == null || string.IsNullOrEmpty(newUser.UserName)
              || string.IsNullOrEmpty(newUser.Password)
              || string.IsNullOrEmpty(newUser.Note))

            {
                return BadRequest("Invalid user data.");
            }

            clsUser user = new clsUser();
            user.UserName = newUser.UserName;
            user.Note = newUser.Note;
            user.IsAdmin = newUser.IsAdmin;
            user.PasswordHash = clsSecurity.Hash256Password(newUser.Password);
            bool isSaved = user.Save();
            if (!isSaved)
            {
                return BadRequest("Not Saved");
            }
            return CreatedAtRoute("GetUserById", new { id = user.UserID }, user.DTO);

        }

        [HttpPut("Update/{id}", Name = "UpdateUserDTO")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserDTO> UpDateUserDTO(int id, UserDTO updatauser)
        {
            if (updatauser == null
                || updatauser.UserID < 0 ||
                string.IsNullOrEmpty(updatauser.UserName) ||
                string.IsNullOrEmpty(updatauser.Note))
            {
                return BadRequest("Invalid user data.");
            }

            clsUser user = clsUser.FindUserByID(id);
            if (user == null)
            {
                return NotFound($"Not Found User With {id}");
            }

            user.UserID = id;
            user.UserName = updatauser.UserName;
            user.Note = updatauser.Note;
            user.IsAdmin = updatauser.IsAdmin;
            bool isSave = user.Save();
            if (!isSave)
            {
                return BadRequest("Not Saved Update");
            }
            return Ok(user.DTO);
        }

        [HttpDelete("Delete/{id}", Name = "DeleteUserDTO")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteUserDTO(int id)
        {
            if (id < 1)
            {
                return BadRequest("Invalid user data.");
            }
            clsUser user = clsUser.FindUserByID(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} was not found.");
            }
            bool isDeleted = clsUser.DeleteUser(id);

            if (!isDeleted)
            {
                return BadRequest("Delete failed.");
            }
            return Ok($"User {id} has been deleted successfully.");
        }


        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Register([FromBody] RegisterDTO newregister)
        {
            if (newregister == null ||
               string.IsNullOrEmpty(newregister.UserName) ||
               string.IsNullOrEmpty(newregister.Password) 
              ) 
            {
                return BadRequest("Invalid user data.");
                    
                    
                    };

            if(clsUser.isUserExist(newregister.UserName))
            {
                return BadRequest("User already exists.");
            }

            clsUser user = new clsUser();
            user.Mode = clsUser.enMode.AddNew;
            user.UserName = newregister.UserName;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newregister.Password);
            user.Note = newregister.Note;
            user.IsAdmin = false;

            bool isSave = user.Save();
            if(!isSave)
            {
                return BadRequest("Not Saved");
            }
            return CreatedAtRoute("GetUserById", new {id= user.UserID} , user.DTO);
        
        }

       
        [HttpPut("ChangPassword/{UserName}", Name = "ChangPasswordDTO")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ChangPasswordDTOstring(string UserName, [FromBody] ChangePasswordDTO ChangePassword)
        {
            if (ChangePassword == null ||
                string.IsNullOrEmpty(ChangePassword.OldPassword) ||
                string.IsNullOrEmpty(ChangePassword.NewPassword) ||
                string.IsNullOrEmpty(ChangePassword.ConfirmNewPassword))
            {
                return BadRequest("Invalid password data.");
            }
            //find User by UserName
            clsUser user = clsUser.FindUserByUserName(UserName);
            if (user == null)
            {
                return NotFound($"User '{UserName}' not found.");
            }

            //Check if old password is right
            string stroeHash = clsUser.FindPasswordHashByUserName(UserName); ;
            bool isOldCorrect = false;
            if(stroeHash.Length==64)
            {
                string oldHashh = clsSecurity.Hash256Password(ChangePassword.OldPassword);
                isOldCorrect = (oldHashh == stroeHash);
            }
            else
            {
                isOldCorrect = BCrypt.Net.BCrypt.Verify(ChangePassword.OldPassword, stroeHash);
            }

            if(!isOldCorrect)
            {
                return BadRequest("Old password is incorrect.");
            }
        

            //Check Newpasswor== ConfirmNewPassword
            if (ChangePassword.NewPassword != ChangePassword.ConfirmNewPassword)
            {
                return BadRequest("ConfirmNewPassword is incorrect.");
            }
            //Security NewPassword
          
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(ChangePassword.NewPassword);
            user.Mode = clsUser.enMode.UpdatePassword;
            //Save Update Password
            bool isSave = user.Save();

            if (!isSave)
            {
                return BadRequest("Not Change NewvPassword");
            }
            // Return Update user DTO
            return Ok(user.DTO);

        }

        
        [HttpPost("Auth", Name = "userLogin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult userLogin([FromBody] UserLoginRequest dto)
        {
            if (string.IsNullOrEmpty(dto.UserName) || string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest("Invalid password data.");
            }
            clsUser user = clsUser.FindUserByUserName(dto.UserName);


            if (user == null)
            {
                return NotFound($"User '{dto.UserName}' not found.");
            }

            string storePasswordHash = clsUser.FindPasswordHashByUserName(dto.UserName);
            if(storePasswordHash==null)
            {
                return BadRequest("PasswordHash is NULL for this user!");
            }

            if(storePasswordHash.Length == 64)
            {
               bool isOldPasswordCorrect = clsUser.FindUserByUserNameandPassword(dto.UserName, dto.Password);


                if (!isOldPasswordCorrect)
                {
                    
                        return BadRequest("Old password is incorrect.");
                }
                return Ok(new
                {
                    RequirePasswordReset = true,
                    UserId = user.UserID,
                    Message = "Your password uses old hashing. Please reset it."
                });

            }

            // New BCrypt
            bool isVailed = BCrypt.Net.BCrypt.Verify(dto.Password, storePasswordHash);

            if (!isVailed)
            {
                return BadRequest("Incorrect username or password.");
            }
            //return Ok(new

            //{
            //    Message = "Login Successful",
            //    UserId = user.UserID,
            //    UserName = user.UserName,
            //    passordhash = user.PasswordHash

            //});

            string token = CreateToken(user);
            return Ok(token);



        }

        private string CreateToken(clsUser user)
        {
            List<Claim> claims = new()
            {
                 new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),

                 new Claim(ClaimTypes.Name, user.UserName),
                   
                new Claim (ClaimTypes.Role, user.IsAdmin? "Admin":"User")
                    
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:Token"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds

                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

    }
}
