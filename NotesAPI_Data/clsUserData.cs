using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Reflection.PortableExecutable;

namespace NotesAPI_Data
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Note { get; set; }
        public bool IsAdmin { get; set; }

        public UserDTO() { }
        public UserDTO(int userID, string userName, string note, bool isAdmin)
        {
            UserID = userID;
            UserName = userName;
            Note = note;
            IsAdmin = isAdmin;

        }
    }


    public class RegisterDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Note { get; set; }
        public bool IsAdmin { get; set; }

        public RegisterDTO() { }
        public RegisterDTO(string userName, string password, string note, bool isAdmin)
        {
           
            UserName = userName;
            Password = password;
            Note = note;
            IsAdmin = isAdmin;

        }
    }

    public class UserCreateDTO
    {

        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Note { get; set; }
        public bool IsAdmin { get; set; }

        public UserCreateDTO() { }

        public UserCreateDTO(int userID, string userName,string password, string note, bool isAdmin)
        {
            UserID = userID;
            UserName = userName;
            Password = password;
            Note = note;
            IsAdmin = isAdmin;

        }

    }

    public class UserLoginRequest
    {

        public string UserName { get; set; }
        public string Password { get; set; }

        public UserLoginRequest() { }
        public UserLoginRequest(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

    }

    public class ChangePasswordDTO
    {
      
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }

        public ChangePasswordDTO() { }
        public ChangePasswordDTO(string oldpassword, string newpassword, string confirmNewPassword)

        {
            
            OldPassword = oldpassword;
            NewPassword = newpassword;
            ConfirmNewPassword = confirmNewPassword;


        }


    }


    public class clsUserData
    {

        public static List<UserDTO> GetAllUsersDTO()
        {

            var UsersList = new List<UserDTO>();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_GetAllUsers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    try
                    {

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {


                                int UserID = reader.GetInt32(reader.GetOrdinal("UserID"));
                                string UserName = reader.GetString(reader.GetOrdinal("UserName"));
                                string Note = !reader.IsDBNull(reader.GetOrdinal("Note"))
                                      ? reader.GetString(reader.GetOrdinal("Note")) : "";


                                bool IsAdmin = !reader.IsDBNull(reader.GetOrdinal("IsAdmin"))
                                  ? reader.GetBoolean(reader.GetOrdinal("IsAdmin")) : false;

                                UsersList.Add(new UserDTO(UserID, UserName, Note, IsAdmin));
                            }

                        }

                    }catch(Exception ex)
                    {
                        throw new Exception("An error occurred while getting all users.", ex);

                    }


                }

            }
            return UsersList;
        }
        public static UserDTO GetUserByID(int UserID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_GetUserByID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", UserID);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                               
                                string UserName = reader.GetString(reader.GetOrdinal("UserName"));

                                string Note = !reader.IsDBNull(reader.GetOrdinal("Note"))

                                       ? reader.GetString(reader.GetOrdinal("Note")) : "";

                                bool IsAdmin = !reader.IsDBNull(reader.GetOrdinal("IsAdmin"))
                                   ? reader.GetBoolean(reader.GetOrdinal("IsAdmin")) : false;

                                return new UserDTO(UserID, UserName, Note, IsAdmin);

                            }
                            else
                            {
                                return null;
                            }

                        }

                    }
                    catch (Exception ex)
                    {

                        throw new Exception("An error occurred while getting user By ID.", ex);

                    }
                }
            }
        }
        public static UserDTO GetUserByUserName(string UserName)
        {           
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand command = new SqlCommand("SP_GetUserByUserName", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserName", UserName);

                try
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {                         

                          int UserID = reader.GetInt32(reader.GetOrdinal("UserID"));

                            string Note = !reader.IsDBNull(reader.GetOrdinal("Note"))

                                  ? reader.GetString(reader.GetOrdinal("Note")) : "";

                            bool IsAdmin = !reader.IsDBNull(reader.GetOrdinal("IsAdmin"))
                               ? reader.GetBoolean(reader.GetOrdinal("IsAdmin")) : false;
                            return new UserDTO(UserID, UserName, Note, IsAdmin);

                        }
                        else
                        {
                            return null;
                        }

                    }

                }
                catch (Exception ex)
                {

                    throw new Exception("An error occurred while getting user By UserName.", ex);

                }

            }
        }
        public static string GetPasswordHashByUserName(string UserName)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand command = new SqlCommand("SP_GetPasswordHashByUserName", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserName", UserName);

                try
                {
                    connection.Open();

                    object result = command.ExecuteScalar();
                    return result != null ? result.ToString() : null;


                }
                catch (Exception ex)
                {

                    throw new Exception("An error occurred while getting user By UserName.", ex);

                }

            }
        }
        public static int AddNewUser(string username, string passwordHash, string note, bool isAdmin)
        {
            using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand Command = new SqlCommand("SP_AddNewUser", Connection))
            {
                Command.CommandType = CommandType.StoredProcedure;
                SqlParameter outputIdParam = new SqlParameter("@NewUserID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                Command.Parameters.Add(outputIdParam);
                Command.Parameters.AddWithValue("@UserName", username); 
                Command.Parameters.AddWithValue("@PasswordHash", passwordHash); 
                Command.Parameters.AddWithValue("@Note", note);
                Command.Parameters.AddWithValue("@IsAdmin", isAdmin);
               

                try
                {

                    Connection.Open();
                    Command.ExecuteNonQuery();

                    return outputIdParam.Value != DBNull.Value? (int)outputIdParam.Value : -1;


                }
                catch (Exception ex)
                {

                    throw new Exception("An error occurred while adding a new user.", ex);

                }
               
            }
          

        }
        public static bool UpdateUser(UserDTO DTO)
        {
            
            using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand Command = new SqlCommand("SP_UpdateUser", Connection))
            {
                Command.CommandType = CommandType.StoredProcedure;
                Command.Parameters.AddWithValue("@UserID", DTO.UserID);
                Command.Parameters.AddWithValue("@UserName", DTO.UserName);
                Command.Parameters.AddWithValue("@Note", DTO.Note);
                Command.Parameters.AddWithValue("@IsAdmin", DTO.IsAdmin);

                try
                {
                    Connection.Open();

                  Command.ExecuteNonQuery();
                   return true;

                }
                catch (Exception ex)
                {

                    //throw new Exception("An error occurred while Update a user.", ex);

                }
                
            }
            return false;

        }    
        public static bool UpdateUserPassword(int UserId, string PasswordHash)
        {
            using(SqlConnection connection= new SqlConnection(clsDataAccessSettings.connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_ChangePassword", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", UserId);
                    command.Parameters.AddWithValue("@PasswordHash", PasswordHash);

                    try
                    {

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch(Exception ex)
                    {
                        throw new Exception("An error occurred while Update a Password.", ex);
                    }
                }
            }


        }
        public static bool DeleteUserByID(int UserID)
        {

            
            using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand Command = new SqlCommand("SP_DeleteUserByID", Connection))

            {
                Command.CommandType = CommandType.StoredProcedure;
                Command.Parameters.AddWithValue("@UserID", UserID);


                try
                {

                    Connection.Open();
                   int rowsAffected = Command.ExecuteNonQuery();
                    return (rowsAffected > 0);

                }
                catch (Exception ex)
                {

                    throw new Exception($"An error occurred while Delete a user {UserID}.", ex);

                }               

            }

        }
        public static bool isUserExist(string UserName)
        {
            bool isFound = false;
            using (SqlConnection Connection = new SqlConnection(clsDataAccessSettings.connectionString))

            using (SqlCommand Command = new SqlCommand("SP_isUserExist", Connection))
            {
                Command.CommandType = CommandType.StoredProcedure;
                Command.Parameters.AddWithValue("@UserName", UserName);

                try
                {

                    Connection.Open();
                    SqlDataReader Reader = Command.ExecuteReader();
                    isFound = Reader.HasRows;

                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while Delete a user.", ex);

                }

                return isFound;


            }

        }
    }
}
