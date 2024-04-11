using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kaiten : MonoBehaviour
{
    [SerializeField]
    [Tooltip("最小角度(-180～180")]
    private float minAngle;

    [SerializeField]
    [Tooltip("最大角度(-180～180")]
    private float maxAngle;

    [SerializeField]
    [Tooltip("回転するスピード")]
    private float rotationSpeed = 1;

    // Update is called once per frame
    void Update()
    {
        // 左右キーの入力を取得
        float horizontal = Input.GetAxis("Horizontal");
        // 現在のGameObjectのZ軸方向の角度を取得
        float currentZAngle = transform.eulerAngles.y;
        // 現在の角度が180より大きい場合
        if (currentZAngle > 180)
        {
            // デフォルトでは角度は0～360なので-180～180となるように補正
            currentZAngle = currentZAngle - 360;
        }
        // (現在の角度が最小角度以上かつキー入力が0未満(左キー押下)) または (現在の角度が最大角度以下かつキー入力が0より大きい(右キー押下))の時
        if ((currentZAngle >= minAngle && horizontal < 0) || (currentZAngle <= maxAngle && horizontal > 0))
        {
            // Z軸を基準に回転させる
            transform.Rotate(new Vector3(0, 0, horizontal * rotationSpeed));
        }
    }
}