using EXE_BE.Application.DTOs.Requests.Gameplay;
using EXE_BE.Application.DTOs.Responses.Gameplay;

namespace EXE_BE.Application.IServices
{
    public interface IGameplayService
    {
        Task<FocusSessionFlowResponse> StartFocusSessionAsync(StartFocusSessionRequest request);
        Task<FocusSessionFlowResponse> CompleteFocusSessionAsync(Guid sessionId);
        Task<PurchaseItemResponse> BuyItemWithCoinsAsync(BuyShopItemRequest request);
        Task<RoomDecorationResponse> DecorateRoomAsync(DecorateVirtualRoomRequest request);
        Task<JoinFriendRoomResponse> JoinFriendRoomAsync(JoinFriendRoomRequest request);
        Task<StudyRoomFlowResponse> CreateStudyRoomAsync(CreateStudyRoomFlowRequest request);
        Task<StudyRoomFlowResponse> JoinStudyRoomAsync(JoinStudyRoomRequest request);
        Task<StudyRoomFlowResponse> LeaveStudyRoomAsync(LeaveStudyRoomRequest request);
        Task<IEnumerable<ShopItemResponse>> GetShopItemsAsync();
    }
}
