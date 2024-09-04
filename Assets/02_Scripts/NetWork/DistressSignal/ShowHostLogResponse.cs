using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class ShowHostLogResponse
{
    [JsonProperty("d_signal_id")]
    public int SignalID { get; set; }
    [JsonProperty("stage_id")]
    public int StageID { get; set; }
    [JsonProperty("action")]
    public bool IsStageClear { get; set; }
    [JsonProperty("cnt_guest")]
    public int GuestCnt { get; set; }
    [JsonProperty("created_at")]
    public DateTime CreateDay { get; set; }
}
