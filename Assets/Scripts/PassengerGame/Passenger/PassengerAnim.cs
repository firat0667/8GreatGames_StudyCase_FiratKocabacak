using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerAnim : MonoBehaviour
{
    [SerializeField] private Animator _passengerAnim;

    [SerializeField] private  string _passengerSitAnimName;

    public void PlayPassengerSit()
    {
        _passengerAnim.Play(_passengerSitAnimName);
    }
}
