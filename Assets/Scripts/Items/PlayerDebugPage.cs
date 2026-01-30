#if UNITY_EDITOR || DEVELOPMENT_BUILD


using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;
namespace DebugTool
{
    public class PlayerDebugPage : IDebugPage
    {

        public string GetName() => "プレイヤー設定";

        public void Setup(DebugManager manager)
        {
            List<DebugActionItem> list = new List<DebugActionItem>();
            list.Add(new DebugActionItem("TestAction", TestAction));
            list.Add(new DebugActionItem("CreateAtPosition", CreateAtPosition));
            list.Add(new DebugActionItem("CreateWithScaling", CreateWithScaling));
            list.Add(new DebugActionItem("CreateWithRotation", CreateWithRotation));
            list.Add(new DebugActionItem("CreateSphereShape", CreateSphereShape));
            list.Add(new DebugActionItem("CreateColoredCapsule", CreateColoredCapsule));
            list.Add(new DebugActionItem("CreateRandomObject", CreateRandomObject));
            list.Add(new DebugActionItem("TestAction", TestAction));


            manager.AddActionList("テスト", list);
        }

        public void TestAction()
        {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.blue;

        }
        // 位置（Position）を変更するパターン
        public void CreateAtPosition()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // (x: 5, y: 2, z: 0) の位置に配置
            cube.transform.position = GetRandomPosition();
        }

        // サイズ（Scale）を変更するパターン
        public void CreateWithScaling()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // 横長にする (x: 3倍, y: 0.5倍, z: 1倍)
            cube.transform.localScale = new Vector3(3f, 0.5f, 1f);
        }

        // 回転（Rotation）を変更するパターン
        public void CreateWithRotation()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // X軸を中心に45度回転させる
            cube.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
        }
        // 形状（PrimitiveType）を変えるパターン
        public void CreateSphereShape()
        {
            // 球体を作成
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        }

        // 色（Color）を正しく変更するパターン
        public void CreateColoredCapsule()
        {
            var capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);

            // 重要：.materials[0] ではなく .material にアクセスすることで
            // 実行時に生成されたマテリアルを直接操作できます
            capsule.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        public void CreateRandomObject()
        {
            // ランダムな形状を選択
            PrimitiveType[] types = { PrimitiveType.Cube, PrimitiveType.Sphere, PrimitiveType.Capsule };
            var obj = GameObject.CreatePrimitive(types[Random.Range(0, types.Length)]);

            // ランダムな位置と色
            obj.transform.position = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
            obj.GetComponent<MeshRenderer>().material.color = new Color(Random.value, Random.value, Random.value);
            obj.transform.position = GetRandomPosition();
            // ランダムな回転
            obj.transform.rotation = Random.rotation;
        }


        // 共通して使えるランダム位置生成関数
        private Vector3 GetRandomPosition()
        {
            float range = 5.0f; // 生成される範囲の広さ
            return new Vector3(
                Random.Range(-range, range),
                Random.Range(0, range), // 地面より上に出るように0〜
                Random.Range(-range, range)
            );
        }


    }
}
#endif