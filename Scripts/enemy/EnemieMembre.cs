using UnityEngine;
using System.Collections.Generic;
using VHS;

public class EnemieMembre : MonoBehaviour
{
    [SerializeField] private NavEnemie ne = null;
    [SerializeField] private Boss b = null;
    [SerializeField] [Range(0, 200)] private float rangeDegat = 100f;

    public void ReceiveDamageMembre(float damage,GameObject player)
    {
        if(b)
        {
            b.ReceiveDamage(damage * (rangeDegat / 100));
            return;
        }
        ne.ReceiveDamage(damage * (rangeDegat / 100),player);
    }
}
