using System.ComponentModel.DataAnnotations;

namespace NotatnikAPI.Models;

public class Note
{
    public int Id { get; set; }
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    public int UserId { get; set; }
    
    public User User { get; set; } = null!;
}

