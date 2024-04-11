using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dwarf : MonoBehaviour
{
    const float DWARF_HP = 10;

    const float DAMAGE_RAINY = 1.3f;
    const float DAMAGE_STORM = 2f;

    [SerializeField, Range(1f, 5f)]
    float speed;

    const int a = (int)GameManager.Weather.RAIN_STORM;

    readonly float[] SPEEDS = new float[] {1f, 1.2f };

    const float DAMAGE_BASE = 0.05f;

    float[] movePosX = new float[2] {4f, -1f};

    [SerializeField]
    GameManager gm;

    // 反転処理用
    [SerializeField]
    GameObject dwarfSpriteObj;

    Animator animator;

    bool isCrow;
    const float DAMAGE_CROW = 1.7f;

    bool isEating;

    public enum MoveState
    {
        RIGHT,
        LEFT
    }
    [SerializeField]
    MoveState moveState;

    // コンポーネント
    SpriteRenderer sr;

    // 小人の差分
    Sprite[] sprites;

    [SerializeField]
    Image hpGauge;


    // Start is called before the first frame update
    void Start()
    {
        isCrow = false;
        moveState = MoveState.RIGHT;
        sr = GetComponent<SpriteRenderer>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        animator = GetComponent<Animator>();
        isEating = false;
    }

    // Update is called once per frame
    void Update()
    {
        // キャンプの天気を取得
        var tmpWeather = gm.GetWeather();
        if (!isEating)
        {

            
            // カラスが来ていないかつ、晴れの時かつ、ゲームが終了していないなら移動
            if ((tmpWeather == GameManager.Weather.SUNNY || tmpWeather == GameManager.Weather.CLOUDY) && gm.gameState == GameManager.GameState.DEFAULT && isCrow == false)
            {
                Move();
                CheckPosition();
            }
        }
        else
        {
            transform.position = new Vector3(-1.39f, -0.93f, 0);
        }

        // ダメージ処理
        CheckWeatherDamage(tmpWeather);
        // アニメーション
        Animation(tmpWeather);
    }


    void CrowDamage()
    {
        hpGauge.fillAmount -= Time.deltaTime * DAMAGE_CROW * DAMAGE_BASE;

        if (hpGauge.fillAmount <= 0f)
        {
            gm.GameOver();
        }
    }

    /// <summary>
    /// ダメージ処理、
    /// </summary>
    void Damage(GameManager.Weather _w)
    {
        var damageBonus = 0f;
        if(_w == GameManager.Weather.RAINY)
        {
            damageBonus = DAMAGE_RAINY;
        }
        else if(_w == GameManager.Weather.RAIN_STORM)
        {
            damageBonus = DAMAGE_STORM;
        }

        hpGauge.fillAmount -= Time.deltaTime * damageBonus * DAMAGE_BASE;

        if(hpGauge.fillAmount <= 0f)
        {
            gm.GameOver();
        }
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        var tmpVec = new Vector3();
        switch(moveState)
        {
            case MoveState.RIGHT:
                tmpVec = new Vector3(1, 0, 0);
                break;

            case MoveState.LEFT:
                tmpVec = new Vector3(-1, 0, 0);
                break;
        }

        // 移動量は、ベクトル×スピード×時間
        // これで調整しやすくなる
        if(gm.GetWeather() == GameManager.Weather.SUNNY)
        {
            speed = SPEEDS[0];
        }
        else
        {
            speed = SPEEDS[1];
        }
        transform.position += tmpVec * speed * Time.deltaTime;
    }


    /// <summary>
    /// 現在の座標を判定して状態を更新
    /// </summary>
    void CheckPosition()
    {
        var tmpX = transform.position.x;

        if(tmpX > movePosX[(int)MoveState.RIGHT] && moveState == MoveState.RIGHT)
        {
            moveState = MoveState.LEFT;
            FlipScale();
        }
        else if(tmpX < movePosX[(int)MoveState.LEFT] && moveState == MoveState.LEFT)
        {
            // 画面外に出たので画像切り替え
            moveState = MoveState.RIGHT;
            FlipScale();
        }
    }


    /// <summary>
    /// 現在の天気を参照してダメージ処理
    /// </summary>
    void CheckWeatherDamage(GameManager.Weather tmpWeather)
    {
        if(isCrow)
        {
            CrowDamage();
            return;
        }


        // ゲーム中の状態が雨か暴風雨ならダメージ
        if (tmpWeather == GameManager.Weather.RAIN_STORM)
        {
            Damage(GameManager.Weather.RAIN_STORM);
        }
        else if (tmpWeather == GameManager.Weather.RAINY)
        {
            Damage(GameManager.Weather.RAINY);
        }
    }


    /// <summary>
    /// 小人のSpriteのlocalScaleを反転させる
    /// </summary>
    void FlipScale()
    {
        var tmp = dwarfSpriteObj.transform.localScale;
        print("lscale:" + tmp);
        tmp.x *= -1;
        print("計算後lscale:" + tmp);
        dwarfSpriteObj.transform.localScale = tmp;
        print("代入後lscale:" + dwarfSpriteObj.transform.localScale);

    }


    /// <summary>
    /// アニメーション処理
    /// </summary>
    void Animation(GameManager.Weather _w)
    {
        // 恐怖状態なら恐怖if()
        // そうでないなら天気のアニメーション
        var trigger = "";
        if (isCrow)
        {
            trigger = "Crow";
        }
        else
        {
            switch (_w)
            {
                case GameManager.Weather.SUNNY:
                    if(isEating)
                    {
                        trigger = "Eat";
                    }
                    else
                    {
                        trigger = "Sunny";
                    }
                    break;
                case GameManager.Weather.CLOUDY:
                    trigger = "Cloudy";
                    break;
                case GameManager.Weather.RAINY:
                    trigger = "Rainy";
                    break;
            }
        }

        animator.SetTrigger(trigger);
    }


    public void EncountCrow()
    {
        isCrow = true;
    }

    public void OverCrow()
    {
        isCrow = false;
    }


    public void Eat()
    {
        isEating = true;
    }
}
