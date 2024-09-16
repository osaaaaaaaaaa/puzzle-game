using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ShowConstantResponse
{
    [JsonProperty("constant")]
    public int Constant { get; set; }
}
