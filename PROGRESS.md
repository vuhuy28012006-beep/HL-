# PROGRESS — Evolution Sort (HL-)

Cập nhật lần cuối: 03/08/2026

## Đã hoàn thành

- [x] Restructure repo: Unity project nằm đúng ở root (Assets, ProjectSettings, Packages ngang hàng)
- [x] Canvas setup chuẩn: Reference Resolution 1920x1080, Scale With Screen Size, Match 0.5
- [x] `Card.prefab`: có Image (Preserve Aspect) + Text (TMP) con, đủ component EventCard + CardVisual
- [x] `EventData` (ScriptableObject): id, eventName, year, image
  - QUY ƯỚC: `year` là SỐ ÂM = số năm trước hiện tại (VD: LUCA = -4200000000). Sort tăng dần = đúng thứ tự tiến hóa.
- [x] `LevelData` (ScriptableObject): levelNumber, levelName, list events (5 EventData), maxMoves
- [x] `BoardManager.cs`: spawn card, xáo trộn, chọn/đổi chỗ (swap), đếm lượt, kiểm tra thắng/thua, Hint (nhấp nháy cặp sai gần nhất), Undo
- [x] `GameManager.cs`: quản lý state, bật/tắt WinPanel/LosePanel, RestartLevel(), BackToMenu() (placeholder)
- [x] UI hoàn chỉnh cho 1 màn chơi: WinPanel, LosePanel, nút Khởi động lại / Gợi ý / Hoàn tác, Text hiển thị lượt còn lại
- [x] Level 1 (Chương "Khởi Nguyên") chạy được toàn bộ vòng lặp: chọn → đổi chỗ → thắng/thua → panel hiện đúng

## Đang dở / cần làm tiếp

- [ ] **Hệ thống load Level theo tham số** — hiện `BoardManager.currentLevel` đang gán CỨNG 1 LevelData trong Inspector. Cần sửa để nhận LevelData truyền vào lúc load scene (từ Level Select), không thì mỗi màn phải sửa tay trong Editor. Đây là việc cần bàn kỹ với bạn code.
- [ ] Scene Level Select / Chapter Map — chưa có, cần danh sách 30 LevelData + UI chọn màn
- [ ] `BackToMenu()` trong GameManager mới là placeholder (chỉ Debug.Log), cần nối khi có scene Menu thật
- [ ] Chưa build thử APK lần nào — nên làm sớm sau khi Level Select xong để phát hiện lỗi Android sớm
- [ ] Sản xuất ảnh + tạo EventData/LevelData cho 29 màn còn lại (theo bảng thiết kế 30 màn đã có — file `evolution_sort_30_levels.md` nếu còn giữ, hoặc soạn lại)

## Quy ước kỹ thuật đã chốt 

- Target: Android APK only, landscape 16:9, canvas 1920x1080
- Card sinh vật: ảnh vuông 1024x1024 (hoặc 512x512), PNG nền trong suốt, style: oil painting, dark teal background, warm dramatic lighting, đã có prompt chuẩn
- Đặt tên file không dấu, không khoảng trắng
- `year` trong EventData luôn là số âm

## File code hiện có (đường dẫn trong repo)

```
Assets/Scripts/Core/GameManager.cs
Assets/Scripts/Core/GameState.cs
Assets/Scripts/Core/GameEvent.cs        (chưa dùng, có thể xóa sau)
Assets/Scripts/Data/EventData.cs
Assets/Scripts/Data/LevelData.cs
Assets/Scripts/GamePlay/Board/BoardManager.cs
Assets/Scripts/GamePlay/Card/EventCard.cs
Assets/Scripts/GamePlay/Card/CardVisual.cs
Assets/Prefabs/Card.prefab
Assets/Scenes/Gameplay.unity
```