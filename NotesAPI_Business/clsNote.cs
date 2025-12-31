using NotesAPI_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NotesAPI_Business
{
    public class clsNote
    {
        public enum enMode { Add = 0, Update = 1 }

        enMode Mode = enMode.Add;
        public int NoteID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedDate { get; set; }

        public NoteDTO DTO {
            get
            {
                return new NoteDTO(this.NoteID, this.Title, this.Content, this.UserID, this.CreatedDate);
            }
        }

        public clsNote(NoteDTO DTO, enMode cMode = enMode.Update)
        {
            this.NoteID = DTO.NoteID;
            this.Title = DTO.Title;
            this.Content = DTO.Content;
            this.UserID = DTO.UserID;
            this.CreatedDate = DTO.CreatedDate;

            this.Mode = cMode;
        }
        public clsNote()
        {
            this.NoteID = -1;
            this.Title = "";
            this.Content = "";
            this.UserID = -1;
            Mode = enMode.Add;

        }


        public clsNote(int NoteID, string Title, string Content, int UserId)
        {
            this.NoteID = NoteID;
            this.Title = Title;
            this.Content = Content;
            this.UserID = UserId;
            Mode = enMode.Update;

        }

        public static List<NoteDTO> GetAllNotea(int PageNumber, int PageSize)
        {
            return clsNoteData.GetAllNotes(PageNumber, PageSize);
        }


        public static List<NoteDTO> GetNotesByUserID(int UserID, int PageNumber, int PageSize)
        {
            List<NoteDTO> noteDTO = clsNoteData.GetNotesByUserId(UserID, PageNumber, PageSize);
            return noteDTO;
        }

        public static clsNote FindNoteByID(int NoteID)
        {
            NoteDTO noteDTO = clsNoteData.GetNoteById(NoteID);


            if (noteDTO != null)
            {

                return new clsNote(noteDTO, enMode.Update);

            }
            else
            {
                return null;
            }


        }

        private bool _AddNewNote()
        {
            this.NoteID = clsNoteData.AddNewNote(this.Title, this.Content, this.UserID);
            return this.NoteID > 0;

        }

        public bool _UpdateNote()
        {
            return clsNoteData.UpdateNote(DTO);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.Add:
                    if (_AddNewNote())
                    {
                        Mode = enMode.Update;
                        return true;
                    } else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateNote();
            }

            return false;
        }

        public static bool DeleteNoteById(int NoteID)
        {
            return clsNoteData.DeleteNote(NoteID);
                     
        }


        public static List<NoteDTO> SearchNotes(int UserID, string Search)
        {

            return clsNoteData.SearchNotes(UserID, Search);
        }
    }
}
