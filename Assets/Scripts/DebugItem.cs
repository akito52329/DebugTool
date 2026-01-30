#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DebugTool
{

    public enum ItemType { Float, Int, Bool, Enum, Action, List }

    public class DebugItem
    {
        public string Label;
        public ItemType Type;

        // 値の取得と反映用
        public Func<object> Getter;
        public Action<object> Setter;

        // 実行用（Action / List用）
        public Action OnClick;
        public Action<object> OnClickList;

        // パラメータ
        public float Min, Max;
        public Array EnumValues;
        public IList ListData; // List<T>を汎用的に扱うためIList

        // 表示用の文字列を返す
        public string GetValueString()
        {
            if (Type == ItemType.Action) return ">>>";
            if (Getter == null) return "";

            var val = Getter();
            if (Type == ItemType.Enum || Type == ItemType.List) return $"[ {val} ]";
            if (Type == ItemType.Bool) return (bool)val ? "ON" : "OFF";
            if (Type == ItemType.Float) return ((float)val).ToString("F2");
            return val.ToString();
        }

        // 左右入力での値変更
        public void ChangeValue(int dir)
        {
            switch (Type)
            {
                case ItemType.Float:
                    float fVal = (float)Getter() + (dir * 0.1f); // 変化量は必要に応じて調整
                    Setter(Mathf.Clamp(fVal, Min, Max));
                    break;

                case ItemType.Int:
                    int iVal = (int)Getter() + dir;
                    Setter((int)Mathf.Clamp(iVal, Min, Max));
                    break;

                case ItemType.Bool:
                    Setter(!(bool)Getter());
                    break;

                case ItemType.Enum:
                    if (EnumValues == null) return;
                    int eIdx = Array.IndexOf(EnumValues, Getter()) + dir;
                    if (eIdx >= 0 && eIdx < EnumValues.Length) Setter(EnumValues.GetValue(eIdx));
                    break;

                case ItemType.List:
                    if (ListData == null || ListData.Count == 0) return;

                    // 現在の「インデックス」を特定する
                    int lIdx = -1;
                    var currentVal = Getter(); // 現在表示されている名前(string)

                    // ListDataの中身が DebugActionItem の場合、Nameで一致を確認する
                    if (ListData is List<DebugActionItem> actionList)
                    {
                        lIdx = actionList.FindIndex(a => a.Name == (string)currentVal);
                    }
                    else
                    {
                        // 通常のリストの場合
                        for (int i = 0; i < ListData.Count; i++)
                        {
                            if (ListData[i].Equals(currentVal)) { lIdx = i; break; }
                        }
                    }

                    lIdx = Mathf.Clamp(lIdx + dir, 0, ListData.Count - 1);

                    // Setterに渡すものを決める
                    if (ListData is List<DebugActionItem> al)
                    {
                        Setter(al[lIdx].Name); // 名前をセット
                    }
                    else
                    {
                        Setter(ListData[lIdx]);
                    }
                    break;
            }
        }

        // 決定ボタン（Space/Enter）押下時の処理
        public void Execute()
        {
            if (Type == ItemType.Action)
            {
                OnClick?.Invoke();
            }
            else if (Type == ItemType.List)
            {
                // リストで現在選ばれているものを引数に渡して実行
                OnClickList?.Invoke(Getter());
            }
        }
    }
}
#endif