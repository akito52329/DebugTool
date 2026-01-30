#if UNITY_EDITOR || DEVELOPMENT_BUILD

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DebugTool
{

    // --- 1. デバッグページが守るべきルール ---
    public interface IDebugPage
    {
        string GetName();
        void Setup(DebugManager manager);
    }
    public partial class DebugManager : MonoBehaviour
    {
        public void Start()
        {
            SetAddItem();
        }


        public void SetAddItem()
        {
            AddPage(new PlayerDebugPage());
        }
    }
}

#endif