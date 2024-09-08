using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class DestroySignalGuestRequest
{
    [JsonProperty("d_signal_id")]
    public int SignalID { get; set; }
    [JsonProperty("user_id")]
    public int UserID { get; set; }
}
