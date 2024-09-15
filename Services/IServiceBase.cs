using EventStaf.Infra.Result;
using System.Linq.Expressions;

namespace EventStaf.Services
{
    public interface IServiceBase<T>
	{
		Task<Result<T?>> GetByIdAsync(int id);
		Task<Result<IEnumerable<T>?>> GetAllAsync();
		Task<Result<IEnumerable<T>?>> FindAsync(Expression<Func<T, bool>> filter);
		Task<Result<T?>> CreateAsync(T user);
		Task<Result<T?>> UpdateAsync(T user);
		Task<Result<bool>> DeleteAsync(int id);
		Task<Result<bool>> AnyAsync();
	}
}