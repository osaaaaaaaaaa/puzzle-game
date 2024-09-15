using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class UpdateUserMailRequest
{
    [JsonProperty("mail_id")]
    public int MailID { get; set; }
    [JsonProperty("user_id")]
    public int UserID { get; set; }
}
