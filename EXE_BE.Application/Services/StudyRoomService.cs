using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.StudyRoom;
using EXE_BE.Application.DTOs.Responses.StudyRoom;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class StudyRoomService : IStudyRoomService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudyRoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<StudyRoomResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.StudyRooms.GetAllAsync();
            // TODO: Map from Entity to Response DTO
            return entities.Select(e => new StudyRoomResponse()); 
        }

        public async Task<StudyRoomResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.StudyRooms.GetByIdAsync(id);
            if (entity == null) return null;
            
            // TODO: Map from Entity to Response DTO
            return new StudyRoomResponse();
        }

        public async Task AddAsync(CreateStudyRoomRequest request)
        {
            // TODO: Map from Request DTO to Entity
            var entity = new StudyRoom();
            
            await _unitOfWork.StudyRooms.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, UpdateStudyRoomRequest request)
        {
            var entity = await _unitOfWork.StudyRooms.GetByIdAsync(id);
            if (entity != null)
            {
                // TODO: Apply updates from Request DTO to Entity

                _unitOfWork.StudyRooms.Update(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.StudyRooms.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.StudyRooms.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
