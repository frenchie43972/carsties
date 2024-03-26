using Microsoft.EntityFrameworkCore;

namespace AuctionServices;

// Declaration of AuctionDbContext class that inherits from DbContext
// which is part of the EF Core that helps interact with the datbase
public class AuctionDbContext : DbContext
{
  // A Constructor of the AuctionDbContext class that takes a parameter which
  // Contains configurations such a the DB provide (Postgres on our case) and 
  // the connection string and passes this infor to DbContext 
  public AuctionDbContext(DbContextOptions options) : base(options)
  {
  }

  // This represents the Auction Table in the DB. It is a collecion of Auction
  // entities that EF Core uses for queries and saving instances 
  public DbSet<Auction> Auctions { get; set; }
}
