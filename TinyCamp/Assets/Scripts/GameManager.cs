using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;

public class GameManager : MonoBehaviour
{
    #region 列挙体

    // 天気情報の列挙
    public enum Weather
    {
        SUNNY,
        CLOUDY,
        RAINY,
        RAIN_STORM,

        LENGTH
    }

    // ゲームの状態
    public enum GameState
    {
        GAME_CLEAR,
        GAME_OVER,
        DEFAULT
    }

    // キャンプの状態、時間経過で完成する
    public enum CampState
    {
        FASE0,
        FASE1,
        FASE2,
        FASE3,
        FASE4,
        FASE5,
        FASE6
    }


    #endregion


    public float tmp = -4.8f;

    // 鳥
    [Header("カラス系")]
    Crow crow;
    bool isEncount;
    float crowTimer;
    const float CROW_RETIME = 15f;
    const float CROW_RAND = 2f;

    [Header("小人")]
    Dwarf dwarf;


    const float GAUGE_SEC = 25f;
    float gaugeSp = 1f / GAUGE_SEC;

    const int COUNT_CLOUDY = 10;
    const int COUNT_RAINY = 20;
    const int COUNT_RAIN_STORM = 50;

    [Header("シーンの名前")]
    [SerializeField]
    string resultSceneName;

    [Header("左上の天気図")]
    [SerializeField]
    GameObject rotatableWeather;
    readonly int[] WEATHERS_RAD = new int[4] { 180, 90, 0, 270 };
    // 回転タイマー
    float rotateTimer;
    // 回転中フラグ
    bool isRotation;
    // 初期位置から目標角度までのずれ
    float radDiffStartEnd;
    // 角度ずれの閾値
    const float RAD_DIFF = 3f;
    // 目標角度まで回転するときにかかる時間
    const float RAD_MOVE_SEC = 1f;
    [SerializeField]
    GameObject rainyEffect;
    [SerializeField]
    Image imageBack;
    [SerializeField]
    Sprite[] sourcesBackImage;

    [Header("キャンプ")]
    [SerializeField]
    GameObject[] campObjs;
    [SerializeField]
    Sprite picnik01;
    [SerializeField]
    Sprite picnik02;
    [SerializeField]
    Sprite tent01;
    [SerializeField]
    Sprite tent02;

    [Header("攻撃ボタン")]
    [SerializeField]
    GameObject bowButton;

    public int cloudCnt;


    [SerializeField]
    Image imageCampGauge;

    public GameState gameState { private set; get; }
    Weather weather;



    bool isStart;
    [SerializeField]
    AudioSource bgm;
    [SerializeField]
    AudioSource rainBgm;// mute
    [SerializeField]
    AudioSource crowLoop;// mute

    [SerializeField]
    AudioSource[] se;

    [SerializeField]
    AudioClip bowSe;

    [SerializeField]
    GameObject tutoPanel;


    CampState campFase;

    // Start is called before the first frame update
    void Start()
    {
        // 初期化
        Init();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Init()
    {
        // 初期化
        campFase = CampState.FASE0;
        imageCampGauge.fillAmount = 0;
        SetWeather(Weather.SUNNY);
        gameState = GameState.DEFAULT;
        cloudCnt = 0;
        rotateTimer = 0;
        bowButton.SetActive(false);
        crow = GameObject.Find("Crow").GetComponent<Crow>();
        isEncount = false;
        dwarf = GameObject.Find("Dwarf").GetComponent<Dwarf>();
        crowTimer = 0;

        isStart = false;

        var tmp = PlayerPrefs.GetInt("game_mode");
        GetComponent<CloudManager>().SetGameMode(tmp);

        se[0].mute = true;
        crowLoop.mute = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            if (gameState == GameState.DEFAULT)
            {
                AddGauge();
                CheckWeather();
            }

#if true
            crowTimer += Time.deltaTime;
            if (crowTimer > CROW_RETIME)
            {
                EncountCrow();
                crowTimer = UnityEngine.Random.Range(-CROW_RAND, CROW_RAND);
            }

#else

        // 仮でカラスを出す
        if(Input.GetKeyDown(KeyCode.Return))
        {
            EncountCrow();
        }
#endif
        }
        else
        {
            // チュートリアルパネルおん
            Time.timeScale = 0;
            tutoPanel.SetActive(true);
        }



    }

    /// <summary>
    /// ゲージの制御
    /// </summary>
    void AddGauge()
    {
        // カラスが来てないかつ、晴れじゃなかったらゲージは増えない
        if (weather != Weather.SUNNY || isEncount == true)
        {
            return;
        }
        // 晴れなのでゲージを増やす
        imageCampGauge.fillAmount += Time.deltaTime * gaugeSp;

        // ゲージの量によってキャンプが豪華になっていく
        MoveFase();

        // ゲージがマックス＝キャンプ完成なのでクリア
        if (imageCampGauge.fillAmount >= 1)
        {
            print("ゲームクリア！");
            gameState = GameState.GAME_CLEAR;
            // ゲームクリアをPrefsに保存
            EndGame(gameState);
        }
    }

