using EventStaf.Models;

namespace EventStaf.Repositories
{
	public interface IUnitOfWork : IDisposable
	{
		IRepository<AppUser> AppUsers { get; }
		// Add other repositories as needed
		Task<int> CompleteAsync();
		IRepository<T> GetRepository<T>() where T : EntityBase;
	}
}
