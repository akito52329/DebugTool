#if UNITY_EDITOR || DEVELOPMENT_BUILD


using UnityEngine;
namespace DebugTool
{
    public partial class DebugManager
    {
        void Update()
        {
            // 表示切り替え: F1 または L3+R3
            bool gamepadTrigger = Input.GetKey(KeyCode.JoystickButton8) && Input.GetKeyDown(KeyCode.JoystickButton9);
            if (Input.GetKeyDown(KeyCode.F1) || gamepadTrigger)
            {
                _isVisible = !_isVisible;
            }

            if (!_isVisible) return;

            int max = _activePage == null ? _pages.Count : _currentItems.Count;
            if (max == 0) return;

            // 上下移動 (WASD / D-Pad)
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) _selectedIndex = (_selectedIndex - 1 + max) % max;
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) _selectedIndex = (_selectedIndex + 1) % max;

            // 決定 (Space / Enter / Aボタン)
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                if (_activePage == null) EnterPage(_pages[_selectedIndex]);
                else _currentItems[_selectedIndex].Execute();
            }

            // 戻る (Escape / Backspace / Bボタン)
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.JoystickButton1))
            {
                if (_activePage != null) ExitPage();
            }

            // 左右操作 (A/D / 左右キー)
            if (_activePage != null)
            {
                int dir = 0;
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) dir = 1;
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) dir = -1;
                if (dir != 0) _currentItems[_selectedIndex].ChangeValue(dir);
            }
        }

        private void EnterPage(IDebugPage page)
        {
            _activePage = page;
            _currentItems.Clear();
            _activePage.Setup(this);
            _selectedIndex = 0;
        }

        private void ExitPage()
        {
            _activePage = null;
            _selectedIndex = 0;
        }
    }
}
#endif