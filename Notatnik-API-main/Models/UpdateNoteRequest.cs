using System.ComponentModel.DataAnnotations;

namespace NotatnikAPI.Models;

public class UpdateNoteRequest
{
    public int? Id { get; set; }
    
    [Required]
    public string Content { get; set; } = string.Empty;
}

