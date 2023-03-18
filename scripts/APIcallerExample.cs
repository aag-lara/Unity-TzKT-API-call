using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using UnityEditor.PackageManager;

//
//
//Unity Tezos API caller example by AaG
//Uses JsonHelper by Programmer and dbc (stackoverflow https://stackoverflow.com/users/3744182/dbc https://stackoverflow.com/users/3785314/programmer) 
//So its free use however you like good luck on your projects <3
//It limits the search to 100 tokens, if you need more you can change the URL2 string down below.
//
//Powered by TzKT API - https://tzkt.io/
//SHOW THEM SOME LOVE!!!!!
//And read their best practices + documentation for more fine tuning
//
//
//

public class APIcallerExample : MonoBehaviour
{
    string URL0 = "https://staging.api.tzkt.io/v1/tokens/balances?account=";
    string URL1 = "&balance.ne=0&token.contract=";
    string URL2 = "&select=token.tokenId as tokenId,balance as Balance&limit=100&sort.asc=token.tokenId";
    string fullURL;
    string fullJson;
    bool gotContent = false;

    string TEIAcontract = "KT1RJ6PbjHpwc3M5rw5s2Nbmefwbuwbdxton";
    public string UserWallet;

    public static APIcallerExample instance;

    public Token[] TokenList = null;

    public TMP_Text show;
    public TMP_InputField InputField;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //Gets input wallet
    public void clickTest()
    {
        if (InputField.text == "")
        {
            Debug.Log("NO INPUT");
            show.text = "error";
        }
        else
        {
            show.text = "";
            TokenList = null;
            UserWallet = InputField.text;
            APIcall(UserWallet, TEIAcontract);
        }
        

    }
    //Fetch TzKT data and wait for it before parsing
    public void APIcall(string walletAddress, string contractAddres)
    {
        StartCoroutine(GetData(walletAddress, contractAddres));
        StartCoroutine(ParseData());

    }
    IEnumerator GetData(string walletAddress, string contractAddres)
    {
        gotContent = false;
        fullURL = URL0 + walletAddress + URL1 + contractAddres + URL2;
        using (UnityWebRequest request = UnityWebRequest.Get(fullURL))
        {
            yield return request.SendWebRequest();

            if(request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("API REQUEST ERROR");
            } else {
                string json = request.downloadHandler.text;
                //if(show != null) { show.text = json; }
                fullJson= json;
                gotContent = true;
            }
        }
    }
    //Organizes the data into an array of Token that unity can easily read. Call for your game's inventory function when tokens 
    IEnumerator ParseData()
    {
        while (!gotContent)
        {
            yield return null;
        }
        //Most of the magic is this, delve into JsonHelper for more 
        TokenList = JsonHelper.FromJson<Token>(fixJson(fullJson));
        show.text = "";
        for(int i = 0; i < TokenList.Length; i++)
        {
            Debug.Log(TokenList[i].tokenId + " - " + TokenList[i].Balance);
            show.text += TokenList[i].tokenId + " - " + TokenList[i].Balance + "\n";
        }

        Debug.Log(TokenList.Length + " tokens found");
        
    }
    //Adding some needed json syntax to make them read as an array
    string fixJson(string value)
    {
        value = "{\"Items\":" + value + "}";
        return value;
    }

}
