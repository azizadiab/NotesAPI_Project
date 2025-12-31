using NotesAPI_Security;
using NotesAPI_Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Microsoft.Identity.Client;

namespace NotesAPI_Business
{
     public class clsUser
    {
        public enum enMode { AddNew = 0, Update = 1, UpdatePassword = 3 }

        public enMode Mode = enMode.AddNew;
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Note { get; set; }

        public bool IsAdmin { get; set; }

        public UserDTO DTO { 
            
            get { return (new UserDTO (this.UserID, this.UserName, this.Note, this.IsAdmin)); }
        }


        public clsUser()
        {
            this.UserID = -1;
            this.UserName = "";
            this.PasswordHash = "";
            this.Note = "";
            this.IsAdmin = false;
            Mode = enMode.AddNew;

        }

        public clsUser(UserDTO DTO, enMode cMode = enMode.Update)
        {
            this.UserID = DTO.UserID;
            this.UserName = DTO.UserName;
            this.Note = DTO.Note;
            this.IsAdmin = DTO.IsAdmin;
            Mode = cMode;

        }


        public clsUser(int UserID, string UserName,
                               string PasswordHash, string Note, bool IsAdmin)
        {
            this.UserID = UserID;
            this.UserName = UserName;
            this.PasswordHash = PasswordHash;
            this.Note = Note;
            this.IsAdmin = IsAdmin;
            Mode = enMode.Update;

        }

        public static List<UserDTO> GetAllUsers()
        {

            return clsUserData.GetAllUsersDTO();

        }


        public static clsUser FindUserByID(int UserID)
        {

            UserDTO userDTO = clsUserData.GetUserByID(UserID);
            if (userDTO !=null)
            {
                return new clsUser(userDTO, enMode.Update);
            }
               else
                {
                return null;
            }

        }

        public static clsUser FindUserByUserName(string UserName)
        {
            UserDTO userDTO = clsUserData.GetUserByUserName(UserName);
            if(userDTO != null)
            { 
                return new clsUser(userDTO, enMode.Update);
            }else
            {
                return null;
            }
        }

        public static bool FindUserByUserNameandPassword(string UserName, string Password)
        {

            UserDTO userDTO = clsUserData.GetUserByUserName(UserName);

            if(userDTO == null)
            {
                return false;
            }
                
            
             string enteredPasswordHash = clsSecurity.Hash256Password(Password);

            string storedPasswordHash = clsUserData.GetPasswordHashByUserName(UserName);

            return storedPasswordHash == enteredPasswordHash;


        }

        public static string FindPasswordHashByUserName(string UserName)
        {

           string storedPasswordHash =  clsUserData.GetPasswordHashByUserName(UserName);
            return storedPasswordHash;
        }

       //public static bool VerifySHA256Password(UserDTO passwordHash, string storepaassword)
       // {
       //     storepaassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

       // }

        private bool _AddNewUser()
        {

            
            this.UserID= clsUserData.AddNewUser(this.UserName, this.PasswordHash, this.Note, this.IsAdmin);
            return (this.UserID !=-1);

        }

        private bool _UpdateUser()
        {

            return clsUserData.UpdateUser(DTO);
        }


        private bool _UpdatePassword()
        {

            return clsUserData.UpdateUserPassword(UserID, PasswordHash);
        }



        public bool Save()
        {
            
            switch (Mode)
            {
                case enMode.AddNew:
                    if (clsUserData.isUserExist(DTO.UserName))
                    {
                        throw new Exception("UserName already exists.");
                    }
                    if (_AddNewUser())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateUser();

                case enMode.UpdatePassword:
                    return _UpdatePassword();
            }
            return false;
        }

     

        public static bool DeleteUser(int UserID)
        {

            return clsUserData.DeleteUserByID(UserID);
        }

        public static bool isUserExist(string UserName)
        {
            return clsUserData.isUserExist(UserName);

        }

        public void SetPassword(string Password)
        {

            this.PasswordHash = clsSecurity.Hash256Password(Password);

        }

        public bool CkecPassword(string Password)
        {
            return this.PasswordHash == clsSecurity.Hash256Password(Password);

        }

      

    }
}
