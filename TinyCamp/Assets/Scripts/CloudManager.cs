using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    // 変数定義(SerializeField)
    [SerializeField] GameObject canvas;             // 雲用のキャンバス
    [SerializeField] GameObject cloud;              // 雲
    [SerializeField] GameObject rainCloud;          // 雨雲
    [SerializeField, Range(1f, 100f)] float speed;   // 雲の生成間隔

    // 変数定義(プライベート)
    private float spawnPosX, spawnPosY; // 雲の生成場所を入れる変数（x,y）
    private float spawnCount;           // 雲を生成するための時間をカウントする変数
    private float hScaleX, hScaleY;     // 雲の幅の半分の値を入れる変数

    // 変数定義(Vector)
    Vector3 screenLeft;  // 画面左下の世界座標
    Vector3 screenRight; // 画面右上の世界座標

    // 難易度用
    [SerializeField, Range(0, 1)] int def;
    float spawnTime;    // カウントの上限を定義
    const float NORMAL = 15;
    const float HARD = 9.7f;

    GameManager gm;

    // 雲を生成する関数
    void GenerateCloud()
    {
        // 雲を生成する座標をx,yともに画面上の方に生成されるようランダム定義
        spawnPosX = Random.Range(screenLeft.x + hScaleX, screenRight.x - hScaleX);
        spawnPosY = Random.Range(1.0f, screenRight.y - hScaleY);

        // 乱数を用いて雲か雨雲かを分ける
        int rnd = Random.Range(0, 10);
        // 雲の生成

        // ハード
        if (def == 1)
        {
            // 乱数が4以上なら雨雲
            if (rnd > 3)
            {
                var generate = Instantiate(rainCloud, canvas.transform);
                generate.transform.position = new Vector3(spawnPosX, spawnPosY, 0);
                gm.RainCloudGen();
            }
            else
            {
                var generate = Instantiate(cloud, canvas.transform);
                generate.transform.position = new Vector3(spawnPosX, spawnPosY, 0);
                gm.CloudGen();
            }
        }
        // ノーマル
        else if (def == 0)
        {
            // 乱数が7以上なら雨雲
            if (rnd > 7)
            {
                var generate = Instantiate(rainCloud, canvas.transform);
                generate.transform.position = new Vector3(spawnPosX, spawnPosY, 0);
                gm.RainCloudGen();
            }
            else
            {
                var generate = Instantiate(cloud, canvas.transform);
                generate.transform.position = new Vector3(spawnPosX, spawnPosY, 0);
                gm.CloudGen();
            }
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

        // 変数の初期化
        spawnPosX = 0;
        spawnPosY = 0;
        spawnCount = 0;
        hScaleX = transform.localScale.x / 3;
        hScaleY = transform.localScale.y / 3;
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();


        // ハード
        if (def == 1)
        {
            spawnTime = HARD;
        }
        // ノーマル
        else if(def == 0) 
        {
            spawnTime = NORMAL;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 毎フレームごとにカウントを増やす
        // 乱数をかけて生成間隔を同じではないようにする
        spawnCount += Time.deltaTime * Random.Range(0, 2) * speed;
        
        // カウントが上限に達したら雲を生成しカウンターを０に戻す
        if (spawnCount >= spawnTime && gm.GetCampState() != GameManager.CampState.FASE5)
        {
            GenerateCloud();
            spawnCount = 0;
        }
    }


    public void SetGameMode(int _mode)
    {
        def = _mode;
    }
}
