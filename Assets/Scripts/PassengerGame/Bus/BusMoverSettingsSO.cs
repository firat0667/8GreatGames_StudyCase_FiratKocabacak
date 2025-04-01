using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "BusMoverSettings", menuName = "Game/BusMoverSettings")]
public class BusMoverSettingsSO : ScriptableObject
{
    public float MoveDuration = 0.25f;
    public Ease MoveEase = Ease.InOutSine;
    public float RotationSpeed = 0.1f;
    public Ease RotationEase = Ease.InOutSine;

    public float ShakePower = 0.25f;

    public float DestroyJumpPower = 0.6f;
    public float DestroyJumpDuration = 0.4f;
    public Ease DestroyEase = Ease.InBack;
    public float DestroyScale = 0.4f;
    public Ease DestroyScaleEase = Ease.InOutSine;
    public float DelayBetweenSegmentDestruction = 0.2f;

}
