using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultManeger : MonoBehaviour
{
    int a;

    [SerializeField]
    string titleSceneName;

    public GameObject success;
    public GameObject failure;
    public GameObject back_image_suc;
    public GameObject back_image_fai;

    [SerializeField]
    Image successBackImage;
    [SerializeField]
    Image FailedBackImage;
    [SerializeField]
    Text mainText;

    [SerializeField]
    AudioSource successSE;
    [SerializeField]
    AudioSource FailedSE;
    [SerializeField]
    AudioSource butttonSE;

    // Start is called before the first frame update
    void Start()
    {
        a = PlayerPrefs.GetInt("result");

        // 'a' の値をチェックして成功か失敗かを判断します
        if (a == 1)
        {
            // 成功時のロジックをここに実装します
            Debug.Log("成功");

            successBackImage.gameObject.SetActive(true);
            successSE.Play();
            
            //success.SetActive(true);
            //failure.SetActive(false);
            //back_image_suc.SetActive(true);
            //back_image_fai.SetActive(false);
        }
        else if (a == 0)
        {
            // 失敗時のロジックをここに実装します
            Debug.Log("失敗");

            FailedBackImage.gameObject.SetActive(true);
            FailedSE.Play();

            //success.SetActive(false);
            //failure.SetActive(true);
            //back_image_suc.SetActive(false);
            //back_image_fai.SetActive(true);
        }
        else
        {
            Debug.LogWarning("予期しない結果値: " + a);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadNewScene()
    {
        Invoke("DelayScene", 1f);
        // SceneManager.LoadScene("ロードしたいシーン名");
        butttonSE.Play();
    }

    void DelayScene()
    {
        SceneManager.LoadScene(titleSceneName);
    }

    public void BackTitle()
    {
        LoadNewScene();
    }
}
