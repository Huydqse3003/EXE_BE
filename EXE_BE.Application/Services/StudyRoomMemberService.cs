using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.StudyRoomMember;
using EXE_BE.Application.DTOs.Responses.StudyRoomMember;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class StudyRoomMemberService : IStudyRoomMemberService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudyRoomMemberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<StudyRoomMemberResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.StudyRoomMembers.GetAllAsync();
            // TODO: Map from Entity to Response DTO
            return entities.Select(e => new StudyRoomMemberResponse()); 
        }

        public async Task<StudyRoomMemberResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.StudyRoomMembers.GetByIdAsync(id);
            if (entity == null) return null;
            
            // TODO: Map from Entity to Response DTO
            return new StudyRoomMemberResponse();
        }

        public async Task AddAsync(CreateStudyRoomMemberRequest request)
        {
            // TODO: Map from Request DTO to Entity
            var entity = new StudyRoomMember();
            
            await _unitOfWork.StudyRoomMembers.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, UpdateStudyRoomMemberRequest request)
        {
            var entity = await _unitOfWork.StudyRoomMembers.GetByIdAsync(id);
            if (entity != null)
            {
                // TODO: Apply updates from Request DTO to Entity

                _unitOfWork.StudyRoomMembers.Update(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.StudyRoomMembers.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.StudyRoomMembers.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
