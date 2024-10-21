using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class DistressSignalGuestRequest
{
    [JsonProperty("d_signal_id")]
    public int SignalID { get; set; }
    [JsonProperty("destroy_user_id")]
    public int DestroyUserID { get; set; }
}
