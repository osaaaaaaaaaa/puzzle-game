using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class StoreDistressSignalRequest
{
    [JsonProperty("stage_id")]
    public int StageID { get; set; }
}
