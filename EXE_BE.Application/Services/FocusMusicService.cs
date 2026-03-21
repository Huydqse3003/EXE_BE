using EXE_BE.Application.DTOs.Requests.FocusMusic;
using EXE_BE.Application.DTOs.Responses.FocusMusic;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class FocusMusicService : IFocusMusicService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FocusMusicService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FocusMusicResponse> AddAsync(CreateFocusMusicRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.AudioUrl))
            {
                throw new InvalidOperationException("Tên nhạc và đường dẫn nhạc không được để trống.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            var music = new FocusMusic
            {
                MusicId = Guid.NewGuid(),
                UserId = request.UserId,
                Title = request.Title.Trim(),
                Artist = request.Artist.Trim(),
                AudioUrl = request.AudioUrl.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.FocusMusics.AddAsync(music);

            await _unitOfWork.UserHabits.AddAsync(new UserHabit
            {
                HabitId = Guid.NewGuid(),
                UserId = user.UserId,
                HabitType = "MusicAdded",
                EventTime = DateTime.UtcNow,
                Details = $"Add music {music.Title}"
            });

            await _unitOfWork.SaveChangesAsync();

            return new FocusMusicResponse
            {
                MusicId = music.MusicId,
                UserId = music.UserId,
                Title = music.Title,
                Artist = music.Artist,
                AudioUrl = music.AudioUrl,
                CreatedAt = music.CreatedAt,
                IsCurrent = user.CurrentFocusMusicId == music.MusicId
            };
        }

        public async Task<IEnumerable<FocusMusicResponse>> GetByUserIdAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            var musics = await _unitOfWork.FocusMusics.FindAsync(m => m.UserId == userId);

            return musics
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new FocusMusicResponse
                {
                    MusicId = m.MusicId,
                    UserId = m.UserId,
                    Title = m.Title,
                    Artist = m.Artist,
                    AudioUrl = m.AudioUrl,
                    CreatedAt = m.CreatedAt,
                    IsCurrent = user.CurrentFocusMusicId == m.MusicId
                });
        }

        public async Task<FocusMusicResponse?> GetCurrentByUserIdAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            if (!user.CurrentFocusMusicId.HasValue)
            {
                return null;
            }

            var music = await _unitOfWork.FocusMusics.GetByIdAsync(user.CurrentFocusMusicId.Value);
            if (music == null)
            {
                return null;
            }

            return new FocusMusicResponse
            {
                MusicId = music.MusicId,
                UserId = music.UserId,
                Title = music.Title,
                Artist = music.Artist,
                AudioUrl = music.AudioUrl,
                CreatedAt = music.CreatedAt,
                IsCurrent = true
            };
        }

        public async Task<FocusMusicResponse> SetCurrentAsync(Guid userId, Guid musicId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            var music = await _unitOfWork.FocusMusics.GetByIdAsync(musicId)
                ?? throw new KeyNotFoundException("Không tìm thấy nhạc.");

            if (music.UserId != userId)
            {
                throw new InvalidOperationException("Bạn chỉ được chọn nhạc của chính mình.");
            }

            user.CurrentFocusMusicId = musicId;
            _unitOfWork.Users.Update(user);

            await _unitOfWork.UserHabits.AddAsync(new UserHabit
            {
                HabitId = Guid.NewGuid(),
                UserId = userId,
                HabitType = "MusicSetCurrent",
                EventTime = DateTime.UtcNow,
                Details = $"Set current music {music.Title}"
            });

            await _unitOfWork.SaveChangesAsync();

            return new FocusMusicResponse
            {
                MusicId = music.MusicId,
                UserId = music.UserId,
                Title = music.Title,
                Artist = music.Artist,
                AudioUrl = music.AudioUrl,
                CreatedAt = music.CreatedAt,
                IsCurrent = true
            };
        }
    }
}
