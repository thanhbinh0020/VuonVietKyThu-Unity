# Validation — Vườn Việt Kỳ Thú 2.0 Unity Alpha

## PASS trong môi trường hiện tại
- `levels.json`: đúng 50 màn, 5 vùng.
- Màn 1–10: 28–30 lượt, 5 loại trái, goal-bias 24–35%, jelly tối đa 14.
- Màn 1–2 chỉ có 1 mục tiêu; màn 5/8/10 có special khởi đầu.
- Workload/move trung bình: màn 1–10 = 0,817; màn 11–20 = 1,894.
- Sinh thử 2.500 bàn theo thuật toán mới: không có match sẵn, luôn có nước đi.
- Số nước đi hợp lệ trung bình trên bàn đầu: màn 1–10 = 24,05; màn 11–20 = 14,79.
- Java `LegacySaveBridge.java`: compile syntax PASS bằng stub Android/UnityPlayer.
- Tất cả C# source: lexical/bracket balance PASS.
- Signing keystore không được đóng gói trong bản source GitHub-clean. Giữ khóa ký và mật khẩu ở môi trường build riêng.

## Chưa thể PASS tại đây
- Unity package import/compile C# thực tế.
- IL2CPP Android build.
- Gradle/APK signing output từ Unity.
- FPS/thermal trên điện thoại thật.
- Legacy WebView save bridge trên thiết bị thật.

Lý do: container hiện tại không có Unity Editor/Unity Android Build Support và không có Unity license/runtime để chạy batch build.
