using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ShowUserMailResponse
{
    [JsonProperty("mail_id")]
    public int MailID { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("text")]
    public string Text { get; set; }
    [JsonProperty("is_received")]
    public bool IsReceived { get; set; }
    [JsonProperty("created_at")]
    public string CreatedAt { get; set; }
    [JsonProperty("elapsed_days")]
    public int ElapsedDay { get; set; }
}
