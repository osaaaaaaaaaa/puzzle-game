using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class UpdateUserMailRequest
{
    [JsonProperty("user_mail_id")]
    public int UserMailID { get; set; }
}
