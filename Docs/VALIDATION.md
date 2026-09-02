# Validation — Vườn Việt Kỳ Thú 2.0.1 FIX

## PASS trong môi trường hiện tại
- `levels.json`: đúng 50 màn, 5 vùng.
- Màn 1–10: 28–30 lượt, 5 loại trái, goal-bias 24–35%, jelly tối đa 14.
- Màn 1–2 chỉ có 1 mục tiêu; màn 5/8/10 có special khởi đầu.
- Workload/move trung bình: màn 1–10 = 0,817; màn 11–20 = 1,894.
- Sinh thử 2.500 bàn theo thuật toán mới: không có match sẵn, luôn có nước đi.
- Số nước đi hợp lệ trung bình trên bàn đầu: màn 1–10 = 24,05; màn 11–20 = 14,79.
- Java `LegacySaveBridge.java`: compile syntax PASS bằng stub Android/UnityPlayer.
- Tất cả C# source: lexical/bracket balance PASS.
- Playtest keystore alias `vuonvietplaytest`: SHA-256 certificate giữ nguyên `D9:CB:4E:C3:3A:3B:B3:CA:2F:71:1E:92:C7:85:1E:28:AE:7D:86:82:99:CF:23:16:79:16:E4:B9:E1:E8:C8:CC`.

## Chưa thể PASS tại đây
- Unity package import/compile C# thực tế.
- IL2CPP Android build.
- Gradle/APK signing output từ Unity.
- FPS/thermal trên điện thoại thật.
- Legacy WebView save bridge trên thiết bị thật.

Lý do: container hiện tại không có Unity Editor/Unity Android Build Support và không có Unity license/runtime để chạy batch build.


## Static checks 2.0.1
- BuildAndroid imports `System` before using `Environment`.
- Runtime bootstrap creates EventSystem/InputModule and AudioListener if absent.
- Special and booster paths re-enter cascade resolution after refill.
- `FindSpecial` intersection check no longer relies on `FirstOrDefault()` sentinel index 0.
- Source package contains the full `Assets/Resources/Art/` tree and excludes `Signing/`.
