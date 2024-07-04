using System.Collections;
using UnityEngine;

public class RandomSell : MonoBehaviour
{
    [SerializeField] private float avgSellTimeSecs, varienceSellTimeSecs;
    [SerializeField] private int moneyOnSell;

    private IEnumerator Start()
    {
        //wait random time
        float varience = Random.Range(-varienceSellTimeSecs, varienceSellTimeSecs);
        float waitTimeTotal = avgSellTimeSecs + varience;
        yield return new WaitForSeconds(waitTimeTotal);

        //destroy object and give player money
        GameManager.Instance.Money += moneyOnSell;
        Destroy(gameObject);
    }
}