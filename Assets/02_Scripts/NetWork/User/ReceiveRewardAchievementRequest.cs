using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ReceiveRewardAchievementRequest
{
    [JsonProperty("achievement_id")]
    public int AchievementID { get; set; }
    [JsonProperty("user_id")]
    public int UserID { get; set; }
}
