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

    public async Task IncrementCompletedCount(int cancelTimeout = 3000)
    {
        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(cancelTimeout);

        User? user = (await this._userRepository.GetAsync(x => true)).FirstOrDefault();

        if (user is null)
        {
            throw new UserNotFoundException("Текущий пользователь не найден");
        }

        user.IncrementTechniquesCompletedCount();

        await this._userRepository.UpdateAsync(user);

        await _unitOfWork.Commit();
    }

    public async Task SetSubscribersCount(int count, int cancelTimeout = 3000)
    {
        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(cancelTimeout);

        User? user = (await this._userRepository.GetAsync(x => true)).FirstOrDefault();

        if (user is null)
        {
            throw new UserNotFoundException("Текущий пользователь не найден");
        }

        user.SetSubscribersCount(count);

        await this._userRepository.UpdateAsync(user);

        await _unitOfWork.Commit();
    }

    public async Task SetUserInitials(string name, string surname, string patronymic, int cancelTimeout = 3000)
    {
        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(cancelTimeout);

        User? user = (await this._userRepository.GetAsync(x => true)).FirstOrDefault();

        if (user is null)
        {
            throw new UserNotFoundException("Текущий пользователь не найден");
        }

        user.SetUserInitials(name, surname, patronymic);

        await this._userRepository.UpdateAsync(user);

        await _unitOfWork.Commit();
    }

    public async Task UnitLocalUser(int cancelTimeout = 3000)
    {
        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(cancelTimeout);

        User user = new(string.Empty, string.Empty, string.Empty, 0, 0);

        await this._userRepository.InsertAsync(user);

        await this._unitOfWork.Commit();
    }
}
