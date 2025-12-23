using System.ComponentModel.DataAnnotations;

namespace NotatnikAPI.Models;

public class CreateNoteRequest
{
    [Required]
    public string Content { get; set; } = string.Empty;
}

