using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class StoreDistressSignalRequest
{
    [JsonProperty("user_id")]
    public int UserID { get; set; }
    [JsonProperty("stage_id")]
    public int StageID { get; set; }
}
