using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ErrorResponse
{
    [JsonProperty("error")]
    public string Error { get; set; }
}
