using EventStaf.Entities;

namespace EventStaf.Services
{
    public interface IUserService<T> : IServiceBase<T> where T : EntityBase
	{

	}
}
