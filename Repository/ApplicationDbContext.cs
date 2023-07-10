using Domain.DomainModels;
using Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class ApplicationDbContext : IdentityDbContext<CinemaApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
    
    public virtual DbSet<EmailMessage> EmailMessages { get; set; }
    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<Ticket> Tickets { get; set; }
    public virtual DbSet<TicketInOrder> TicketsInOrders { get; set; }
    public virtual DbSet<TicketInShoppingCart> TicketsInShoppingCarts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Ticket>()
            .Property(z => z.Id)
            .ValueGeneratedOnAdd();

        builder.Entity<TicketInShoppingCart>()
            .HasOne(z => z.Ticket)
            .WithMany(z => z.TicketsInShoppingCarts)
            .HasForeignKey(z => z.ShoppingCartId);

        builder.Entity<TicketInShoppingCart>()
            .HasOne(z => z.ShoppingCart)
            .WithMany(z => z.TicketsInShoppingCart)
            .HasForeignKey(z => z.TicketId);

        builder.Entity<TicketInOrder>()
            .Property(z => z.Id)
            .ValueGeneratedOnAdd();

        builder.Entity<TicketInOrder>()
            .HasOne(z => z.Ticket)
            .WithMany(z => z.TicketsInOrder)
            .HasForeignKey(z => z.OrderId);

        builder.Entity<TicketInOrder>()
            .HasOne(z => z.Ticket)
            .WithMany(z => z.TicketsInOrder)
            .HasForeignKey(z => z.TicketId);

        builder.Entity<ShoppingCart>()
            .Property(z => z.Id)
            .ValueGeneratedOnAdd();

        builder.Entity<ShoppingCart>()
            .HasOne(z => z.User)
            .WithOne(z => z.ShoppingCart)
            .HasForeignKey<ShoppingCart>(z => z.UserId);

        builder.Entity<Order>()
            .Property(z => z.Id)
            .ValueGeneratedOnAdd();
    }
}