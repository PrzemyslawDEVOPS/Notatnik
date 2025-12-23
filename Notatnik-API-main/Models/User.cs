using System.ComponentModel.DataAnnotations;

namespace NotatnikAPI.Models;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    public ICollection<Note> Notes { get; set; } = new List<Note>();
}

