using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class ShowRndSignalResponse
{
    [JsonProperty("d_signal_id")]
    public int SignalID { get; set; }
    [JsonProperty("user_id")]
    public int HostID { get; set; }
    [JsonProperty("host_name")]
    public string HostName { get; set; }
    [JsonProperty("stage_id")]
    public int StageID { get; set; }
    [JsonProperty("cnt_guest")]
    public int GuestCnt { get; set; }
    [JsonProperty("elapsed_days")]
    public int ElapsedDays { get; set; }
}
