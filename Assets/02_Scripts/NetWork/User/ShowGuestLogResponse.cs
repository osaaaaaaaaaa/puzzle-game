using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class ShowGuestLogResponse
{
    [JsonProperty("d_signal_id")]
    public int SignalID { get; set; }
    [JsonProperty("host_id")]
    public int HostID { get; set; }
    [JsonProperty("host_name")]
    public string HostName { get; set; }
    [JsonProperty("stage_id")]
    public int StageID { get; set; }
    [JsonProperty("action")]
    public bool IsStageClear { get; set; }
    [JsonProperty("is_rewarded")]
    public bool IsRewarded { get; set; }
    [JsonProperty("cnt_guest")]
    public int GuestCnt { get; set; }
    [JsonProperty("created_at")]
    public DateTime CreateDay { get; set; }
}
