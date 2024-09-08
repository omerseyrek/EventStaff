using EventStaf.Data;
using EventStaf.Entities;
using EventStaf.Models;
using Microsoft.EntityFrameworkCore;

namespace EventStaf.Repositories
{
    public class UnitOfWork : IUnitOfWork
	{
		private readonly ApplicationDbContext _context;
		public IRepository<AppUser> AppUsers { get; private set; }

		public UnitOfWork(ApplicationDbContext context)
		{
			_context = context;
			AppUsers = new Repository<AppUser>(_context);
		}

		public IRepository<T> GetRepository<T>() where T : EntityBase
		{
			if(typeof(T) == typeof(AppUser))
			{
				return (IRepository<T>)AppUsers;
			}

			return null;
		}	

		public async Task<int> CompleteAsync()
		{
			return await _context.SaveChangesAsync();
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
