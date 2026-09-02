package vn.vuonviet.kythu.bridge;

import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.webkit.WebSettings;
import com.unity3d.player.UnityPlayer;

public final class LegacySaveBridge {
    public static void startRead(final String gameObjectName) {
        final android.app.Activity activity = UnityPlayer.currentActivity;
        if (activity == null) return;
        activity.runOnUiThread(new Runnable() {
            @Override public void run() {
                try {
                    final WebView web = new WebView(activity);
                    WebSettings settings = web.getSettings();
                    settings.setJavaScriptEnabled(true);
                    settings.setDomStorageEnabled(true);
                    settings.setAllowFileAccess(true);
                    settings.setAllowContentAccess(true);
                    web.setWebViewClient(new WebViewClient() {
                        @Override public void onPageFinished(WebView view, String url) {
                            String js = "(function(){try{var s=localStorage.getItem('vvkt.full.save')||'';return btoa(unescape(encodeURIComponent(s)));}catch(e){return '';}})()";
                            view.evaluateJavascript(js, value -> {
                                String result = value == null ? "" : value;
                                if (result.length() >= 2 && result.startsWith("\"") && result.endsWith("\"")) result = result.substring(1, result.length()-1);
                                UnityPlayer.UnitySendMessage(gameObjectName, "OnLegacySave", result);
                                try { view.destroy(); } catch (Throwable ignored) {}
                            });
                        }
                    });
                    web.loadUrl("file:///android_asset/index.html");
                } catch (Throwable t) {
                    UnityPlayer.UnitySendMessage(gameObjectName, "OnLegacySave", "");
                }
            }
        });
    }
}
