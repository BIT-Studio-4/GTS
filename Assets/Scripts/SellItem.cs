using UnityEngine;

public class SellItem : MonoBehaviour
{
    //this will be set by the stock item or stock list eventually
    public int moneyOnSell;

    private void Start()
    {
        StockManager.Instance.itemPlaced.Invoke(this);

        ////wait random time
        //float variance = Random.Range(-varianceSellTimeSecs, varianceSellTimeSecs);
        //float waitTimeTotal = avgSellTimeSecs + variance;
        //yield return new WaitForSeconds(waitTimeTotal);
        //// spawn customer and give this item to it
        //CustomerManager.Instance.SpawnCustomer(this);
    }
}