# Migration status — 2.0.1 FIX

## Đã có trong source
- 50 level definitions + 5 vùng.
- First-10 easy curve + dynamic assist.
- Match-3 C#: swap, match, cascade, shuffle, goals, jelly.
- Special: rocket row/column, bomb, rainbow.
- Booster: Giỏ tre, Quạt gió, Chong chóng.
- Energy/combo meter + Bùng nổ Vườn.
- Home, Map, Game, Shop, Character, Events, Settings, modal/result UI tạo runtime.
- Shop 12 items, daily free gift, daily -30% deal, cosmetics.
- Hearts + hồi 30 phút; lose assist.
- 3 quality modes, haptic, safe-area, immersive portrait.
- Mascot idle/cheer 2.5D.
- Legacy WebView save bridge thử migrate level/xu/tim/booster/cosmetics/stars.
- Android build automation; signing key không nằm trong source public.

## Chưa thể xác minh tại môi trường hiện tại
- Unity C# compiler/package resolution.
- IL2CPP Android build + Gradle packaging.
- FPS/thermal trên điện thoại thật.
- Legacy WebView localStorage bridge trên OEM WebView cụ thể.
- Animation rig xương/Spine/3D thật (hiện là 2.5D motion).

## Bước sau alpha
Sau khi build/chạy được trên máy thật: profiler → object pooling FX → Addressables/atlases → rig mascot → shader/VFX Graph/URP 2D → obstacle mới → 100+ màn → cloud save/live-ops/IAP nếu phát hành thương mại.


## Sửa trong 2.0.1 FIX
- Runtime input service: EventSystem + StandaloneInputModule.
- Runtime AudioListener fallback.
- Cascade sau special/booster.
- Giao điểm T/L không còn false-positive tại index 0.
- Build script dùng versionName `2.0.1-fix`, versionCode `21`.
- Bộ art PNG đầy đủ được giữ lại trong source.
