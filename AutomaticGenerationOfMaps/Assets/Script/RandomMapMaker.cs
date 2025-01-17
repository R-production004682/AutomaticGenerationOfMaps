using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMapMaker : MonoBehaviour
{
    private float seedX, seedZ;

    [Header("---------実行中に変更できない変数--------")]
    [SerializeField, Tooltip("マップの横幅")]
    private int mapSizeWidth = 50;

    [SerializeField, Tooltip("マップの奥行き")]
    private int mapDepth = 50;

    [SerializeField, Tooltip("コライダーは必要？")]
    private bool checkNeedCollider = false;

    [Header("----------実行中に変更できる変数---------")]
    [SerializeField, Tooltip("マップサイズ")]
    private float mapSize = 1.0f;

    [SerializeField, Tooltip("高さの最大値")]
    private float mapMaxHeight = 10;

    [SerializeField, Tooltip("隆起の激しさ")]
    private float mapRelief = 10.0f;

    [SerializeField, Tooltip("パーリンノイズを使用している？")]
    private bool checkParlinNoiseMap = true;

    [SerializeField, Tooltip("Y座標を滑らかにする？")]
    private bool checkSmoothness = false;

    private void Awake()
    {
        // マップ全体のスケールを設定
        transform.localScale = Vector3.one * mapSize;

        // ノイズ生成用のランダムなシード値を設定
        seedX = Random.value * 100.0f;
        seedZ = Random.value * 100.0f;

        // マップの各位置にキューブを生成
        for (var x = 0; x < mapSizeWidth; x++)
        {
            for (var z = 0; z < mapDepth; z++)
            {
                // 新しいキューブを作成し、位置を設定
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localPosition = new Vector3(x, 0, z);
                cube.transform.SetParent(transform);

                // コライダーが不要なら削除
                if (!checkNeedCollider)
                {
                    Destroy(cube.GetComponent<BoxCollider>());
                }

                SetY(cube);
            }
        }
    }

    /// <summary>
    /// 実行中にインスペクタ上の値が変更された時に呼び出される
    /// </summary>
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        transform.localScale = Vector3.one * mapSize;

        // 各キューブの高さを再設定
        foreach (Transform child in transform)
        {
            SetY(child.gameObject);
        }
    }

    /// <summary>
    /// キューブのY座標を設定する
    /// </summary>
    /// <param name="cube"></param>
    private void SetY(GameObject cube)
    {
        float y = 0;

        // パーリンノイズを使って高さを決定
        if (checkParlinNoiseMap)
        {
            // 隆起の激しさが0以下だとエラーになるのでチェック
            if (mapRelief <= 0)
            {
                Debug.LogError("mapRelief の値が0以下。");
                return;
            }

            // パーリンノイズ用のサンプル値を計算
            float xSample = (cube.transform.localPosition.x + seedX) / mapRelief;
            float zSample = (cube.transform.localPosition.z + seedZ) / mapRelief;

            // パーリンノイズを使用して高さを決定
            float noise = Mathf.PerlinNoise(xSample, zSample);
            y = mapMaxHeight * noise;
        }
        else
        {
            y = Random.Range(0, mapMaxHeight);
        }

        // 高さを滑らかにしない場合は四捨五入
        if (!checkSmoothness)
        {
            y = Mathf.Round(y);
        }

        // キューブのY座標を設定
        cube.transform.localPosition = new Vector3(cube.transform.localPosition.x, y, cube.transform.localPosition.z);

        Color color = Color.black; // 岩盤をイメージして黒くする

        if (y > mapMaxHeight * 0.3f)
        {
            ColorUtility.TryParseHtmlString("#019540FF", out color); // 草っぽい色
        }
        else if (y > mapMaxHeight * 0.2f)
        {
            ColorUtility.TryParseHtmlString("#2432ADFF", out color); // 水っぽい色
        }
        else if (y > mapMaxHeight * 0.1f)
        {
            ColorUtility.TryParseHtmlString("#D4500EFF", out color); // マグマっぽい色
        }
        else
        {
            ColorUtility.TryParseHtmlString("#000000FF", out color); // 最下層は岩盤をイメージして黒
        }


        MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }
}
