using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class UpdateUserAchievementRequest
{
    [JsonProperty("type")]
    public int Type { get; set; }
    [JsonProperty("allie_val")]
    public int AllieVal { get; set; }
}
