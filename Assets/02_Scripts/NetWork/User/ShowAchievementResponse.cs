using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ShowAchievementResponse
{
    [JsonProperty("achievement_id")]
    public int AchievementID { get; set; }
    [JsonProperty("text")]
    public string Text { get; set; }
    [JsonProperty("type")]
    public int Type { get; set; }
    [JsonProperty("achieved_val")]
    public int AchievedVal { get; set; }
    [JsonProperty("progress_val")]
    public int ProgressVal { get; set; }
    [JsonProperty("is_achieved")]
    public bool IsAchieved { get; set; }
    [JsonProperty("is_receive_item")]
    public bool IsReceivedItem { get; set; }
    [JsonProperty("item")]
    public ShowUserItemResponse RewardItem { get; set; }
}
