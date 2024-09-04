using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class UpdateUserData
{
    [JsonProperty("user_id")]
    public int UserID { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("achievement_id")]
    public int AchievementID { get; set; }
    [JsonProperty("stage_id")]
    public int StageID { get; set; }
    [JsonProperty("icon_id")]
    public int IconID { get; set; }
}