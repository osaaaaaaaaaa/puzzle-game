using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ShowRankingResponse
{
    [JsonProperty("user_id")]
    public int UserID { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("title")]
    public string AchievementTitle { get; set; }
    [JsonProperty("icon_id")]
    public int IconID { get; set; }
    [JsonProperty("stage_id")]
    public int StageID { get; set; }
    [JsonProperty("score")]
    public int TotalScore { get; set; }
    [JsonProperty("is_agreement")]
    public bool IsAgreement { get; set; }
}
