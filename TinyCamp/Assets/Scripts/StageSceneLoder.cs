using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSceneLoder : MonoBehaviour
{
    // 新しいシーンをロードするためのメソッド
    public void LoadNewScene()
    {
        SceneManager.LoadScene("Result");
        // SceneManager.LoadScene("ロードしたいシーン名");

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // スペースキーを押した時
        {
            LoadNewScene(); // 新しいシーンをロード
        }
    }
}
