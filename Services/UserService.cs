﻿using EventStaf.Models;
using EventStaf.Repositories;

namespace EventStaf.Services
{

	public class UserService : BaseService<AppUser>, IUserService<AppUser>
	{
		public UserService(IUnitOfWork unitOfWork) : base(unitOfWork)
		{
		}
	}
}