    //フェーズごとに難易度設定可能
    /// <summary>
    /// ゲージ一定量毎にキャンプが完成していく
    /// </summary>
    public void MoveFase()
    {
        var tmp = Enum.GetNames(typeof(CampState)).Length - 1;
        if ((int)campFase < tmp)
        {
            switch (campFase)
            {
                //4段階目まではその段階に応じてオブジェクトを表示する
                case CampState.FASE1:
                    CampActive(0);
                    campObjs[0].GetComponent<SpriteRenderer>().sprite = picnik01;
                    break;
                case CampState.FASE2:
                    campObjs[0].GetComponent<SpriteRenderer>().sprite = picnik02;
                    break;
                case CampState.FASE3:
                    CampActive(1);
                    campObjs[1].GetComponent<SpriteRenderer>().sprite = tent01;
                    break;
                case CampState.FASE4:
                    campObjs[1].GetComponent<SpriteRenderer>().sprite = tent02;
                    break;
                case CampState.FASE5:
                    // todo:5段階目はお弁当を食べ始める
                    dwarf.Eat();
                    break;
            }
            var tmpCnt = imageCampGauge.fillAmount * 6 - 1;
            if (tmpCnt > (int)campFase)
            {
                campFase++;
                // とんてんかんとｎ
                se[5].Play();
            }
        }
    }

    /// <summary>
    /// 天気の変更、雲の生成された個数で判定
    /// </summary>
    void CheckWeather()
    {

#if false
        if (Input.GetKeyDown(KeyCode.C))
        {
            SetWeather(Weather.CLOUDY);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetWeather(Weather.RAINY);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetWeather(Weather.SUNNY);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            SetWeather(Weather.RAIN_STORM);
        }
#else
        if(cloudCnt > COUNT_RAIN_STORM)
        {
            SetWeather(Weather.RAIN_STORM);
        }
        else if(cloudCnt > COUNT_RAINY)
        {
            SetWeather(Weather.RAINY);
        }
        else if(cloudCnt > COUNT_CLOUDY)
        {
            SetWeather(Weather.CLOUDY);
        }
        else
        {
            SetWeather(Weather.SUNNY);
        }
#endif

        // 現在の天気に応じて天気図を回す
        WeatherRotation();


        switch (weather)
        {
            case Weather.SUNNY:
                rainyEffect.SetActive(false);
                imageBack.sprite = sourcesBackImage[(int)Weather.SUNNY];
                se[0].mute = true;
                break;
            case Weather.CLOUDY:
                rainyEffect.SetActive(false);
                imageBack.sprite = sourcesBackImage[(int)Weather.CLOUDY];
                se[0].mute = true;
                break;
            case Weather.RAINY:
                rainyEffect.SetActive(true);
                imageBack.sprite = sourcesBackImage[(int)Weather.RAINY];
                SetRainLevel1();
                se[0].mute = false;
                break;
            case Weather.RAIN_STORM:
                rainyEffect.SetActive(true);
                imageBack.sprite = sourcesBackImage[(int)Weather.RAIN_STORM];
                SetRainLevel2();
                se[0].mute = false;
                break;
        }


        // 雨ならエフェクトをオン
        if (weather == Weather.RAINY || weather == Weather.RAIN_STORM)
        {
            rainyEffect.SetActive(true);
        }
        else
        {
            rainyEffect.SetActive(false);
        }


    }

    // レベル1の雨の音声
    public void SetRainLevel1()
    {
        
        // レベル1の雨の音量とピッチを設定
        se[0].volume = 0.7f;
        se[0].pitch = 0.8f;
    }

    // レベル2の雨の音声
    public void SetRainLevel2()
    {
        // レベル2の雨の音量とピッチを設定
        se[0].volume = 1.0f;
        se[0].pitch = 1.1f;
    }


