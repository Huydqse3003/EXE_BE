using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.FocusSession;
using EXE_BE.Application.DTOs.Responses.FocusSession;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class FocusSessionService : IFocusSessionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FocusSessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<FocusSessionResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.FocusSessions.GetAllAsync();
            // TODO: Map from Entity to Response DTO
            return entities.Select(e => new FocusSessionResponse()); 
        }

        public async Task<FocusSessionResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.FocusSessions.GetByIdAsync(id);
            if (entity == null) return null;
            
            // TODO: Map from Entity to Response DTO
            return new FocusSessionResponse();
        }

        public async Task AddAsync(CreateFocusSessionRequest request)
        {
            // TODO: Map from Request DTO to Entity
            var entity = new FocusSession();
            
            await _unitOfWork.FocusSessions.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, UpdateFocusSessionRequest request)
        {
            var entity = await _unitOfWork.FocusSessions.GetByIdAsync(id);
            if (entity != null)
            {
                // TODO: Apply updates from Request DTO to Entity

                _unitOfWork.FocusSessions.Update(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.FocusSessions.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.FocusSessions.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
