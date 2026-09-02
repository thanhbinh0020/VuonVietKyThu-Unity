# 2.0.2 BUILD READY

Pre-build hardening for Unity 6000.0.40f1.

- Android application entry is forced to `Activity` to match the custom `UnityPlayerActivity` manifest and legacy save bridge. Unity 6 defaults new projects to GameActivity.
- APK output is forced (`buildAppBundle=false`).
- Version: `2.0.2-build-ready`, versionCode `22`.
- Art importer keeps 720x1280 backgrounds/regions at full source resolution (`maxTextureSize=2048`) instead of downscaling to 1024.
- Android textures use ETC2 for broad GLES3 compatibility; transparent art uses ETC2 RGBA8.
- No signing key is committed. Use the private playtest keystore at build time if an in-place upgrade from the old package is required.

## GitHub Actions signing (optional)
Nếu muốn APK cài đè bản playtest cũ mà không mất dữ liệu, workflow hỗ trợ 4 GitHub Actions Secrets: `ANDROID_KEYSTORE_BASE64`, `ANDROID_KEYSTORE_PASS`, `ANDROID_KEYALIAS_NAME`, `ANDROID_KEYALIAS_PASS`. Workflow tạo file signing tạm trong `.ci-signing/` (đã gitignore), Unity đọc file này qua `-vvktSigningProperties`; secret không được ghi vào repo hay command line.

Nếu không cấu hình 4 secret Android signing, Unity dùng debug signing và APK thử nghiệm có thể yêu cầu gỡ bản cũ trước khi cài.
