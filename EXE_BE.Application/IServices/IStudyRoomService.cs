using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.StudyRoom;
using EXE_BE.Application.DTOs.Responses.StudyRoom;

namespace EXE_BE.Application.IServices
{
    public interface IStudyRoomService
    {
        Task<IEnumerable<StudyRoomResponse>> GetAllAsync();
        Task<StudyRoomResponse?> GetByIdAsync(Guid id);
        Task AddAsync(CreateStudyRoomRequest request);
        Task UpdateAsync(Guid id, UpdateStudyRoomRequest request);
        Task DeleteAsync(Guid id);
    }
}
