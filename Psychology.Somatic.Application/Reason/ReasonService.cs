using AutoMapper;
using PsychologyApp.Application.Helpers;
using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Common;
using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Services.ReasonService
{
    public class ReasonService : IReasonService
    {
        private readonly IGenericRepository<Reason> _reasonRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Mapper _mapper;

        public ReasonService(IGenericRepository<Reason> reasonRepository, IUnitOfWork unitOfWork, Mapper mapper)
        {
            this._reasonRepository = reasonRepository;
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<IList<ReasonDTO>> GetReasons(int count)
        {
            IList<Reason> reasons = (await this._reasonRepository.GetAsync(x => true)).Take(count).ToList();

            IList<ReasonDTO> reasonDTOs = this._mapper.Map<IList<ReasonDTO>>(reasons);

            return reasonDTOs;
        }

        public async Task SaveReasonsIfEmpty()
        {
            ReasonHelper psyhosomaticHelper = new();

            int count = (await this._reasonRepository.GetAsync(x => true)).Count();

            if (count > 0)
            {
                return;
            }

            IList<ReasonDTO> reasonDTOs = await psyhosomaticHelper.GetPsyhosomaticData();

            IList<Reason> reasons = this._mapper.Map<IList<Reason>>(reasonDTOs);

            await this._reasonRepository.InsertRangeAsync(reasons);

            await this._unitOfWork.Commit();
        }
    }
}
