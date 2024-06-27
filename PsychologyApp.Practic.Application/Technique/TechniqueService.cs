using AutoMapper;
using PsychologyApp.Application.Exceptions;
using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Common;
using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Services.TechniqueService;

public class TechniqueService : ITechniqueService
{
    private readonly IGenericRepository<Technique> _techniqueRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly Mapper _mapper;

    public TechniqueService(IGenericRepository<Technique> techniqueRepository, IUnitOfWork unitOfWork, Mapper mapper)
    {
        _techniqueRepository = techniqueRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task AddNewTechnique(TechniqueDTO techniqueDTO)
    {
        Technique technique = _mapper.Map<Technique>(techniqueDTO);

        await _techniqueRepository.InsertAsync(technique);

        await _unitOfWork.Commit();
    }

    public async Task DeleteTechnique(TechniqueDTO techniqueDTO)
    {
        Technique technique = _mapper.Map<Technique>(techniqueDTO);

        await _techniqueRepository.RemoveAsync(technique);

        await _unitOfWork.Commit();
    }

    public async Task<TechniqueDTO> GetTechniqueById(int id)
    {
        Technique? technique = await this._techniqueRepository.FindByIdAsync(id);

        TechniqueDTO? techniqueDTO = this._mapper.Map<TechniqueDTO>(technique);

        return techniqueDTO;
    }

    public async Task<IList<TechniqueDTO>> GetTechniquesList(int count)
    {
        IList<Technique> techniques = (await _techniqueRepository.GetAsync(x => true)).Take(count).ToList();

        IList<TechniqueDTO> techniqueDTOs = _mapper.Map<IList<TechniqueDTO>>(techniques);

        return techniqueDTOs;
    }

    public async Task MarkTechniqueAsCompleted(int id)
    {
        Technique? technique = await _techniqueRepository.FindByIdAsync(id);

        if (technique is null)
        {
            throw new TechniqueNotFoundException($"Техника с идентификатором {id} не найдена");
        }

        technique.MarkAsCompleted();

        await _techniqueRepository.UpdateAsync(technique);

        await _unitOfWork.Commit();
    }

    public async Task UpdateTechnique(TechniqueDTO techniqueDTO)
    {
        Technique technique = _mapper.Map<Technique>(techniqueDTO);

        await _techniqueRepository.UpdateAsync(technique);

        await _unitOfWork.Commit();
    }
}
