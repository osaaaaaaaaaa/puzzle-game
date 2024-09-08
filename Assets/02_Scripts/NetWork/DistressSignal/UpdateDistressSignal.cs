using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class UpdateDistressSignal
{
    [JsonProperty("d_signal_id")]
    public int SignalID { get; set; }
}
