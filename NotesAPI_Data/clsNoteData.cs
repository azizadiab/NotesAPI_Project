using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace NotesAPI_Data
{

    public class NoteDTO
    {   
        public int NoteID { get; set; }
        public string Title { get; set; }
        public string Content{get;set;}
       public int UserID {get;set;}
        public DateTime CreatedDate { get; set; }



        public NoteDTO() { }

        public NoteDTO(int noteID, string title, string content, int userID, DateTime createdDate)
        {
            NoteID = noteID;
            Title = title;
            Content = content;
            UserID = userID;
            CreatedDate = createdDate;


        }
    }
   

    public class clsNoteData
    {
        
        public static List<NoteDTO> GetAllNotes(int PageNumber, int PageSize)
        {
            List<NoteDTO> NoteList = new List<NoteDTO>();
            
            using(SqlConnection con=new SqlConnection(clsDataAccessSettings.connectionString))
                using(SqlCommand cmd = new SqlCommand("SP_GetAllNotes", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PageNumber", PageNumber);
                cmd.Parameters.AddWithValue("@PageSize", PageSize);
                try
                {
                    con.Open();
                   using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            int NoteID = reader.GetInt32(reader.GetOrdinal("NoteID"));
                            string Title = reader.GetString(reader.GetOrdinal("Title"));
                            string Content = reader.GetString(reader.GetOrdinal("Content"));
                            int UserID = reader.GetInt32(reader.GetOrdinal("UserID"));
                            DateTime CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));
                            NoteList.Add(new NoteDTO(NoteID, Title, Content, UserID, CreatedDate));
                        }

                    }
                }catch(Exception ex)
                    {

                throw new DataException("Database error while getting all Notes.", ex);
            }
        }
            return NoteList;
        }

        public static NoteDTO GetNoteById(int NoteID)
        {
           
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_GetNoteByID", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NoteID", NoteID);
                
                try {

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if(reader.Read())
                    {
                        string Title = reader.GetString(reader.GetOrdinal("Title"));
                        string Content = reader.GetString(reader.GetOrdinal("Content"));
                        int UserID = reader.GetInt32(reader.GetOrdinal("UserID"));
                        DateTime CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));
                        return new NoteDTO(NoteID, Title, Content, UserID, CreatedDate);

                    }
                    else
                    {
                        return null;
                    }

                }
                catch (Exception ex)
                {
                 
                    throw new DataException("Database error while getting a Note.", ex);

                }

            }


            }


        public static List<NoteDTO> GetNotesByUserId(int UserID, int PageNumber, int PageSize)
        {
            List<NoteDTO> NoteList = new List<NoteDTO>();
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_GetNoteByUserID", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.Add("@PageNumber", SqlDbType.Int).Value = PageNumber;
                cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value=PageSize;

                try
                {

                    con.Open();
                    
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int NoteID = reader.GetInt32(reader.GetOrdinal("NoteID"));
                        string Title = reader.GetString(reader.GetOrdinal("Title"));
                        string Content = reader.GetString(reader.GetOrdinal("Content"));
                        DateTime CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));
                        NoteList.Add(new NoteDTO(NoteID, Title, Content, UserID, CreatedDate));
                    }

                }
                catch (Exception ex)
                {

                    throw new DataException("Database error while getting a Note.", ex);
                }


            }
            return NoteList;

        }

        public static int AddNewNote(string title, string content, int userID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_AddNote", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Content", content);
                cmd.Parameters.AddWithValue("@UserID", userID);

                SqlParameter outputIdParam = new SqlParameter("@NewNoteID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputIdParam);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    return outputIdParam.Value != DBNull.Value ? (int)outputIdParam.Value : -1;


                }
                catch (Exception ex)
                {

                    throw new DataException("Database error while adding a new Note.", ex);

                }
            }

        }

        public static bool DeleteNote(int NoteID)
        {
            using(SqlConnection con= new SqlConnection(clsDataAccessSettings.connectionString))
                using(SqlCommand cmd=new SqlCommand("SP_DeleteNoteByID", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NoteID", NoteID);

                try
                {
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                    
                }
                catch (Exception ex)
                {

                    throw new DataException("Database error while deleting a Note.", ex);

                }

            }
           
        }

        public static bool UpdateNote(NoteDTO DTO)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_UpdateNote", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NoteID", DTO.NoteID);
                cmd.Parameters.AddWithValue("@Title", DTO.Title);
                cmd.Parameters.AddWithValue("@Content", DTO.Content);
                cmd.Parameters.AddWithValue("@UserID", DTO.UserID);

                try
                {
                    con.Open();
                   int rowEffected = cmd.ExecuteNonQuery();
                     return rowEffected > 0;
                }
                catch (Exception ex)
                {

                    throw new DataException("Database error while updating a Note.", ex);

                }

            }

        }

        public static List<NoteDTO> SearchNotes(int UserID, string Search)
        {
            List<NoteDTO> NoteList = new List<NoteDTO>();
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_SearchNote", con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 50)
                .Value = string.IsNullOrWhiteSpace(Search) ? DBNull.Value : Search;

                try
                {
                   
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int NoteID = reader.GetInt32(reader.GetOrdinal("NoteID"));
                            string Title = reader.GetString(reader.GetOrdinal("Title"));
                            string Content = reader.GetString(reader.GetOrdinal("Content"));
                            int DbUserID = reader.GetInt32(reader.GetOrdinal("UserID"));
                            DateTime CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));
                            NoteList.Add(new NoteDTO(NoteID, Title, Content, DbUserID, CreatedDate));
                        }

                    }

                }

                catch (Exception ex)
                {

                    throw new DataException("Database error while Get a Notes.", ex);

                }
                return NoteList;
            }


          }

    }
}
