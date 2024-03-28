using DG.Tweening;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExample : MonoBehaviour
{
    private void Start()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOMove(new Vector2(2,0),4)).
        Append(transform.DOMove(new Vector2(-2,0),4));
    }
}
