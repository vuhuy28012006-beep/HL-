# TÀI LIỆU THIẾT KẾ V2 — ĐA CHỦ ĐỀ (Concept mới)

> Thay thế tài liệu 30-màn-tiến-hóa cũ. Từ nay "Tiến Hóa Sinh Vật" chỉ là 1 trong nhiều Chương.
> Nguồn tra cứu, nguyên tắc chống trùng lặp/mơ hồ vẫn giữ nguyên như tài liệu V1 (PBDB, ICS, Wikipedia...).

---

## 1. NGUYÊN TẮC THIẾT KẾ (áp dụng cho MỌI chương, không riêng sinh vật)

1. **Luôn sort theo 1 giá trị số duy nhất** (`year`/`sortValue`) — với chủ đề không phải thời gian (nguyên tố, dung lượng dữ liệu...) vẫn dùng field này, chỉ đổi ý nghĩa (số hiệu nguyên tử, số byte...).
2. **Chủ đề dạng CHU TRÌNH (không có điểm đầu/cuối tự nhiên)** — vòng tuần hoàn nước, chuỗi thức ăn, chu kỳ sống động vật — **bắt buộc cố định 1 thẻ làm mốc bắt đầu** (không cho di chuyển thẻ đó), nếu không người chơi không có cách nào biết đâu là "đúng" vì chu trình xoay vòng có thể bắt đầu từ bất kỳ đâu.
3. **Không dùng cặp thực sự đồng hạng/mơ hồ** trong cùng 1 màn (đã áp dụng ở chương Tiến Hóa, áp dụng tương tự chương khác — vd 2 nguyên tố cùng nhóm dễ nhầm, 2 phát minh cùng năm).
4. **Độ khó trong 1 Chương tăng dần theo thứ tự màn** — Level đầu chương luôn dễ nhất (khoảng cách xa, object khác biệt rõ), Level cuối khó nhất (khoảng cách hẹp, dễ nhầm).
5. **Chủ đề mà số lượng đối tượng "ai cũng biết đủ hết"** (VD: 8 hành tinh — người chơi biết ngay có 8 thẻ) → ứng cử viên tốt cho tính năng "Ẩn thẻ" (mục 2) để tránh việc đoán mò bằng trí nhớ thay vì suy luận.

---

## 2. GHI CHÚ VỀ CÁC TÍNH NĂNG GAMEPLAY (bạn tự chọn gán vào màn nào)

Đây là kho tính năng đã thống nhất — liệt kê để tài liệu đầy đủ, KHÔNG gán sẵn vào màn cụ thể.

| Tính năng | Mô tả | Trạng thái |
|---|---|---|
| Free Swap | Đổi chỗ tự do 2 thẻ bất kỳ (cơ chế mặc định hiện tại) | Đã có |
| Bubble Swap | Chỉ được đổi chỗ 2 thẻ liền kề nhau (mô phỏng Bubble Sort) | Đã làm |
| Selection Swap | Mỗi lượt chọn 1 vị trí "chốt", đổi phần tử nhỏ nhất trong phần còn lại vào đó | Đã làm |
| Insertion Swap | Rút 1 thẻ, chèn vào đúng vị trí trong dãy đã sắp 1 phần | Chưa làm |
| Tháp Hà Nội | Biến thể đặc biệt, mượn cơ chế 3 cột, cần thiết kế luật riêng | Chưa làm |
| Ẩn thẻ | Không hiện ảnh/tên cho tới khi chọn, buộc suy luận thay vì nhớ mặt | Chưa làm|
| Time Rush | Giới hạn thời gian thay vì/thêm giới hạn lượt | Chưa làm |
| Limited Swaps | Giới hạn số lượt đổi (maxMoves) | Đã có |
| Fog Mode | Chỉ hiện rõ 2-3 thẻ gần vị trí thao tác, còn lại mờ | Chưa làm — để dành, ghi nhận ý tưởng, làm sau |
| Memory Mode | Hiện đúng thứ tự/ảnh trong X giây đầu rồi úp lại, chơi theo trí nhớ | đã làm |

