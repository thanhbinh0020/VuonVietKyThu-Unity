# 2.0.1 FIX

Mục tiêu của bản này là loại các lỗi P0/P1 tìm thấy trong audit repo ngày 02/09/2026.

## Đã sửa
1. `BuildAndroid.cs`: thêm `using System;`, versionCode 21, signing chỉ bật khi có `VVKT_KEYSTORE_PATH`.
2. `GameBootstrap`: tự tạo EventSystem + StandaloneInputModule và AudioListener nếu scene chưa có.
3. `GameController`: special và booster tiếp tục resolve cascade sau refill.
4. `Match3Logic`: giao điểm T/L dùng tìm giao thực, không dùng index 0 làm sentinel.
5. Art: source FIX chứa lại toàn bộ PNG dưới `Assets/Resources/Art`.
6. Test: tránh NUnit làm vỡ assembly thường khi chưa tạo Test Assembly trong Unity; test runner chuẩn sẽ được cấu hình sau khi project import thành công.

## Còn cần Unity Editor để hoàn tất
- Mở bằng Unity 6000.0.40f1 để Unity import asset, sinh `.meta`/ProjectSettings còn thiếu.
- Chạy EditMode tests sau khi tạo Test Assembly chuẩn.
- Build IL2CPP Android và test máy thật.

Không có keystore trong gói này.
