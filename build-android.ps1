param([string]$Unity="")
$ErrorActionPreference="Stop"
$Root=(Resolve-Path $PSScriptRoot).Path
if([string]::IsNullOrWhiteSpace($Unity)){
  $candidate=Get-ChildItem "C:\Program Files\Unity\Hub\Editor\*\Editor\Unity.exe" -ErrorAction SilentlyContinue | Sort-Object FullName -Descending | Select-Object -First 1
  if(-not $candidate){ throw "Không tìm thấy Unity Editor. Cài Unity 6 + Android Build Support trong Unity Hub." }
  $Unity=$candidate.FullName
}
& $Unity -batchmode -quit -projectPath $Root -executeMethod VuonVietKyThu.Editor.BuildAndroid.PerformBuild -logFile "$Root\Builds\unity-build.log"
if($LASTEXITCODE -ne 0){ throw "Unity build thất bại. Xem Builds\unity-build.log" }
Write-Host "APK: $Root\Builds\VuonVietKyThu-2.0.0-unity-alpha.apk"
