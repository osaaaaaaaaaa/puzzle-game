using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class StoreUserResponse
{
    [JsonProperty("user_id")]
    public int UserID { get; set; }
}
