using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ShowUserItemResponse
{
    [JsonProperty("item_id")]
    public int ItemID { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("effect")]
    public int Effect { get; set; }
    [JsonProperty("amount")]
    public int Amount { get; set; }
}
