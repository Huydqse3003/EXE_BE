using EXE_BE.Application.DTOs.Requests.Gameplay;
using EXE_BE.Application.DTOs.Responses.Gameplay;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class GameplayService : IGameplayService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GameplayService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FocusSessionFlowResponse> StartFocusSessionAsync(StartFocusSessionRequest request)
        {
            if (request.DurationMinutes <= 0)
            {
                throw new InvalidOperationException("DurationMinutes phải lớn hơn 0.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            var topic = await _unitOfWork.Topics.GetByIdAsync(request.TopicId)
                ?? throw new KeyNotFoundException("Không tìm thấy topic.");

            if (topic.UserId != request.UserId)
            {
                throw new InvalidOperationException("Topic không thuộc về người dùng này.");
            }

            if (request.RoomId.HasValue)
            {
                var room = await _unitOfWork.StudyRooms.GetByIdAsync(request.RoomId.Value);
                if (room == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy phòng học.");
                }
            }

            var entity = new FocusSession
            {
                SessionId = Guid.NewGuid(),
                UserId = request.UserId,
                TopicId = request.TopicId,
                RoomId = request.RoomId,
                DurationMinutes = request.DurationMinutes,
                StartTime = DateTime.UtcNow,
                Status = "InProgress",
                EarnedCoins = 0,
                EarnedExp = 0
            };

            await _unitOfWork.FocusSessions.AddAsync(entity);
            await _unitOfWork.UserHabits.AddAsync(new UserHabit
            {
                HabitId = Guid.NewGuid(),
                UserId = request.UserId,
                HabitType = "FocusStart",
                EventTime = entity.StartTime,
                Details = $"Start focus session {entity.SessionId}"
            });
            await _unitOfWork.SaveChangesAsync();

            return new FocusSessionFlowResponse
            {
                SessionId = entity.SessionId,
                UserId = entity.UserId,
                TopicId = entity.TopicId,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                DurationMinutes = entity.DurationMinutes,
                Status = entity.Status,
                EarnedCoins = entity.EarnedCoins,
                EarnedExp = entity.EarnedExp,
                UserCoins = user.Coins,
                UserLevel = user.Level
            };
        }

        public async Task<FocusSessionFlowResponse> CompleteFocusSessionAsync(Guid sessionId)
        {
            var session = await _unitOfWork.FocusSessions.GetByIdAsync(sessionId)
                ?? throw new KeyNotFoundException("Không tìm thấy phiên học.");

            var user = await _unitOfWork.Users.GetByIdAsync(session.UserId)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            if (session.Status == "Completed")
            {
                return MapFocusSession(session, user);
            }

            var endTime = DateTime.UtcNow;
            session.EndTime = endTime;

            var actualFocusedMinutes = Math.Max(0, (int)(endTime - session.StartTime).TotalMinutes);
            var eligibleForCoins = actualFocusedMinutes >= 30;

            session.Status = eligibleForCoins ? "Completed" : "Interrupted";
            session.EarnedCoins = eligibleForCoins ? 30 : 0;
            session.EarnedExp = actualFocusedMinutes;

            user.ExperiencePoints += session.EarnedExp;
            user.Level = Math.Max(1, (user.ExperiencePoints / 100) + 1);

            if (eligibleForCoins)
            {
                user.Coins += session.EarnedCoins;

                var transaction = new UserCoinTransaction
                {
                    TransactionId = Guid.NewGuid(),
                    UserId = user.UserId,
                    Amount = session.EarnedCoins,
                    Source = "FocusSession",
                    ReferenceId = session.SessionId,
                    TransactionDate = endTime
                };

                await _unitOfWork.UserCoinTransactions.AddAsync(transaction);
            }

            _unitOfWork.Users.Update(user);

            await _unitOfWork.UserHabits.AddAsync(new UserHabit
            {
                HabitId = Guid.NewGuid(),
                UserId = user.UserId,
                HabitType = eligibleForCoins ? "FocusCompleted" : "FocusInterrupted",
                EventTime = endTime,
                Details = $"End focus session {session.SessionId}"
            });

            _unitOfWork.FocusSessions.Update(session);
            await _unitOfWork.SaveChangesAsync();

            return MapFocusSession(session, user);
        }

        public async Task<PurchaseItemResponse> BuyItemWithCoinsAsync(BuyShopItemRequest request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            var item = await _unitOfWork.GameItems.GetByIdAsync(request.ItemId)
                ?? throw new KeyNotFoundException("Không tìm thấy vật phẩm.");

            if (user.Level < item.UnlockLevel)
            {
                throw new InvalidOperationException($"Vật phẩm yêu cầu level {item.UnlockLevel}.");
            }

            if (item.IsPremiumOnly)
            {
                var activeSubscription = await _unitOfWork.UserSubscriptions.FindAsync(s =>
                    s.UserId == request.UserId && s.IsActive && s.EndDate >= DateTime.UtcNow);

                if (!activeSubscription.Any())
                {
                    throw new InvalidOperationException("Vật phẩm này yêu cầu gói premium đang hoạt động.");
                }
            }

            var existedUserItem = await _unitOfWork.UserItems.FindAsync(ui =>
                ui.UserId == request.UserId && ui.ItemId == request.ItemId);

            if (existedUserItem.Any())
            {
                throw new InvalidOperationException("Bạn đã sở hữu vật phẩm này.");
            }

            if (user.Coins < item.PriceCoins)
            {
                throw new InvalidOperationException("Không đủ coin để mua vật phẩm.");
            }

            var purchaseId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            user.Coins -= item.PriceCoins;
            _unitOfWork.Users.Update(user);

            await _unitOfWork.UserPurchases.AddAsync(new UserPurchase
            {
                PurchaseId = purchaseId,
                UserId = request.UserId,
                ItemId = request.ItemId,
                CoinsSpent = item.PriceCoins,
                PurchaseDate = now
            });

            await _unitOfWork.UserItems.AddAsync(new UserItem
            {
                UserId = request.UserId,
                ItemId = request.ItemId,
                AcquiredDate = now,
                IsEquipped = false
            });

            await _unitOfWork.UserCoinTransactions.AddAsync(new UserCoinTransaction
            {
                TransactionId = Guid.NewGuid(),
                UserId = request.UserId,
                Amount = -item.PriceCoins,
                Source = "StorePurchase",
                ReferenceId = purchaseId,
                TransactionDate = now
            });

            await _unitOfWork.UserHabits.AddAsync(new UserHabit
            {
                HabitId = Guid.NewGuid(),
                UserId = request.UserId,
                HabitType = "Purchase",
                EventTime = now,
                Details = $"Purchase item {item.Name} ({item.ItemId})"
            });

            await _unitOfWork.SaveChangesAsync();

            return new PurchaseItemResponse
            {
                PurchaseId = purchaseId,
                UserId = request.UserId,
                ItemId = request.ItemId,
                ItemName = item.Name,
                CoinsSpent = item.PriceCoins,
                RemainingCoins = user.Coins,
                PurchaseDate = now
            };
        }

        public async Task<RoomDecorationResponse> DecorateRoomAsync(DecorateVirtualRoomRequest request)
        {
            var room = await _unitOfWork.VirtualRooms.GetByIdAsync(request.VirtualRoomId)
                ?? throw new KeyNotFoundException("Không tìm thấy phòng ảo.");

            if (room.UserId != request.UserId)
            {
                throw new InvalidOperationException("Bạn không có quyền trang trí phòng này.");
            }

            var ownedItem = await _unitOfWork.UserItems.FindAsync(ui =>
                ui.UserId == request.UserId && ui.ItemId == request.ItemId);

            if (!ownedItem.Any())
            {
                throw new InvalidOperationException("Bạn chưa sở hữu vật phẩm này.");
            }

            var existedPosition = (await _unitOfWork.UserItemPositions.FindAsync(p =>
                p.UserId == request.UserId &&
                p.ItemId == request.ItemId &&
                p.VirtualRoomId == request.VirtualRoomId)).FirstOrDefault();

            if (existedPosition == null)
            {
                existedPosition = new UserItemPosition
                {
                    PositionId = Guid.NewGuid(),
                    UserId = request.UserId,
                    ItemId = request.ItemId,
                    VirtualRoomId = request.VirtualRoomId
                };

                await _unitOfWork.UserItemPositions.AddAsync(existedPosition);
            }

            existedPosition.X = request.X;
            existedPosition.Y = request.Y;
            existedPosition.Z = request.Z;
            existedPosition.ActionState = string.IsNullOrWhiteSpace(request.ActionState) ? "Static" : request.ActionState;
            existedPosition.MovementPattern = string.IsNullOrWhiteSpace(request.MovementPattern) ? "Stationary" : request.MovementPattern;
            existedPosition.Direction = string.IsNullOrWhiteSpace(request.Direction) ? "Down" : request.Direction;

            _unitOfWork.UserItemPositions.Update(existedPosition);

            await _unitOfWork.UserHabits.AddAsync(new UserHabit
            {
                HabitId = Guid.NewGuid(),
                UserId = request.UserId,
                HabitType = "RoomDecoration",
                EventTime = DateTime.UtcNow,
                Details = $"Decorate room {request.VirtualRoomId} with item {request.ItemId}"
            });

            await _unitOfWork.SaveChangesAsync();

            return new RoomDecorationResponse
            {
                PositionId = existedPosition.PositionId,
                UserId = existedPosition.UserId,
                ItemId = existedPosition.ItemId,
                VirtualRoomId = existedPosition.VirtualRoomId ?? Guid.Empty,
                X = existedPosition.X,
                Y = existedPosition.Y,
                Z = existedPosition.Z,
                ActionState = existedPosition.ActionState,
                MovementPattern = existedPosition.MovementPattern,
                Direction = existedPosition.Direction
            };
        }

        public async Task<JoinFriendRoomResponse> JoinFriendRoomAsync(JoinFriendRoomRequest request)
        {
            if (request.UserId == request.FriendUserId)
            {
                throw new InvalidOperationException("Không thể vào phòng của chính mình bằng chức năng này.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            var friend = await _unitOfWork.Users.GetByIdAsync(request.FriendUserId)
                ?? throw new KeyNotFoundException("Không tìm thấy bạn bè.");

            var friendships = await _unitOfWork.Friendships.FindAsync(f =>
                (f.UserId == request.UserId && f.FriendId == request.FriendUserId) ||
                (f.UserId == request.FriendUserId && f.FriendId == request.UserId));

            var isFriendAccepted = friendships.Any(f => string.Equals(f.Status, "Accepted", StringComparison.OrdinalIgnoreCase));
            if (!isFriendAccepted)
            {
                throw new InvalidOperationException("Hai người chưa là bạn bè (Accepted).");
            }

            var userOnline = await IsUserOnlineAsync(request.UserId);
            var friendOnline = await IsUserOnlineAsync(request.FriendUserId);
            if (!userOnline || !friendOnline)
            {
                throw new InvalidOperationException("Cả hai người cần online để vào phòng của nhau.");
            }

            var friendRoom = (await _unitOfWork.VirtualRooms.FindAsync(v => v.UserId == request.FriendUserId)).FirstOrDefault()
                ?? throw new KeyNotFoundException("Bạn bè chưa có phòng ảo.");

            var friendCharacter = (await _unitOfWork.RoomCharacters.FindAsync(rc =>
                rc.UserId == request.FriendUserId && rc.VirtualRoomId == friendRoom.VirtualRoomId)).FirstOrDefault();

            if (friendCharacter == null)
            {
                throw new InvalidOperationException("Bạn bè đang online nhưng chưa ở trong phòng của họ.");
            }

            var userCharacter = (await _unitOfWork.RoomCharacters.FindAsync(rc => rc.UserId == request.UserId)).FirstOrDefault();
            if (userCharacter == null)
            {
                userCharacter = new RoomCharacter
                {
                    CharacterId = Guid.NewGuid(),
                    UserId = request.UserId,
                    VirtualRoomId = friendRoom.VirtualRoomId,
                    X = 0,
                    Y = 0,
                    Z = 0,
                    ActionState = "Idle",
                    Direction = "Down"
                };

                await _unitOfWork.RoomCharacters.AddAsync(userCharacter);
            }
            else
            {
                userCharacter.VirtualRoomId = friendRoom.VirtualRoomId;
                userCharacter.ActionState = "Idle";
                userCharacter.Direction = "Down";
                _unitOfWork.RoomCharacters.Update(userCharacter);
            }

            await _unitOfWork.UserHabits.AddAsync(new UserHabit
            {
                HabitId = Guid.NewGuid(),
                UserId = user.UserId,
                HabitType = "JoinFriendRoom",
                EventTime = DateTime.UtcNow,
                Details = $"Join friend room {friendRoom.VirtualRoomId} of {friend.UserId}"
            });

            await _unitOfWork.SaveChangesAsync();

            return new JoinFriendRoomResponse
            {
                CharacterId = userCharacter.CharacterId,
                UserId = user.UserId,
                FriendUserId = friend.UserId,
                VirtualRoomId = friendRoom.VirtualRoomId,
                ActionState = userCharacter.ActionState,
                Direction = userCharacter.Direction
            };
        }

        public async Task<StudyRoomFlowResponse> CreateStudyRoomAsync(CreateStudyRoomFlowRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new InvalidOperationException("Tên phòng không được để trống.");
            }

            if (request.MaxCapacity <= 0)
            {
                throw new InvalidOperationException("Sức chứa phòng phải lớn hơn 0.");
            }

            if (request.IsPrivate && string.IsNullOrWhiteSpace(request.Passcode))
            {
                throw new InvalidOperationException("Phòng private cần passcode.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(request.CreatedByUserId)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng tạo phòng.");

            var room = new StudyRoom
            {
                RoomId = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Description = request.Description?.Trim() ?? string.Empty,
                MaxCapacity = request.MaxCapacity,
                IsPrivate = request.IsPrivate,
                Passcode = request.IsPrivate ? request.Passcode!.Trim() : string.Empty,
                CreatedByUserId = request.CreatedByUserId
            };

            await _unitOfWork.StudyRooms.AddAsync(room);

            await _unitOfWork.StudyRoomMembers.AddAsync(new StudyRoomMember
            {
                RoomId = room.RoomId,
                UserId = request.CreatedByUserId,
                JoinedAt = DateTime.UtcNow,
                IsActive = true,
                Role = "Owner"
            });

            await _unitOfWork.UserHabits.AddAsync(new UserHabit
            {
                HabitId = Guid.NewGuid(),
                UserId = user.UserId,
                HabitType = "StudyRoomCreated",
                EventTime = DateTime.UtcNow,
                Details = $"Create study room {room.RoomId}"
            });

            await _unitOfWork.SaveChangesAsync();

            return new StudyRoomFlowResponse
            {
                RoomId = room.RoomId,
                Name = room.Name,
                IsPrivate = room.IsPrivate,
                MaxCapacity = room.MaxCapacity,
                ActiveMembers = 1,
                IsDeleted = false,
                Message = "Tạo phòng học thành công."
            };
        }

        public async Task<StudyRoomFlowResponse> JoinStudyRoomAsync(JoinStudyRoomRequest request)
        {
            var room = await _unitOfWork.StudyRooms.GetByIdAsync(request.RoomId)
                ?? throw new KeyNotFoundException("Không tìm thấy phòng học.");

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            if (room.IsPrivate && !string.Equals(room.Passcode, request.Passcode, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Sai passcode phòng private.");
            }

            var members = (await _unitOfWork.StudyRoomMembers.FindAsync(m => m.RoomId == room.RoomId)).ToList();
            var activeCount = members.Count(m => m.IsActive);

            var existingMember = members.FirstOrDefault(m => m.UserId == request.UserId);
            if (existingMember != null && existingMember.IsActive)
            {
                return new StudyRoomFlowResponse
                {
                    RoomId = room.RoomId,
                    Name = room.Name,
                    IsPrivate = room.IsPrivate,
                    MaxCapacity = room.MaxCapacity,
                    ActiveMembers = activeCount,
                    IsDeleted = false,
                    Message = "Người dùng đã ở trong phòng học."
                };
            }

            if (existingMember == null && activeCount >= room.MaxCapacity)
            {
                throw new InvalidOperationException("Phòng học đã đầy.");
            }

            if (existingMember == null)
            {
                await _unitOfWork.StudyRoomMembers.AddAsync(new StudyRoomMember
                {
                    RoomId = room.RoomId,
                    UserId = request.UserId,
                    JoinedAt = DateTime.UtcNow,
                    IsActive = true,
                    Role = "Member"
                });
            }
            else
            {
                if (activeCount >= room.MaxCapacity)
                {
                    throw new InvalidOperationException("Phòng học đã đầy.");
                }

                existingMember.IsActive = true;
                existingMember.JoinedAt = DateTime.UtcNow;
                _unitOfWork.StudyRoomMembers.Update(existingMember);
            }

            await _unitOfWork.UserHabits.AddAsync(new UserHabit
            {
                HabitId = Guid.NewGuid(),
                UserId = user.UserId,
                HabitType = "StudyRoomJoined",
                EventTime = DateTime.UtcNow,
                Details = $"Join study room {room.RoomId}"
            });

            await _unitOfWork.SaveChangesAsync();

            var activeAfterJoin = activeCount + 1;

            return new StudyRoomFlowResponse
            {
                RoomId = room.RoomId,
                Name = room.Name,
                IsPrivate = room.IsPrivate,
                MaxCapacity = room.MaxCapacity,
                ActiveMembers = activeAfterJoin,
                IsDeleted = false,
                Message = "Vào phòng học thành công."
            };
        }

        public async Task<StudyRoomFlowResponse> LeaveStudyRoomAsync(LeaveStudyRoomRequest request)
        {
            var room = await _unitOfWork.StudyRooms.GetByIdAsync(request.RoomId)
                ?? throw new KeyNotFoundException("Không tìm thấy phòng học.");

            var member = (await _unitOfWork.StudyRoomMembers.FindAsync(m =>
                m.RoomId == request.RoomId && m.UserId == request.UserId)).FirstOrDefault()
                ?? throw new InvalidOperationException("Người dùng chưa ở trong phòng học.");

            member.IsActive = false;
            _unitOfWork.StudyRoomMembers.Update(member);

            var allMembers = (await _unitOfWork.StudyRoomMembers.FindAsync(m => m.RoomId == request.RoomId)).ToList();
            var activeAfterLeave = allMembers.Count(m => m.UserId != request.UserId && m.IsActive);

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            await _unitOfWork.UserHabits.AddAsync(new UserHabit
            {
                HabitId = Guid.NewGuid(),
                UserId = user.UserId,
                HabitType = "StudyRoomLeft",
                EventTime = DateTime.UtcNow,
                Details = $"Leave study room {room.RoomId}"
            });

            var deleted = false;
            if (activeAfterLeave == 0)
            {
                _unitOfWork.StudyRooms.Remove(room);
                deleted = true;
            }

            await _unitOfWork.SaveChangesAsync();

            return new StudyRoomFlowResponse
            {
                RoomId = room.RoomId,
                Name = room.Name,
                IsPrivate = room.IsPrivate,
                MaxCapacity = room.MaxCapacity,
                ActiveMembers = activeAfterLeave,
                IsDeleted = deleted,
                Message = deleted
                    ? "Phòng học đã được xóa vì không còn người dùng nào online trong phòng."
                    : "Rời phòng học thành công."
            };
        }

        public async Task<IEnumerable<ShopItemResponse>> GetShopItemsAsync()
        {
            var items = await _unitOfWork.GameItems.GetAllAsync();

            return items.Select(item => new ShopItemResponse
            {
                ItemId = item.ItemId,
                Name = item.Name,
                Description = item.Description,
                PriceCoins = item.PriceCoins,
                UnlockLevel = item.UnlockLevel,
                ItemType = item.ItemType,
                ImageUrl = item.ImageUrl,
                IsPremiumOnly = item.IsPremiumOnly,
                AnimalType = item.AnimalType,
                TargetAnimalType = item.TargetAnimalType
            });
        }

        private static FocusSessionFlowResponse MapFocusSession(FocusSession session, User user)
        {
            return new FocusSessionFlowResponse
            {
                SessionId = session.SessionId,
                UserId = session.UserId,
                TopicId = session.TopicId,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                DurationMinutes = session.DurationMinutes,
                Status = session.Status,
                EarnedCoins = session.EarnedCoins,
                EarnedExp = session.EarnedExp,
                UserCoins = user.Coins,
                UserLevel = user.Level
            };
        }

        private async Task<bool> IsUserOnlineAsync(Guid userId)
        {
            var habits = await _unitOfWork.UserHabits.FindAsync(h =>
                h.UserId == userId && (h.HabitType == "Login" || h.HabitType == "Logout"));

            var latestLogin = habits
                .Where(h => h.HabitType == "Login")
                .OrderByDescending(h => h.LoginTime ?? h.EventTime)
                .FirstOrDefault();

            if (latestLogin == null)
            {
                return false;
            }

            var loginAt = latestLogin.LoginTime ?? latestLogin.EventTime;
            if (loginAt < DateTime.UtcNow.AddHours(-2))
            {
                return false;
            }

            var latestLogout = habits
                .Where(h => h.HabitType == "Logout")
                .OrderByDescending(h => h.LogoutTime ?? h.EventTime)
                .FirstOrDefault();

            if (latestLogout == null)
            {
                return true;
            }

            var logoutAt = latestLogout.LogoutTime ?? latestLogout.EventTime;
            return logoutAt < loginAt;
        }
    }
}
