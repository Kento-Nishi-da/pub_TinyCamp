﻿using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public class Cloud : MonoBehaviour
{
    // 変数定義(SerializeField)
    [SerializeField] GameObject sticker;   // シールの画像

    // 変数定義（Vector）
    Vector3 screenLeft;  // 画面左下の世界座標
    Vector3 screenRight; // 画面右上の世界座標

    // 変数定義（フラグ）
    int eFlg;   // 向きのフラグ（０が右、１が左）※乱数をとるためint型
    bool gFlg;  // 生成しきったかどうかのフラグ
    bool bFlg;  // 消え始めてよいかのフラグ

    // その他の変数定義
    Color colorC;   // 雲の色の変数
    Color colorS;   // シールの色の変数
    float hScaleX;  // 雲の横幅の半分の値を入れる変数
    float timeCnt;  // タイマーカウントする変数
    float moveSpeed;// 雲の移動スピードの変数

    // 定数定義
    const float TIME_MAX = 0.5f;    // カウントの上限を定義

    GameManager gm;

    // 雲の生成をする関数
    void Generate()
    {
        // expは生成速度を入れる変数
        float exp = 5f;
        // ｘ、ｙともに同じ速度で拡大させる
        this.transform.localScale += new Vector3(exp, exp, 0) * Time.deltaTime;
    }

    // 雲の動きの関数
    void Move()
    {
        // 向きフラグ（０が右、１が左）によって移動する向きを変える
        if (eFlg == 1)
        {
            // 左
            this.transform.position -= new Vector3(moveSpeed, 0, 0) * Time.deltaTime;
        }
        else
        {
            // 右
            this.transform.position += new Vector3(moveSpeed, 0, 0) * Time.deltaTime;
        }

        // 画面の端についたら向きフラグを変える
        if ((transform.position.x + hScaleX) >= screenRight.x)
        {
            // 右端
            eFlg = 1;
        }

        if ((transform.position.x - hScaleX) <= screenLeft.x)
        {
            // 左端
            eFlg = 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // 画面の左下の座標を取得
        screenLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
        // 画面の右上の座標を取得
        screenRight = Camera.main.ScreenToWorldPoint(
            new Vector3(Screen.width, Screen.height, 0));

        // 各フラグの初期値を取得
        eFlg = Random.Range(0, 2);
        gFlg = false;
        bFlg = false;

        // 変数の初期化
        timeCnt = 0;
        hScaleX = 0;
        moveSpeed = 2f + Random.Range(-1f, 1f);
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        // 色を取得
        colorC = gameObject.GetComponentInChildren<Button>().image.color;
        colorS = sticker.GetComponent<Image>().color;
    }

    // Update is called once per frame
    void Update()
    {
        // 大きくなりきっていないときは大きくさせる
        if (transform.localScale.x < 3 && transform.localScale.y < 3)
        {
            Generate();
        }
        // 大きくなりきっていたらそれ以外の処理をさせる
        else
        {
            // 生成しきったかどうかのフラグをtrueにする
            gFlg = true;
            // 雲の横幅の半分の値を取得
            hScaleX = transform.localScale.x / 3;

            // 消え始めてよいかのフラグがtrueの時はAlpha値を徐々に減らしていく
            if (bFlg == true)
            {
                // タイマーカウントする
                timeCnt += Time.deltaTime;

                // 上限を超えたら消え始める
                if (timeCnt >= TIME_MAX)
                {
                    float dTime = 0.5f;   // 1フレームごとに減らす量

                    colorC.a -= dTime * Time.deltaTime;
                    gameObject.GetComponentInChildren<Button>().image.color = colorC;
                    if (timeCnt >= TIME_MAX + 0.02)
                    {
                        colorS.a -= dTime * Time.deltaTime;
                        sticker.GetComponent<Image>().color = colorS;
                    }

                    // 完全に消えきったらオブジェクトを消す
                    if (colorS.a <= 0)
                    {
                        gm.CloudDes();
                        Destroy(gameObject);
                    }
                }
            }
            else
            {
                // 動作処理を行う
                Move();
            }
        }
    }

    // 雲が押されたときの処理
    public void PushCloud()
    {
        // 生成しきってないときは何もしない
        if (gFlg == true)
        {
            // ボタンが押されたかどうかのフラグをtrueにする
            sticker.SetActive(true);

            // 消え始めてよいかのフラグをtrueにする
            bFlg = true;
            GetComponentInChildren<Button>().enabled = false;
            GetComponentInChildren<Image>().raycastTarget = false;

            gm.CloudClick();
        }
    }
}
