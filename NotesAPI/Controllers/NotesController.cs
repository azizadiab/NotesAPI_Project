using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NotesAPI.Response;
using NotesAPI_Business;
using NotesAPI_Data;
using NotesAPI_Security;
using System.Collections.Generic;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NotesAPI.Controllers
{


    //[Authorize(Roles = "Admin")]
   // [Authorize]
    [Route("api/Notes")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("All", Name = "GetAllNotes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ApiResponse<IEnumerable<NoteDTO>>> GetAllNotes([FromQuery]int PageNumber = 1, [FromQuery] int PageSize = 10)
        {
            
            if(PageNumber<=0 || PageSize<=0)
            {
                return BadRequest(new ApiResponse<IEnumerable<NoteDTO>>
                {
                    Success = false,
                    Message = "Invailed Pagination Parameters",
                    Data = null
                });
            }
            var note = clsNote.GetAllNotea(PageNumber, PageSize);
            if (note == null || note.Count==0)
            {
                return NotFound(new ApiResponse<IEnumerable<NoteDTO>>

              {
                    Success = false,
                    Message = "Not Notes Found",
                    Data = note
                }
                    );
                
            }
            return Ok(new ApiResponse<IEnumerable<NoteDTO>>

            {
                Success = true,
                Message = "Note retrieved Successfully",
                Data = note


            }              
                );

        }


        [HttpGet("ById/{id}", Name = "GetNoteByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiResponse<clsNote>> GetNoteByID(int id)
        {
            if (id < 0)
            {
                return BadRequest($"Not accepted ID {id}");
            }
            clsNote note = clsNote.FindNoteByID(id);
            if (note == null)
            {
                return NotFound(
                   new ApiResponse<NoteDTO>
                   {
                       Success=false,
                       Message=$"Note with ID {id} Not Found",
                       Data=null
                   }


                    );
            }

            NoteDTO nDTO = note.DTO;
            return Ok(
                new ApiResponse<NoteDTO>
                {
                    Success = true,
                    Message = "",
                    Data = nDTO
                }

                );
        }



        [HttpPost("AddNew", Name = "AddNewNoteDTO")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<clsNote> AddNewNote(NoteDTO newNote)
        {
            if (newNote == null || string.IsNullOrEmpty(newNote.Content) ||
                string.IsNullOrEmpty(newNote.Content) ||
                newNote.UserID <= 0)
            {

                return BadRequest(new ApiResponse<clsNote>
                {
                    Success = false,
                    Message = "Invalid user data.",
                    Data = null
                });
            }

            clsNote note = new clsNote();
            note.Title = newNote.Title;
            note.Content = newNote.Content;
            note.UserID = newNote.UserID;


            bool isSave = note.Save();
            if (!isSave)
            {
                return BadRequest(

                    new ApiResponse<NoteDTO>
                    {
                        Success = false,
                        Message = "Failed to create note.",
                        Data = null
                    });
            }

            return CreatedAtRoute("GetNoteByID",
                new { id = note.NoteID },
               new ApiResponse<NoteDTO>
               {
                   Success = true,
                   Message = "Note created successfully",
                   Data = note.DTO
               });
        }



        [HttpDelete("Delete/{id}", Name = "DeletNoteByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiResponse<bool>>DeletNoteByID(int id)
        {
            if (id < 1)
            {
                return BadRequest("Invalid Note data.");
            }

            clsNote note = clsNote.FindNoteByID(id);
            if (note == null)
            {
                return NotFound($"Note with ID {id} was not found.");
            }
            bool isDelete = clsNote.DeleteNoteById(id);
            if (!isDelete)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Delete failed.",
                    Data = false
                }
                    
                    
                    );
            }

            return Ok(new ApiResponse<bool>
               { 
                Success=true,
                Message = $"Note {id} has been deleted successfully.",
                Data = true
            }
                
                );

        }



        [HttpPut("Update/{id}", Name = "UpdateNoteByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult <ApiResponse<NoteDTO>> UpdateNoteByID(int id, NoteDTO updateNote)
        {
            if (id < 0 || updateNote == null ||
                string.IsNullOrEmpty(updateNote.Title) ||
                string.IsNullOrEmpty(updateNote.Content) ||
                 updateNote.UserID < 0)
            {
                return BadRequest("Invalid Note data");

            }

            clsNote note = clsNote.FindNoteByID(id);
            if (note == null)
            {
                return NotFound(
                    new ApiResponse<NoteDTO>
                    {
                        Success = false,
                        Message = $"Not Found Note With {id}",
                        Data = null
                    });

            }
            note.Title = updateNote.Title;
            note.Content = updateNote.Content;
            note.UserID = updateNote.UserID;
            bool isSave = note.Save();
            if (!isSave)
            {
                return BadRequest(
                    new ApiResponse<NoteDTO>
                    {
                        Success = false,
                        Message = "Not Saved Update",
                        Data = null

                    });
            }
            return Ok(
                new ApiResponse<NoteDTO>
                {
               Success=true,
               Message="Note Updata Successfully",
               Data=note.DTO

                });
        }


        //Endpoint to get notes for the authenticated user
        [Authorize]
        [HttpGet("my", Name = "GetMyNotes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<NoteDTO>> GetMyNotes([FromQuery]int PageNumber = 1, [FromQuery]int PageSize = 10)
        {
            
            if(PageNumber <=0 || PageSize <=0)
            {
                return BadRequest(new ApiResponse<IEnumerable<NoteDTO>>
                {
                    Success = false,
                    Message = "Invailed Pagation Parameters",
                    Data = null

                });
            }
            int UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            List<NoteDTO> myNotes = clsNote.GetNotesByUserID(UserId, PageNumber, PageSize);
            return Ok(new ApiResponse<IEnumerable<NoteDTO>>
            {
                Success = true,
                Message = "User Notes Retrieved Successfully",
                Data = myNotes
            });
        }

        [Authorize]
        [HttpPost("my", Name = "AddMyNote")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<NoteDTO>AddMyNote(NoteDTO noteDTO)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            clsNote note = new clsNote{
                Title = noteDTO.Title,
                Content = noteDTO.Content,
                UserID = userId
            };
            if(string.IsNullOrWhiteSpace(note.Title) ||
                string.IsNullOrWhiteSpace(note.Content))
            {
                return BadRequest(new ApiResponse<NoteDTO>
                {
                    Success = false,
                    Message = "Title and Content are required"
                });
            }
            if (!note.Save())
            {
                return BadRequest(new ApiResponse<NoteDTO>
                {
                    Success = false,
                    Message = "Failed to create note",
                    Data =null
                });
            }
            return CreatedAtRoute("GetMyNotes",
                new { id = note.NoteID },
                new ApiResponse<NoteDTO>
                {
                    Success = true,
                    Message ="Note created successfully",
                    Data = note.DTO
                });

        }


        [Authorize]
        [HttpPut("update/{noteId}", Name = "updateMyNotes")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            public ActionResult<NoteDTO> UpdateMyNotes(int noteId, NoteDTO updatanotes)
        {
            
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            bool isAdmin = User.IsInRole("Admin");

            clsNote note = clsNote.FindNoteByID(noteId);
            if(note == null)
            {
                return NotFound(new ApiResponse<clsNote>
                {
                    Success = false,
                    Message = "Note Not Found",
                    Data = null
                });
            }

            if(userId != note.UserID && !isAdmin)
            
            {
                return StatusCode(403,new ApiResponse<NoteDTO>
                {
                    Success = false,
                    Message= "You are not autorized to update this note",
                    Data = null

                });
            }

            note.Title = updatanotes.Title; 
            note.Content = updatanotes.Content; 
         
            bool isSave = note.Save();
            if(!isSave)
            {
                return BadRequest(new ApiResponse<NoteDTO>
                {
                    Success = false,
                    Message = "Not Saved Updata",
                    Data = null
                });
            }

            return Ok(new ApiResponse<NoteDTO>
            {
                Success = true,
                Message = "Updated Successfully",
                Data = note.DTO

            });

            
        }

        [Authorize]
        [HttpDelete("Delete/{noteId}", Name = "deleteMyNotes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult deleteMyNotes(int noteId)
        {
            if (noteId < 1)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Invalid Note data.",
                    Data = false
                });
            }

            clsNote note = clsNote.FindNoteByID(noteId);
            if (note == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Note Not Found",
                    Data = false
                });
            }
            int UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            bool isAdmin = User.IsInRole("Admin");
            if(UserId!= note.UserID && !isAdmin)
            {
                return StatusCode(403,
                    new ApiResponse<bool> {
                        Success = false,
                        Message = "You are not authorized to delete this note",
                        Data = false
                    
                    });
            }
           bool isDelete = clsNote.DeleteNoteById(noteId);
            if(!isDelete)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Delete Failted",
                    Data = false

                });
            }


            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Note is Deleted",
                Data = true
            });

        }

        [HttpGet("Search", Name = "SearchNotes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiResponse<IEnumerable<NoteDTO>>> SearchNotes([FromQuery]int userId, [FromQuery]string search)
        {

            if (userId < 1 || string.IsNullOrEmpty(search))
            {
                return BadRequest(new ApiResponse<IEnumerable<NoteDTO>>
                {
                    Success = false,
                    Message = "Invalid Search Parameters",
                    Data = null
                });
            }
            var Notes = clsNote.SearchNotes(userId, search);
            if (Notes == null || Notes.Count==0)
            {
                return NotFound(new ApiResponse<IEnumerable<NoteDTO>>
                {
                    Success = false,
                    Message = "Search completed successfully",
                    Data = null
                });

            }

            return Ok(new ApiResponse<IEnumerable<NoteDTO>>
            {
                Success = true,
                Message = "Successfully Search",
                Data = Notes
            });
        }

        }
            
    }


    

