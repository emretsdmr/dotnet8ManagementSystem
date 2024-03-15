using ManagementSystem_DotNet8.Data;
using ManagementSystem_DotNet8.DTOs;
using ManagementSystem_DotNet8.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ManagementSystem_DotNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InformationController : Controller
    {
        private readonly DataContext _context;

        public InformationController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<List<Information>>> AddInformation(CreateInformationDto information)
        {
            var newInformation = new Information() { 
                title = information.title, 
                description = information.description, 
                UserId = information.UserId };
            if (newInformation.title.IsNullOrEmpty())
            {
                return NotFound("Title can not be empty!");
            }
            if (newInformation.description.IsNullOrEmpty())
            {
                return NotFound("Description can not be empty!");
            }
            if (newInformation.UserId.IsNullOrEmpty()) 
            {
                return NotFound("User not found!");
            }
            _context.Informations.Add(newInformation);
            await _context.SaveChangesAsync();
            return Ok(newInformation);
        }

        [HttpGet]
        public async Task<ActionResult<List<Information>>> GetAllInformations()
        {
            var informations = await _context.Informations.ToListAsync();
            return Ok(informations);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<List<Information>>> GetInformation(string userId)
        {
            var information = await _context.Informations.Where(x => x.UserId == userId).ToListAsync();
            if (information.IsNullOrEmpty())
            {
                return NotFound("No informations found!");
            }
            return Ok(information);
        }

        [HttpPut]
        public async Task<ActionResult<List<Information>>> UpdateInformation(int infoId, UpdateInformationDto information)
        {
            var newInformation = await _context.Informations.FindAsync(infoId);
            if (newInformation is null)
            {
                return NotFound("Information couldn't found");
            }
            newInformation.title = information.title;
            newInformation.description = information.description;
            if (newInformation.title.IsNullOrEmpty())
            {
                return NotFound("Title can not be empty!");
            }
            if (newInformation.description.IsNullOrEmpty())
            {
                return NotFound("Description can not be empty!");
            }
            await _context.SaveChangesAsync();
            return Ok("Information updated");
        }

        [HttpDelete]
        public async Task<ActionResult<List<Information>>> DeleteInformation(int infoId)
        {
            
            var information = await _context.Informations.FindAsync(infoId);
            if(information is null)
            {
                return NotFound("Information couldn't found!");
            }
            _context.Informations.Remove(information);
            await _context.SaveChangesAsync();
            return Ok("Information deleted.");
        }

    }
}
