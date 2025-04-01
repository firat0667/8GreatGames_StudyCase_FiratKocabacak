using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "PassengerMoveSettings", menuName = "Game/Passenger Move Settings")]
public class PassengerMoveSettingsSO : ScriptableObject
{
    public float jumpPower = 0.5f;
    public float jumpDuration = 0.4f;
    public int jumpCount = 1;

    public float scaleUp = 1.5f;
    public float scaleDuration = 0.1f;

    public Ease jumpEase = Ease.OutQuad;
    public Ease scaleEase = Ease.OutBack;

    public float moveDelayBetweenPassengers = 0.2f;
}
