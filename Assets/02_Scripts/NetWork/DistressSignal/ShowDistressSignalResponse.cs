using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ShowDistressSignalResponse
{
    [JsonProperty("d_signal_id")]
    public int SignalID { get; set; }
    [JsonProperty("stage_id")]
    public int StageID { get; set; }
}