---

## 3. DANH SÁCH CHƯƠNG (đã thiết kế đầy đủ level)

### CHƯƠNG 0 — Hướng Dẫn (Tutorial)

- Level 0.1 — Học Swap: 3 thẻ hình khối màu đơn giản, số 1-2-3, dạy thao tác chọn + đổi chỗ.
- Level 0.2 — Học Gợi Ý loại 1 (Hiện thứ tự tạm thời).
- Level 0.3 — Học Gợi Ý loại 2 (Gợi ý cặp cần đổi — cơ chế ShowHint hiện tại).

Ghi chú kỹ thuật: hiện code chỉ có 1 loại Hint (nhấp nháy cặp sai). Cần thêm hàm Hint kiểu 2 (hiện đáp án tạm thời vài giây) — việc code, ghi vào PROGRESS.md khi làm.

---

### CHƯƠNG 1 — Tiến Hóa Sinh Vật

Đề xuất 8 màn đại diện (dễ → khó), xem file thiết kế cũ nếu muốn lấy trọn bộ 30 màn:

1. LUCA / Vi khuẩn lam / Eukaryote / Đa bào / Ediacara (đã làm)
2. Trilobite / Haikouichthys / Thực vật lên cạn / Cá có hàm / Cá vây tay (đã làm)
3. Ichthyostega / Hylonomus / Meganeura / Dimetrodon / Eoraptor
4. Morganucodon / Archaeopteryx / T-rex / Titanoboa / Australopithecus
5. Placerias / Plateosaurus / Postosuchus / Coelophysis / Dilophosaurus
6. Iguanodon / Deinonychus / Sarcosuchus / Spinosaurus / Argentinosaurus
7. Basilosaurus / Australopithecus / Smilodon / Homo erectus / Homo sapiens
8. (khó nhất) Velociraptor / Pachycephalosaurus / Quetzalcoatlus / Triceratops / Ankylosaurus

---

### CHƯƠNG 2 — Lịch Sử

Level 1 — Lịch Sử Việt Nam (Dễ):
1. Nhà nước Văn Lang (~2879 TCN) 2. Khởi nghĩa Hai Bà Trưng (40 SCN) 3. Chiến thắng Bạch Đằng - Ngô Quyền (938) 4. Vua Gia Long lập triều Nguyễn (1802) 5. Cách mạng Tháng Tám (1945)

Level 2 — Lịch Sử Việt Nam (Khó hơn):
1. Chiến thắng Điện Biên Phủ (1954) 2. Hiệp định Genève (1954, cần ghi rõ tháng vì trùng năm) 3. Chiến dịch Hồ Chí Minh (1975) 4. Việt Nam gia nhập ASEAN (1995) 5. Việt Nam gia nhập WTO (2007)

Level 3 — Lịch Sử Thế Giới (Dễ):
1. Kim tự tháp Giza (~2560 TCN) 2. Đế chế La Mã thành lập (27 TCN) 3. Đế chế La Mã sụp đổ (476 SCN) 4. Cách mạng Pháp (1789) 5. Thế chiến II kết thúc (1945)

Level 4 — Lịch Sử Thế Giới (Khó hơn):
1. Cách mạng Công nghiệp (~1760) 2. Tuyên ngôn Độc lập Hoa Kỳ (1776) 3. Thế chiến I bắt đầu (1914) 4. Liên Xô sụp đổ (1991) 5. Internet thương mại hóa (~1995)

Level 5 (bổ sung, tùy chọn) — Các Tên Gọi Của Việt Nam:
Văn Lang -> Âu Lạc -> Vạn Xuân -> Đại Cồ Việt -> Đại Việt/Việt Nam (cần tra cứu chính xác niên đại từng tên gọi)

---

### CHƯƠNG 3 — Nguyên Tố Hóa Học

Sort theo số hiệu nguyên tử (Z).

Level 1 — Dễ: Hydro (Z=1), Carbon (Z=6), Oxy (Z=8), Sắt (Z=26), Vàng (Z=79)

