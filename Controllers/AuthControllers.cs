using Microsoft.AspNetCore.Mvc;
using auth11.Data;
using auth11.Records;
using auth11.Models;
using auth11.DTO;
using Microsoft.EntityFrameworkCore;

namespace auth11.Controllers;

[ApiController]
[Route("auth")]
public class AuthControllers : ControllerBase
{
    private readonly IHttpClientFactory _client;
    private readonly AppDbContext _context;

    public AuthControllers(AppDbContext context,IHttpClientFactory client)
    {
        _context= context;
        _client = client;
    }

    [HttpPost("reg")]
    public async Task<IActionResult> RegUser(RegUser dto)
    {

        try
        {
            if(string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Password))
        {
            return BadRequest("please add username and password");
        }
        if(await _context.User.AnyAsync(u=> (u.Name ?? "").ToLower() == dto.Name))
        {
            return BadRequest("Username already in use");
        }

        var user = new User
        {
            Name = dto.Name,
            HashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.User.Add(user);
        await _context.SaveChangesAsync();

        var response = new RegResp
        {
            Id = user.Id,
            Name = user.Name
        };

        return Ok(response);

        }catch(Exception ex)
        {
            return StatusCode(503,"Service not available try again later");
        }
        
    }
    [HttpPost("log")]
    public async Task<IActionResult> LogUser(LogDto dto)
    {
        if(string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Password))
        {
            return BadRequest("Username and password are mandatory");
        }
        var user = await _context.User.FirstOrDefaultAsync(u=> (u.Name ?? "").ToLower() == dto.Name);
        try
        {
             if(user == null || !BCrypt.Net.BCrypt.Verify(dto.Password , user.HashedPassword))
        {
            return BadRequest("Invalid username or password");
        }
        var response = new LogResp
        {
            Id = user.Id,
            Name = user.Name
        };
        return Ok(response);
        }catch(Exception ex)
        {
            return StatusCode(503, "Service not available try agin later");
        }
       
    }

    [HttpGet("puzzle")]
    public async Task<IActionResult> GetPuzzle()
    {
        var client = _client.CreateClient();
        var url= "https://lichess.org/api/puzzle/daily";
        var response = await client.GetFromJsonAsync<DailyPuzzle>(url);

        if (response?.Puzzle?.Solution == null || response?.Puzzle?.Fen == null)
        {
            return StatusCode(503,"Service not available try again later");
        }
        return Ok(response);
    }
}