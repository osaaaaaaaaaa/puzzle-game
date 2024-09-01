using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class StoreUserRequest
{
    [JsonProperty("name")]
    public string Name { get; set; }
}