Level 2 — Khó (cùng chu kỳ, sát nhau): Natri (Z=11), Magie (Z=12), Nhôm (Z=13), Silic (Z=14), Photpho (Z=15)

Gợi ý Level 3 "rất khó": dùng nhóm kim loại chuyển tiếp sát Z (Fe=26, Co=27, Ni=28...)

---

### CHƯƠNG 4 — Những Phát Minh Vĩ Đại

1. Bánh xe (~3500 TCN) 2. Giấy (~105 SCN, Thái Luân) 3. Máy in (1440, Gutenberg) 4. Điện thoại (1876, Bell) 5. Bóng đèn điện (1879, Edison — có tranh cãi, cần đối chiếu)

Gợi ý Level 2 khó hơn: phát minh thế kỷ 20 gần nhau (radio, TV, máy tính, internet, điện thoại di động)

---

### CHƯƠNG 5 — Hệ Mặt Trời

Sort theo khoảng cách tới Mặt Trời: Sao Thủy, Sao Kim, Trái Đất, Sao Hỏa, Sao Mộc, Sao Thổ, Sao Thiên Vương, Sao Hải Vương

Ghi chú quan trọng: ứng viên hàng đầu cho tính năng "Ẩn thẻ" — vì hầu như ai cũng biết có 8 hành tinh và thứ tự gần đúng, không ẩn thì độ khó gần như bằng 0.

---

### CHƯƠNG 6 — Toán Học (đề xuất dùng Time Rush)

Level 1 — Cơ bản (kết quả tăng dần: 5, 6, 8, 9, 10):
3+2(=5), 10-4(=6), 2x4(=8), 3^2(=9), 20/2(=10)

Level 2 — Khó (kết quả tăng dần: 16, 18, 20, 21, 23):
4^2(=16), 30-12(=18), 100/5(=20), 7x3(=21), 15+8(=23)

---

## 4. CHƯƠNG Ý TƯỞNG — CHƯA THIẾT KẾ LEVEL CHI TIẾT (để dành)

Gom nhóm theo kiến thức nền chung, mỗi nhóm sẽ là 1 Chương riêng khi làm tới:

Chương (dự kiến) — Sinh Học: Chu Trình & Cấu Trúc Sự Sống (khác Chương 1, kiến thức phi thời gian tuyến tính):
- Chu kỳ sống bướm/ếch
- Chuỗi thức ăn (cỏ -> châu chấu -> ếch -> rắn) — bắt buộc cố định thẻ đầu
- Vòng tuần hoàn nước — bắt buộc cố định thẻ đầu
- Cấp độ tổ chức sống (nguyên tử -> tế bào -> mô -> cơ quan -> hệ cơ quan -> cơ thể) — tuyến tính thật, không cần cố định thẻ

Chương (dự kiến) — Công Nghệ & Khoa Học Máy Tính:
- Cách mạng CNTT (bàn tính -> transistor -> PC -> smartphone -> AI)
- Ngôn ngữ lập trình theo năm ra đời (Assembly -> Fortran -> C -> ...)
- Dung lượng dữ liệu (bit -> byte -> KB -> MB -> GB -> TB)

Chương (dự kiến) — Vật Lý & Vũ Trụ:
- Kích thước vũ trụ (proton -> phân tử -> tế bào -> người -> hành tinh -> sao -> thiên hà)
- Phổ điện từ (radio -> vi sóng -> hồng ngoại -> ánh sáng -> tia X -> tia gamma)
- Âm nhạc / cao độ (sort theo tần số Hz)

Chương (dự kiến) — Địa Lý Thế Giới:
- Quốc kỳ/quốc gia theo diện tích
- (mở rộng sau: dân số, GDP...)

---

## 5. LƯU Ý KỸ THUẬT LIÊN QUAN

- Chưa sửa lỗi trùng số level giữa các Chương trong SaveManager — ưu tiên sửa sớm khi số Chương tăng nhanh.
- Chương 0 (Tutorial) không cần EventData thật, dùng ảnh/số đơn giản, không cần gen ảnh AI.
