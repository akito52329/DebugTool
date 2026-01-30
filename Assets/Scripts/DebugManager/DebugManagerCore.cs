#if UNITY_EDITOR || DEVELOPMENT_BUILD


using UnityEngine;
using System.Collections.Generic;
namespace DebugTool
{
    public partial class DebugManager : MonoBehaviour
    {
        private static DebugManager _instance;
        public static DebugManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<DebugManager>();
                return _instance;
            }
        }
        // デザイン設定
        private float _menuWidthPercent = 0.25f;
        private Color _themeColor = new Color(0.7f, 0.2f, 1f, 1f); // 鮮やかな紫
        private Color _accentColor = new Color(0.9f, 0.5f, 1f, 1f); // 発光部（薄い紫）
        private Color _bgDark = new Color(0.01f, 0.0f, 0.02f, 0.98f); // ほぼ真っ黒に近い紫


        private List<IDebugPage> _pages = new List<IDebugPage>();
        private List<DebugItem> _currentItems = new List<DebugItem>();
        private IDebugPage _activePage = null;

        private bool _isVisible = false;
        private int _selectedIndex = 0;
        private float _width = 450f;
        private float _itemHeight = 40f;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject); // シーンを跨いでも消えないように
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void OnGUI()
        {
            if (!_isVisible) return;

            float scale = Screen.height / 1080f;
            Matrix4x4 stack = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));

            float scaledWidth = Screen.width / scale;
            float scaledHeight = Screen.height / scale;
            float menuWidth = Mathf.Max(scaledWidth * _menuWidthPercent, 420f);

            // 呼吸アニメーション
            float breath = 0.8f + Mathf.Sin(Time.unscaledTime * 5f) * 0.2f;

            int count = _activePage == null ? _pages.Count : _currentItems.Count;
            float headerHeight = 80f;
            float footerHeight = 45f;
            float finalHeight = Mathf.Min(headerHeight + (count * _itemHeight) + footerHeight, scaledHeight);

            // --- 描画開始 ---
            float x = 0;
            float y = 0; // 左端に密着させてサイドバー感を強調

            // 1. メイン背景
            GUI.color = _bgDark;
            GUI.DrawTexture(new Rect(x, y, menuWidth, finalHeight), Texture2D.whiteTexture);

            // 右端の境界線
            GUI.color = _themeColor * 0.5f;
            GUI.DrawTexture(new Rect(x + menuWidth - 2, y, 2, finalHeight), Texture2D.whiteTexture);

            // 2. ヘッダー
            GUI.color = _themeColor;
            GUI.DrawTexture(new Rect(x, y, menuWidth, headerHeight), Texture2D.whiteTexture);

            // ヘッダーテキスト（影付き）
            string title = _activePage == null ? "SYSTEM_MANIFEST" : _activePage.GetName().ToUpper();
            DrawShadowLabel(new Rect(x + 30, y, menuWidth, headerHeight), title, 28, Color.black, Color.white, true);

            // 3. 項目リスト
            float startY = y + headerHeight + 10f;

            for (int i = 0; i < count; i++)
            {
                Rect r = new Rect(x, startY + (i * _itemHeight), menuWidth, _itemHeight);
                if (r.yMax > y + finalHeight - footerHeight) continue;

                bool isSelected = (i == _selectedIndex);

                if (isSelected)
                {
                    // 選択中：紫のグラデーション
                    for (int g = 0; g < 15; g++)
                    {
                        float alpha = (1.0f - (g / 15f)) * 0.4f * breath;
                        GUI.color = new Color(_themeColor.r, _themeColor.g, _themeColor.b, alpha);
                        GUI.DrawTexture(new Rect(r.x + (g * (r.width / 15f)), r.y, r.width / 15f, r.height), Texture2D.whiteTexture);
                    }
                    // 左端の強発光バー
                    GUI.color = _accentColor * breath;
                    GUI.DrawTexture(new Rect(r.x, r.y, 10, r.height), Texture2D.whiteTexture);
                }

                // --- 文字の描画（影付きで視認性向上） ---
                string label = _activePage == null ? _pages[i].GetName() : _currentItems[i].Label;
                Color textColor = isSelected ? Color.white : new Color(0.9f, 0.8f, 1f, 0.4f);

                // ラベル（左側）
                DrawShadowLabel(new Rect(r.x + (isSelected ? 40 : 30), r.y, r.width, r.height), label, 20, Color.black, textColor, isSelected);

                // 値（右側）
                if (_activePage != null)
                {
                    string valStr = _currentItems[i].GetValueString();
                    Color valColor = isSelected ? _accentColor : new Color(0.9f, 0.8f, 1f, 0.3f);
                    Rect vRect = new Rect(r.x, r.y, r.width - 40, r.height);
                    DrawShadowLabel(vRect, valStr, 20, Color.black, valColor, isSelected, TextAnchor.MiddleRight);
                }
            }

            // 4. フッター
            GUI.color = _themeColor * 0.2f;
            GUI.DrawTexture(new Rect(x, y + finalHeight - footerHeight, menuWidth, footerHeight), Texture2D.whiteTexture);
            DrawShadowLabel(new Rect(x, y + finalHeight - footerHeight, menuWidth, footerHeight), "W/S:NAVIGATE  SPACE:EXECUTE  BACK:RETURN", 14, Color.black, new Color(1, 1, 1, 0.5f), false, TextAnchor.MiddleCenter);

            GUI.matrix = stack;
            GUI.color = Color.white;
        }

        // --- 視認性向上のための影付きラベル関数 ---
        private void DrawShadowLabel(Rect rect, string text, int fontSize, Color shadowColor, Color textColor, bool isBold, TextAnchor anchor = TextAnchor.MiddleLeft)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = fontSize,
                fontStyle = isBold ? FontStyle.Bold : FontStyle.Normal,
                alignment = anchor,
                richText = true
            };

            // 影を描画 (1px ズラして4方向に描くことで縁取り効果)
            GUI.color = shadowColor;
            Rect sRect = rect;
            sRect.x += 1.5f; sRect.y += 1.5f;
            GUI.Label(sRect, text, style);

            // 本体を描画
            GUI.color = textColor;
            GUI.Label(rect, text, style);
            GUI.color = Color.white; // リセット
        }
    }
}
#endif