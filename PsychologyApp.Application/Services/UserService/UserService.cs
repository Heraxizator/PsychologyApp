using AutoMapper;
using PsychologyApp.Application.Exceptions;
using PsychologyApp.Domain.Common;
using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Services.UserService;

public class UserService : IUserService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly Mapper _mapper;

    public UserService(IGenericRepository<User> userRepository, IUnitOfWork unitOfWork, Mapper mapper)
    {
        this._userRepository = userRepository;
        this._unitOfWork = unitOfWork;
        this._mapper = mapper;
    }

    public async Task IncrementCompletedCount()
    {
        User? user = this._userRepository.Get(x => true).FirstOrDefault();

        if (user is null)
        {
            throw new UserNotFoundException("Текущий пользователь не найден");
        }

        user.IncrementTechniquesCompletedCount();

        this._userRepository.Update(user);

        await _unitOfWork.Commit();
    }

    public async Task SetSubscribersCount(int count)
    {
        User? user = this._userRepository.Get(x => true).FirstOrDefault();

        if (user is null)
        {
            throw new UserNotFoundException("Текущий пользователь не найден");
        }

        user.SetSubscribersCount(count);

        this._userRepository.Update(user);

        await _unitOfWork.Commit();
    }

    public async Task SetUserInitials(string name, string surname, string patronymic)
    {
        User? user = this._userRepository.Get(x => true).FirstOrDefault();

        if (user is null)
        {
            throw new UserNotFoundException("Текущий пользователь не найден");
        }

        user.SetUserInitials(name, surname, patronymic);

        this._userRepository.Update(user);

        await _unitOfWork.Commit();
    }

    public async Task UnitLocalUser()
    {
        User user = new(string.Empty, string.Empty, string.Empty, 0, 0);

        this._userRepository.Insert(user);

        await this._unitOfWork.Commit();
    }
}
