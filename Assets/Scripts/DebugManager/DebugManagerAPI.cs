#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections.Generic;

namespace DebugTool
{      
    
    // 汎用的な名前付きアクション構造体
    public struct DebugActionItem
    {
        public string Name;
        public Action Action;
        public DebugActionItem(string name, Action action) { Name = name; Action = action; }
    }


    public partial class DebugManager
    {
        public void AddPage(IDebugPage page) => _pages.Add(page);

        public void AddSlider(string label, float min, float max, Func<float> getter, Action<float> setter)
            => _currentItems.Add(new DebugItem { Label = label, Type = ItemType.Float, Getter = () => getter(), Setter = (v) => setter((float)v), Min = min, Max = max });

        public void AddInt(string label, int min, int max, Func<int> getter, Action<int> setter)
            => _currentItems.Add(new DebugItem { Label = label, Type = ItemType.Int, Getter = () => getter(), Setter = (v) => setter((int)v), Min = min, Max = max });

        public void AddToggle(string label, Func<bool> getter, Action<bool> setter)
            => _currentItems.Add(new DebugItem { Label = label, Type = ItemType.Bool, Getter = () => getter(), Setter = (v) => setter((bool)v) });

        public void AddEnum(string label, Enum current, Action<Enum> setter)
            => _currentItems.Add(new DebugItem { Label = label, Type = ItemType.Enum, Getter = () => current, Setter = (v) => setter((Enum)v), EnumValues = Enum.GetValues(current.GetType()) });

        public void AddAction(string label, Action action)
            => _currentItems.Add(new DebugItem { Label = label, Type = ItemType.Action, OnClick = action });

        public void AddListSelect<T>(string label, List<T> list, Func<T> getter, Action<T> setter, Action<T> onSelected)
            => _currentItems.Add(new DebugItem { Label = label, Type = ItemType.List, ListData = list, Getter = () => getter(), Setter = (v) => setter((T)v), OnClickList = (v) => onSelected?.Invoke((T)v) });

        /// <summary>
        /// アクションのリストを渡し、選択されたアクションを実行する
        /// </summary>
        public void AddActionList(string label, List<DebugActionItem> actions)
        {
            // この項目自体は「ページ遷移」のアクションとして登録する
            _currentItems.Add(new DebugItem
            {
                Label = label,
                Type = ItemType.Action, // 左右切り替えではなくボタンとして扱う
                OnClick = () =>
                {
                    // 決定されたら、新しい「アクション一覧ページ」を生成して入る
                    var actionPage = new ActionListPage(label, actions);
                    EnterPage(actionPage);
                }
            });
        }


    }

    // --- アクション一覧を表示するためのテンポラリページクラス ---
    public class ActionListPage : IDebugPage
    {
        private string _title;
        private List<DebugActionItem> _actions;

        public ActionListPage(string title, List<DebugActionItem> actions)
        {
            _title = title;
            _actions = actions;
        }

        public string GetName() => _title;

        public void Setup(DebugManager manager)
        {
            // リストの中身をすべて「Action」として登録する
            foreach (var item in _actions)
            {
                manager.AddAction(item.Name, item.Action);
            }
        }
    }
}
#endif

