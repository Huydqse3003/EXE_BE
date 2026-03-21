using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.VirtualRoom;
using EXE_BE.Application.DTOs.Responses.VirtualRoom;

namespace EXE_BE.Application.IServices
{
    public interface IVirtualRoomService
    {
        Task<IEnumerable<VirtualRoomResponse>> GetAllAsync();
        Task<VirtualRoomResponse?> GetByIdAsync(Guid id);
        Task AddAsync(CreateVirtualRoomRequest request);
        Task UpdateAsync(Guid id, UpdateVirtualRoomRequest request);
        Task DeleteAsync(Guid id);
    }
}
