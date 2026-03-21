using EXE_BE.Application.DTOs.Requests.User;
using EXE_BE.Application.DTOs.Responses.User;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.Users.GetAllAsync();
            // TODO: Map from Entity to Response DTO
            return entities.Select(e => new UserResponse());
        }

        public async Task<UserResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.Users.GetByIdAsync(id);
            if (entity == null) return null;

            // TODO: Map from Entity to Response DTO
            return new UserResponse();
        }

        public async Task AddAsync(CreateUserRequest request)
        {
            // TODO: Map from Request DTO to Entity
            var entity = new User();

            await _unitOfWork.Users.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, UpdateUserRequest request)
        {
            var entity = await _unitOfWork.Users.GetByIdAsync(id);
            if (entity != null)
            {
                // TODO: Apply updates from Request DTO to Entity

                _unitOfWork.Users.Update(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.Users.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.Users.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
