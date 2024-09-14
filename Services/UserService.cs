using EventStaf.Entities;
using EventStaf.Repositories;

namespace EventStaf.Services
{

    public class UserService : BaseService<AppUser>, IUserService<AppUser>
	{
		public UserService(IUnitOfWork unitOfWork) : base(unitOfWork)
		{
		}
	}

	public class EventService : BaseService<Event>, IEventService
	{
		public EventService(IUnitOfWork unitOfWork) : base(unitOfWork)
		{
		}
	}
}
