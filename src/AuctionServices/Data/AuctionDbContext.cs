﻿using Microsoft.EntityFrameworkCore;

namespace AuctionServices;

public class AuctionDbContext : DbContext
{
  public AuctionDbContext(DbContextOptions options) : base(options)
  {
  }

  public DbSet<Auction> Auctions { get; set; }
}
