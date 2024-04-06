using AutoMapper;
using PsychologyApp.Application.ApiHandlers;
using PsychologyApp.Application.Exceptions;
using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Common;
using PsychologyApp.Domain.Constants;
using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Services.QuotService;

public class QuotService : IQuotService
{
    private readonly IGenericRepository<Quot> _quotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly Mapper _mapper;

    public QuotService(IGenericRepository<Quot> quotRepository, IUnitOfWork unitOfWork, Mapper mapper)
    {
        _quotRepository = quotRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task AddNewQuot(QuotDTO quotDTO)
    {
        Quot quot = _mapper.Map<Quot>(quotDTO);

        await _quotRepository.InsertAsync(quot);

        await _unitOfWork.Commit();
    }

    public async Task<IList<QuotDTO>> GetQuotsList(int count, bool readed = false)
    {
        IList<Quot> quots = (await _quotRepository.GetAsync(x => x.IsReaded == readed)).TakeLast(count).ToList();

        IList<QuotDTO> quotDTOs = _mapper.Map<IList<QuotDTO>>(quots);

        return quotDTOs;
    }

    public async Task MarkQuotAsReaded(int quotId)
    {
        Quot? quot = await _quotRepository.FindByIdAsync(quotId);

        if (quot is null)
        {
            throw new QuotNotFoundException($"Цитата с идентификатром {quotId} не найдена");
        }

        quot.MarkAsReaded();

        await _quotRepository.UpdateAsync(quot);

        await _unitOfWork.Commit();
    }

    public async Task SaveQuotsFromApi(int count)
    {
        QuotDTO quotDTO = await QuotsHandler.GetQuotsFromApi();

        Quot quot = _mapper.Map<Quot>(quotDTO);

        await _quotRepository.InsertOrUpdateAsync(quot);

        await _unitOfWork.Commit();
    }
}
