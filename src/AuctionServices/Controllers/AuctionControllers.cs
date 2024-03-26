using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionServices;

// Defines the controller with a Route prefix. All routes will begin with "api/auctions"
[ApiController]
[Route("api/auctions")]
public class AuctionControllers : ControllerBase
{
    // DbContext for accessing the database and IMapper for mapping are to be injected
    // by the Constructor below
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    // Contructor that initialzes the injected DbContext and IMapper for use in the control method
    public AuctionControllers(AuctionDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // Action method that Gets all auctions
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions() 
    {
        var auctions = await _context.Auctions
            .Include(x => x.Item) // Includes relevant Item data to avoid lazy loading or null references
            .OrderBy(x => x.Item.Make) // Order results by Make
            .ToListAsync(); //Executes the query and converts the results to a list

        // Maps the Auction entities to the AuctionDto object
        return _mapper.Map<List<AuctionDto>>(auctions);
    }

    // Method to Get a single auction item by GUID 
    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id) 
    {
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id); // Find the first auction that matches the id or throws null

        // If no auction found returns a 404 response
        if (auction == null) return NotFound();

        return _mapper.Map<AuctionDto>(auction);
    }

    // Method to create a new auction post
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto) 
    {
        var auction = _mapper.Map<Auction>(auctionDto);
        // TODO: still need to add current user as seller
        auction.Seller = "test";

        // Adds the new auction to DbContext for insertion
        _context.Auctions.Add(auction);

        // Asynchronously save the changes to the database. If successful, more than 0 changes should be saved.
        var result = await _context.SaveChangesAsync() > 0;

        // If the save operation was unsuccessful, throsw a BadRequest, if success adds new entry and 
        // returns a 201 response
        if (!result) return BadRequest("Could not add entry to the DB");

        return CreatedAtAction(nameof(GetAuctionById), new {auction.Id}, _mapper.Map<AuctionDto>(auction));
    }

    // Updates the database
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await _context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null) return NotFound();

        // TODO: check seller == username

        // Updates the Item properties with the values inputted. If updateAuctionDto is null, then the 
        // current value is retained else replace with the new value. 
        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Ok();

        return BadRequest("Problen saving changes.");
    }

    // Remove the entry from the DB
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);

        if (auction == null) return NotFound();

        // TODO: Check seller == username

        _context.Auctions.Remove(auction);

        var result = await _context.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Could not update DB.");

        return Ok();
    }
}