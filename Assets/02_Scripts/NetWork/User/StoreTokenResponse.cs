using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class StoreTokenResponse
{
    [JsonProperty("token")]
    public string Token { get; set; }
}
