using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "BusMoverSettings", menuName = "Game/BusMoverSettings")]
public class BusMoverSettingsSO : ScriptableObject
{
    public float MoveDuration = 0.25f;
    public Ease MoveEase = Ease.InOutSine;
}
