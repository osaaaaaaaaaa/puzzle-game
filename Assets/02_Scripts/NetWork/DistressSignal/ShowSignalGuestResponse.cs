using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ShowSignalGuestResponse
{
    [JsonProperty("user_id")]
    public int UserID { get; set; }
    [JsonProperty("name")]
    public string UserName { get; set; }
    [JsonProperty("position")]
    public string Pos { get; set; }
    [JsonProperty("vector")]
    public string Vector { get; set; }
}
