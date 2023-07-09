using System.ComponentModel.DataAnnotations;

namespace Domain.DomainModels;

public class BaseEntity
{ 
    [Key]
    public Guid Id { get; set; }
}