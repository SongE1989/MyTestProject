using UnityEngine;
using System.Collections;

public class TradeWorld : MonoBehaviour {
    public static TradeWorld Instance;
    public Town180415[] towns;
    public Trader180415[] traders;
    // Use this for initialization
    void Start () {
        Instance = this;
        towns = GetComponentsInChildren<Town180415>();
        for (int i_town = 0, n_town = towns.Length; i_town < n_town; i_town++)
        {
            var town = towns[i_town];
            town.gameObject.SetActive(true);
        }

        traders = GetComponentsInChildren<Trader180415>();
        for (int i_trader = 0, n_trader = traders.Length; i_trader < n_trader; i_trader++)
        {
            var trader = traders[i_trader];
            trader.gameObject.SetActive(true);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
