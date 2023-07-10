using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.DomainModels;

public class Ticket : BaseEntity
{
    [Required]
    [DisplayName("Movie Name")]
    public string MovieName { get; set; }
    
    [Required]
    [DisplayName("Movie Genre")]
    public Genre MovieGenre { get; set; }
    
    [Required]
    [DisplayName("Year")]
    public string MovieYear { get; set; }
    
    [Required]
    [DisplayName]
    public string MovieDescription { get; set; }
    
    
    [Required]
    [DisplayName("Image")]
    public string MovieImage { get; set; }
    
    [Required]
    [DisplayName("Ticket Price")]
    public int TicketPrice { get; set; }
    
    [Required]
    [DisplayName("Start Date")]
    public DateTime StartDate { get; set; }
    
    [Required]
    [DisplayName("End Date")]
    public DateTime EndDate { get; set; }
    
    public virtual ICollection<TicketInShoppingCart> TicketsInShoppingCarts { get; set; }
    public virtual ICollection<TicketInOrder> TicketsInOrder { get; set; }
    
}