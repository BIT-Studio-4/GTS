using System.Collections;
using UnityEngine;

public class SellItem : MonoBehaviour
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
        // spawn customer and give this item to it
        CustomerManager.Instance.SpawnCustomer(this);
    }
}