# Vườn Việt Kỳ Thú 2.0.1 FIX — Unity Migration

Nhánh này chuyển gameplay/render từ WebView sang Unity, giữ package Android `vn.vuonviet.kythu` và playtest keystore cũ để chuẩn bị cài đè trong quá trình thử nghiệm.

## Mục tiêu kiến trúc
- Unity render/UI thay cho WebView DOM/CSS.
- 8×8 match-3 native C# với 64 `TileView` tái sử dụng.
- Trái cây, booster, mascot, 5 vùng Việt Nam và background được chuyển thành PNG để Unity import ổn định.
- 3 chế độ đồ họa: Mượt / Auto / 3D Max.
- Haptic Android, immersive portrait, safe-area cho máy màn hình dài/notch.
- Shop bằng xu, 12 vật phẩm/gói, quà ngày, cosmetic mascot.
- Mascot Mai có idle/bob/wave/cheer 2.5D; khung sẵn để thay bằng rig 2D/Spine/3D sau.
- Có bridge thử đọc `vvkt.full.save` từ WebView cũ khi cài đè cùng package/signature.

## 10 màn đầu đã làm dễ
Màn 1–10 dùng 5 loại trái thay vì 6, 28–30 lượt, goal bias 24–35%, jelly tăng rất chậm, và màn 5/8/10 có special khởi đầu. Nếu thua trong 10 màn đầu, lần thử sau được hỗ trợ động: tối đa +5 lượt và tăng tỉ lệ rơi trái mục tiêu. Từ màn 11 mới quay về 6 loại trái và tăng workload rõ rệt.

## Build APK
Yêu cầu Unity 6 (project khai báo 6000.0.40f1) và module Android Build Support + SDK/NDK/OpenJDK.

Windows PowerShell:
```powershell
.\build-android.ps1
```
Hoặc trong Unity: **Vườn Việt → Build Android APK**.

APK dự kiến:
`Builds/VuonVietKyThu-2.0.2-build-ready.apk`

## Lưu ý quan trọng
Môi trường ChatGPT hiện tại không cài Unity Editor/Android module, vì vậy source đã được tạo và kiểm tra tĩnh nhưng chưa thể trung thực khẳng định đã chạy Unity compile/IL2CPP/Gradle hay tạo APK Unity tại đây. Build script đã cấu hình package, versionCode 22 và playtest keystore cũ.


## 2.0.1 FIX
- Sửa compile `BuildAndroid.cs` khi đọc biến môi trường signing.
- Tạo runtime `EventSystem + StandaloneInputModule` để nút/tile nhận chạm ngay cả khi scene được tạo trống.
- Tạo `AudioListener` dự phòng để SFX có đầu ra âm thanh.
- Special/booster giờ tiếp tục resolve cascade sau refill.
- Sửa phát hiện giao điểm T/L để không nhận nhầm ô 0 thành Bomb.
- Giữ toàn bộ PNG art trong `Assets/Resources/Art/`; không chứa keystore.
- EditMode test được chặn khỏi assembly thường bằng `UNITY_INCLUDE_TESTS`; khi thiết lập Test Assembly trong Unity có thể bật lại test runner chuẩn.

## Cloud build (GitHub Actions)
Repo có workflow `.github/workflows/build-android.yml` để build APK bằng Unity 6000.0.40f1 trên GitHub Actions. Vì Unity Editor bắt buộc kích hoạt license, hãy cấu hình `UNITY_LICENSE`, `UNITY_EMAIL`, `UNITY_PASSWORD` trong GitHub Actions Secrets; không ghi các giá trị này vào source hoặc chat công khai.
