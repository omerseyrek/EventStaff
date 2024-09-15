using EventStaf.Entities;
using EventStaf.Infra.Cache;
using EventStaf.Infra.Result;
using EventStaf.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventStaf.Controllers
{
    [Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly IUserService<AppUser> _userService;
	    private readonly ICacheService _cacheService;

		public UsersController(IUserService<AppUser> userService, ICacheService cacheService)
		{
			_userService = userService;
			_cacheService = cacheService;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
		{
			var result = await _userService.GetAllAsync();
			if (!result.IsSuccess)
				return BadRequest(result.Errors);
			return Ok(result.Value);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<AppUser>> GetUser(int id)
		{
			var cacheKey = $"{typeof(AppUser).Name}_{id}";
			var cachedUser = await _cacheService.GetAsync<AppUser>(cacheKey);

			if (cachedUser != null)
			{
				return Ok(Result<AppUser>.Success(cachedUser));
			}

			var result = await _userService.GetByIdAsync(id);

			if (!result.IsSuccess)
			{
				return NotFound(result);
			}

			await _cacheService.SetAsync(cacheKey, result.Value, TimeSpan.FromSeconds(60));

			return Ok(result);
		}

		[HttpPost]
		public async Task<ActionResult<AppUser>> CreateUser(AppUser user)
		{
			var result = await _userService.CreateAsync(user);
			if (!result.IsSuccess)
				return BadRequest(result.Errors);
			return CreatedAtAction(nameof(GetUser), new { id = result?.Value?.Id }, result?.Value);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateUser(int id, AppUser user)
		{
			if (id != user.Id)
				return BadRequest("ID mismatch");

			var result = await _userService.UpdateAsync(user);
			if (!result.IsSuccess)
				return BadRequest(result.Errors);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			var result = await _userService.DeleteAsync(id);
			if (!result.IsSuccess)
				return BadRequest(result.Errors);
			return NoContent();
		}
	}
}
