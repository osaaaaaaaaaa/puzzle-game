using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class UpdateUserData
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("title_id")]
    public int TitleID { get; set; }
    [JsonProperty("stage_id")]
    public int StageID { get; set; }
    [JsonProperty("icon_id")]
    public int IconID { get; set; }
}
