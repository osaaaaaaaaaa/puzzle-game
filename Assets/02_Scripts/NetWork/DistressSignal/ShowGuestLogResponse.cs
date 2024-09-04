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
    [JsonProperty("icon_id")]
    public int IconID { get; set; }
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
    [JsonProperty("is_agreement")]
    public bool IsAgreement { get; set; }
    [JsonProperty("elapsed_days")]
    public int ElapsedDay { get; set; }
}