using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class StoreTokenRequest
{
    [JsonProperty("user_id")]
    public int UserID { get; set; }
}
