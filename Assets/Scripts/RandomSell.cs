using System.Collections;
using UnityEngine;

public class RandomSell : MonoBehaviour
{
    //these will be set by "PlaceObject" script when object spawns
    public float avgSellTimeSecs, varianceSellTimeSecs;
    
    //this will be set by the stock item or stock list eventually
    public int moneyOnSell;

    private IEnumerator Start()
    {
        //wait random time
        float variance = Random.Range(-varianceSellTimeSecs, varianceSellTimeSecs);
        float waitTimeTotal = avgSellTimeSecs + variance;
        yield return new WaitForSeconds(waitTimeTotal);

        //destroy object and give player money
        GameManager.Instance.Money += moneyOnSell;
        Destroy(gameObject);
    }
}