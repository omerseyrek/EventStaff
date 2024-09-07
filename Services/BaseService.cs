using EventStaf.Models;
using EventStaf.Repositories;
using System.Collections.Generic;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EventStaf.Services
{
	public class BaseService<T> :  IServiceBase<T> where T : EntityBase
	{
		private readonly IUnitOfWork _unitOfWork;
		private IRepository<T> _serviceRepository;
		private string _entityName;

		public BaseService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
			_serviceRepository = _unitOfWork.GetRepository<T>();
			_entityName = typeof(T).Name;
		}

		public async Task<Result<T>> GetByIdAsync(int id)
		{
			T data = await _serviceRepository.GetByIdAsync(id);
			if (data == null)
				return Result<T>.Failure($"{_entityName} with id {id} not found.");
			return Result<T>.Success(data);
		}

		public async Task<Result<IEnumerable<T>>> GetAllAsync()
		{
			IEnumerable<T> data = await _serviceRepository.GetAllAsync();
			return Result<IEnumerable<T>>.Success(data);
		}

		public async Task<Result<T>> CreateAsync(T entityData)
		{
			await _serviceRepository.AddAsync(entityData);
			var result = await _unitOfWork.CompleteAsync();
			if (result <= 0)
				return Result<T>.Failure($"Failed to create {_entityName} .");
			return Result<T>.Success(entityData);
		}

		public async Task<Result<T>> UpdateAsync(T entityData)
		{
			await _serviceRepository.UpdateAsync(entityData);
			var result = await _unitOfWork.CompleteAsync();
			if (result <= 0)
				return Result<T>.Failure($"Failed to update {_entityName} with id {entityData.Id}.");
			return Result<T>.Success(entityData);
		}

		public async Task<Result<bool>> DeleteAsync(int id)
		{
			T data = await _serviceRepository.GetByIdAsync(id);
			if (data == null)
				return Result<bool>.Failure($"{_entityName } with id {id} not found.");

			await _serviceRepository.DeleteAsync(data);
			var result = await _unitOfWork.CompleteAsync();
			if (result <= 0)
				return Result<bool>.Failure($"Failed to delete {_entityName } with id {id}.");
			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> AnyAsync()
		{
			bool dataExists = await _serviceRepository.AnyAsync();
			return Result<bool>.Success(dataExists);
		}

		public async Task<Result<IEnumerable<T>>> FindAsync(Expression<Func<T, bool>> filter)
		{
			IEnumerable<T> data = await _serviceRepository.FindAsync(filter);
			return Result<IEnumerable<T>>.Success(data);
		}
	}
}
