using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.FocusSession;
using EXE_BE.Application.DTOs.Responses.FocusSession;

namespace EXE_BE.Application.IServices
{
    public interface IFocusSessionService
    {
        Task<IEnumerable<FocusSessionResponse>> GetAllAsync();
        Task<FocusSessionResponse?> GetByIdAsync(Guid id);
        Task AddAsync(CreateFocusSessionRequest request);
        Task UpdateAsync(Guid id, UpdateFocusSessionRequest request);
        Task DeleteAsync(Guid id);
    }
}
