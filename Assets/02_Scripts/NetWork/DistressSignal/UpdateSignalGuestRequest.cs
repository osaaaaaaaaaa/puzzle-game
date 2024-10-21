using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class UpdateSignalGuestRequest
{
    [JsonProperty("d_signal_id")]
    public int SignalID { get; set; }
    [JsonProperty("position")]
    public string Pos { get; set; }
    [JsonProperty("vector")]
    public string Vector { get; set; }
}
