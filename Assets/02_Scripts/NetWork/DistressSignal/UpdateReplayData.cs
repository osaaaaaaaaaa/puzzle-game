using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class UpdateReplayData
{
    [JsonProperty("d_signal_id")]
    public int SignalID { get; set; }
    [JsonProperty("replay_data")]
    public List<ReplayData> ReplayDatas { get; set; }
}
