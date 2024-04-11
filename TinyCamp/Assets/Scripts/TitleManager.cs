using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    AudioSource sourceBGM;
    [SerializeField]
    AudioSource sourceSE01;

    [SerializeField]
    string sceneName;

    [SerializeField]
    GameObject panelModeSelect;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("result", 1);
        panelModeSelect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// ノーマル：0,ハード:1
    /// </summary>
    /// <param name="_id"></param>
    public void StartGame(int _id)
    {
        PlayerPrefs.SetInt("game_mode", _id);
        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneName);
        sourceSE01.PlayOneShot(sourceSE01.clip);

    }

    // パネル開く
    public void OpenPanel()
    {
        panelModeSelect.SetActive(true);
        sourceSE01.PlayOneShot(sourceSE01.clip);

    }

    // パネル閉じる
    public void ClosePanel()
    {
        panelModeSelect.SetActive(false);
        sourceSE01.PlayOneShot(sourceSE01.clip);
    }



}
