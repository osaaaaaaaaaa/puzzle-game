using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class UpdateStageClearRequest
{
    [JsonProperty("stage_id")]
    public int StageID { get; set; }
    [JsonProperty("is_medal1")]
    public bool IsMedal1 { get; set; }
    [JsonProperty("is_medal2")]
    public bool IsMedal2 { get; set; }
    [JsonProperty("time")]
    public float Time { get; set; }
    [JsonProperty("score")]
    public int Score { get; set; }
}