    /// <summary>
    /// 天気図の回転処理
    /// </summary>
    void WeatherRotation()
    {
        // 回転中かどうか判定
        if (isRotation)
        {
            // n秒で回転させるためにタイマーも管理
            rotateTimer += Time.deltaTime;
            // 現在座標から目標角度までどのくらい差があるか計算
            var tmpDiff = WEATHERS_RAD[(int)weather] - rotatableWeather.transform.rotation.eulerAngles.z;
            // 絶対値化
            tmpDiff = Math.Abs(tmpDiff);

            // 一定時間経過するor角度差が十分小さくなるまで回転
            if (rotateTimer < RAD_MOVE_SEC || tmpDiff > RAD_DIFF)
            {
                // Quaternionは終わってるのでEulerで計算
                var targetQua = rotatableWeather.transform.rotation;
                var targetRad = targetQua.eulerAngles;
                print("目標角度:" + targetRad);
                // Euler角で代入
                targetRad.z = WEATHERS_RAD[(int)weather];
                // Quaternionに戻す
                targetQua.eulerAngles = targetRad;

                // なめらかに回転させ続ける
                rotatableWeather.transform.rotation = Quaternion.RotateTowards(rotatableWeather.transform.rotation, targetQua, radDiffStartEnd / RAD_MOVE_SEC * Time.deltaTime);
            }
            // 回転の終了
            else
            {
                // Euler角で計算
                var targetQua = rotatableWeather.transform.rotation;
                var targetRad = targetQua.eulerAngles;
                // 直代入して回転を終了させる
                targetRad.z = WEATHERS_RAD[(int)weather];
                targetQua.eulerAngles = targetRad;
                rotatableWeather.transform.rotation = targetQua;
                // 回転フラグ
                isRotation = false;
                rotateTimer = 0;
            }
        }
        // 回転中でないなら目標角度を計算する
        else
        {
            // 目標角度までのずれを算出
            var difRadStartToTarget = WEATHERS_RAD[(int)weather] - rotatableWeather.transform.rotation.eulerAngles.z;
            // 絶対値化
            difRadStartToTarget = Math.Abs(difRadStartToTarget);
            // 目標角度までのずれが一定より大きい場合
            if (difRadStartToTarget > RAD_DIFF)
            {
                // 初期位置からの必要な移動量を保存
                radDiffStartEnd = difRadStartToTarget;
                isRotation = true;
                se[2].Play();
            }
        }
    }

    /// <summary>
    /// キャンプが徐々に出来上がる
    /// </summary>
    void CampActive(int _index)
    {
        var obj = campObjs[_index];
        obj.SetActive(true);
        if(obj.transform.localScale.y < 0.100f)
        {
            var tmp = obj.transform.localScale;
            tmp.y += Time.deltaTime;
            obj.transform.localScale = tmp;
        }
        else
        {
            var tmp = obj.transform.localScale;
            tmp.y = 0.100f;
            obj.transform.localScale = tmp;
        }
    }

    /// <summary>
    /// ゲーム情報を"result"でPrefsに保存、1:成功,0:失敗
    /// </summary>
    void EndGame(GameState _state)
    {
        switch (_state)
        {
            case GameState.GAME_CLEAR:
                PlayerPrefs.SetInt("result", 1);
                break;
            case GameState.GAME_OVER:
                PlayerPrefs.SetInt("result", 0);
                break;
            default:
                print("予期せぬ終了命令");
                return;
        }
        PlayerPrefs.Save();
        Invoke("DelayScene", 1f);
    }

    void DelayScene()
    {
        SceneManager.LoadScene(resultSceneName);
    }

    public CampState GetCampState()
    {
        return campFase;
    }



    public Weather GetWeather()
    {
        return weather;
    }

    public void SetWeather(Weather _weather)
    {
        weather = _weather;
    }

    public void GameOver()
    {
        gameState = GameState.GAME_OVER;
        EndGame(gameState);
    }

    /// <summary>
    /// カラス出現時の処理
    /// </summary>
    public void EncountCrow()
    {
        isEncount = true;
        bowButton.SetActive(true);
        // カラスをアクティブにする
        crow.Encount();
        // 小人を恐怖状態にする
        dwarf.EncountCrow();
        crowLoop.mute = false;
    }

    /// <summary>
    /// ボタンを押したら鳥に攻撃
    /// </summary>
    public void PushBowButton()
    {
        // 鳥に攻撃
        print("攻撃！");
        crow.Damage();
        se[4].Play();
    }

    /// <summary>
    /// カラスを倒したときの処理
    /// </summary>
    public void OverCrow()
    {
        bowButton.SetActive(false);
        isEncount = false;
        // 小人の恐怖状態を解除
        dwarf.OverCrow();
        crowLoop.mute = true;
        se[3].Play();
    }

    public void CloudGen()
    {
        cloudCnt += 1;
    }

    public void CloudDes()
    {
        cloudCnt -= 1;
    }

    public void RainCloudGen()
    {
        cloudCnt += 2;
    }

    public void RainCloudDes()
    {
        cloudCnt -= 2;
    }

    public void CloudClick()
    {
        se[6].Play();
    }

    public void RainCloudClick()
    {
        se[7].Play();
    }


    public void BackPage()
    {
        tutoPanel.transform.position -= new Vector3(tmp, 0, 0);
        se[1].Play();
    }


    public void NextPage()
    {
        tutoPanel.transform.position += new Vector3(tmp, 0, 0);
        se[1].Play();
    }

    public void EndTuto()
    {
        Time.timeScale = 1;
        isStart = true;
        tutoPanel.SetActive(false);
        se[1].Play();

    }
}
