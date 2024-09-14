using EventStaf.Entities;
using EventStaf.Infra;
using EventStaf.Infra.Cache;
using EventStaf.Infra.MessageQue;
using EventStaf.Infra.Result;
using EventStaf.Models;
using EventStaf.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventStaf.Controllers
{

	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class EventsController : ControllerBase
	{
		private readonly IEventService _eventService;
		private readonly ICacheService _cacheService;
		private readonly IEventStafPublisher _eventPublisher; 

		public EventsController(IEventService eventService, ICacheService cacheService, IEventStafPublisher eventPublisher)
		{
			_eventService = eventService;
			_cacheService = cacheService;
			_eventPublisher = eventPublisher;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
		{
			var result = await _eventService.GetAllAsync();
			if (!result.IsSuccess)
				return BadRequest(result.Errors);
			return Ok(result.Value);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Event>> GetUser(int id)
		{
			var cacheKey = $"{typeof(Event).Name}_{id}";
			var cachedEvent = await _cacheService.GetAsync<Event>(cacheKey);

			if (cachedEvent != null)
			{
				return Ok(Result<Event>.Success(cachedEvent));
			}

			var result = await _eventService.GetByIdAsync(id);

			if (!result.IsSuccess)
			{
				return NotFound(result);
			}

			await _cacheService.SetAsync(cacheKey, result.Value, TimeSpan.FromSeconds(60));

			return Ok(result);
		}

		[HttpPost]
		public async Task<ActionResult<Event>> CreateUser(EventModel @eventModel)
		{
			Event newEntity = @eventModel.MapTo<Event>();

			var result = await _eventService.CreateAsync(newEntity);
			if (!result.IsSuccess)
				return BadRequest(result);

			eventModel.Id = newEntity.Id;

			await _eventPublisher.PublishEventOperation(@eventModel, OperationType.Create);

			return CreatedAtAction(nameof(GetUser), new { id = result.Value.Id }, result.Value);
		}

	}

}
