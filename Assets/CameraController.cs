using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // 追従する対象（Playerオブジェクト）
    public float smoothSpeed = 0.125f; // カメラの追従速度
    public Vector3 offset; // カメラと対象との位置関係（オフセット）

    void LateUpdate()
    {
        if (target == null) return; // ターゲットが設定されていない場合は処理をしない

        // ターゲットの目標位置を計算（オフセットを考慮）
        Vector3 desiredPosition = target.position + offset;

        // 現在の位置から目標位置へ滑らかに移動
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // カメラの位置を更新
        transform.position = smoothedPosition;

        // カメラが常にターゲットを正面から見るようにする
        transform.LookAt(target);
    }
}
