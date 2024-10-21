using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class UpdateUserItemRequest
{
    [JsonProperty("item_id")]
    public int ItemID { get; set; }
    [JsonProperty("option_id")]
    public int OptionID { get; set; }
    [JsonProperty("allie_amount")]
    public int AllieAmount { get; set; }
}
