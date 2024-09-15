using EventStaf.Data;
using EventStaf.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EventStaf.Repositories
{
    public class Repository<T> : IRepository<T> where T : EntityBase
	{
		protected readonly ApplicationDbContext _context;
		private readonly DbSet<T> _dbSet;

		public Repository(ApplicationDbContext context)
		{
			_context = context;
			_dbSet = context.Set<T>();
		}

		public async Task<T?> GetByIdAsync(int id)
		{
			return await _dbSet.FindAsync(id);
		}

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
		{
			return await _dbSet.Where(predicate).ToListAsync();
		}

		public async Task AddAsync(T entity)
		{
			await _dbSet.AddAsync(entity);
		}

		public Task UpdateAsync(T entity)
		{
			_dbSet.Update(entity);
			return Task.CompletedTask;
		}

		public Task DeleteAsync(T entity)
		{
			_dbSet.Remove(entity);
			return Task.CompletedTask;
		}

		public async Task<bool> AnyAsync()
		{
			return await _dbSet.AnyAsync();
		}
	}
}
