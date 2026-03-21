using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.StudyRoomMember;
using EXE_BE.Application.DTOs.Responses.StudyRoomMember;

namespace EXE_BE.Application.IServices
{
    public interface IStudyRoomMemberService
    {
        Task<IEnumerable<StudyRoomMemberResponse>> GetAllAsync();
        Task<StudyRoomMemberResponse?> GetByIdAsync(Guid id);
        Task AddAsync(CreateStudyRoomMemberRequest request);
        Task UpdateAsync(Guid id, UpdateStudyRoomMemberRequest request);
        Task DeleteAsync(Guid id);
    }
}
