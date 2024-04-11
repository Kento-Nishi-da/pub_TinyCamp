using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crow : MonoBehaviour
{
    [SerializeField]
    Vector3 stPos;
    [SerializeField]
    Vector3 edPos;
    Vector3 moveVec;

    float moveVecLen;

    int hp;
    const int HP_MAX = 10;
    bool isEncout;
    bool isMove;

    GameManager gm;
    Animator animator;
    SpriteRenderer sr;

    float moveSec = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        transform.position = stPos;
        moveVec = edPos - stPos;
        moveVecLen = moveVec.magnitude;
        hp = HP_MAX;
        isEncout = false;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Animation();
    }

    /// <summary>
    /// 画面に入ってくる移動処理
    /// </summary>
    private void Move()
    {
        // 目標座標を設定
        Vector3 target;
        if (isEncout)
        {
            target = edPos;
        }
        else
        {
            target = stPos;
        }

        // 目標座標までの距離を計算
        moveVecLen = (transform.position - target).magnitude;
        // 距離が一定以上なら移動
        if (moveVecLen > 0.1f)
        {
            // 出現時の処理
            if (isEncout)
            {
                transform.position += moveVec / moveSec * Time.deltaTime;
            }
            // 退場時の処理
            else
            {
                transform.position -= moveVec / moveSec * Time.deltaTime;
            }
        }


        //var tmp = Color.white - Color.red;
        //sr.color += tmp * Time.deltaTime;
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    public void Damage()
    {
        animator.SetTrigger("Hit");
        hp--;
        //sr.color = Color.red;
        if (hp <= 0)
        {
            isEncout = false;
            gm.OverCrow();
        }
    }

    /// <summary>
    /// アニメーション処理
    /// </summary>
    void Animation()
    {
        // hp==0ならダメージ、それ以外なら通常
        string trigger;
        if (hp == 0)
        {
            trigger = "Damage";
        }
        else
        {
            trigger = "Idle";
        }

        animator.SetTrigger(trigger);
    }

    /// <summary>
    /// 現在のHP取得
    /// </summary>
    /// <returns></returns>
    public int GetHp()
    {
        return hp;
    }

    /// <summary>
    /// カラス出現時の処理
    /// </summary>
    public void Encount()
    {
        isEncout = true;
        hp = HP_MAX;
    }
}
