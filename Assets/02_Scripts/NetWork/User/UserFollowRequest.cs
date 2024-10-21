using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class UserFollowRequest
{
    [JsonProperty("following_user_id")]
    public int FollowingUserID { get; set; }
}
