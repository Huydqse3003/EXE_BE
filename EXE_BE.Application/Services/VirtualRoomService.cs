using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.VirtualRoom;
using EXE_BE.Application.DTOs.Responses.VirtualRoom;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class VirtualRoomService : IVirtualRoomService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VirtualRoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<VirtualRoomResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.VirtualRooms.GetAllAsync();
            return entities.Select(e => new VirtualRoomResponse()); 
        }

        public async Task<VirtualRoomResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.VirtualRooms.GetByIdAsync(id);
            if (entity == null) return null;
            return new VirtualRoomResponse();
        }

        public async Task AddAsync(CreateVirtualRoomRequest request)
        {
            var entity = new VirtualRoom();
            await _unitOfWork.VirtualRooms.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, UpdateVirtualRoomRequest request)
        {
            var entity = await _unitOfWork.VirtualRooms.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.VirtualRooms.Update(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.VirtualRooms.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.VirtualRooms.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
