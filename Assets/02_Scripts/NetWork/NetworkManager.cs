using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.IO;
using System.Linq;

public class NetworkManager : MonoBehaviour
{
    // シングルトン ... インスタンスを複数存在させない、デザインパターン

    // インスタンス作成
    private static NetworkManager instance;
    public static NetworkManager Instance
    {
        get
        {
            // GETプロパティを呼ばれたときにインスタンスを作成する(初回のみ)
            if (instance == null)
            {
                GameObject gameObj = new GameObject("NetworkManager");
                instance = gameObj.AddComponent<NetworkManager>();
                DontDestroyOnLoad(gameObj);
            }
            return instance;
        }
    }

    #region API接続情報
    //const string API_BASE_URL = "http://localhost:8000/api/";
    const string API_BASE_URL = "https://api-tikokukaihi.japaneast.cloudapp.azure.com/api/";
    #endregion

    #region ユーザー情報
    public bool IsDistressSignalTutrial { get; private set; } = false;
    public int UserID { get; private set; } = 0;
    public string UserName { get; private set; } = "";
    public int TitleID { get; private set; } = 0;
    public int StageID { get; private set; } = 0;
    public int IconID { get; private set; } = 0;
    public int TotalScore { get; private set; } = 0;
    public bool IsDistressSignalEnabled { get; set; } = false;
    public int ItemCnt { get; private set; } = 0;
    #endregion

    #region ステージリザルト情報・救難信号情報
    public List<ShowStageResultResponse> StageResults { get; private set; } = new List<ShowStageResultResponse>();
    public List<ShowDistressSignalResponse> dSignalList { get; private set; } = new List<ShowDistressSignalResponse>();
    #endregion

    // 救難信号システムを解放するアイテムID
    const int c_DistressSignalEnableItemID = 35;
    // ステージで使用できるアイテムID
    public int GolfClubItemID { get; private set; } = 34;

    /// <summary>
    /// string型からVector3型に変換する
    /// </summary>
    public Vector3 StringToVector3(string strVector)
    {
        // 不要な()を削除
        strVector = strVector.Replace("(", "").Replace(")", "");
        // ，で分割してxyzを取得する
        string[] strValues = strVector.Split(",");

        // float型に変換してVector3を作成する
        if(float.TryParse(strValues[0], out float posX) 
            && float.TryParse(strValues[1], out float posY) 
            && float.TryParse(strValues[2], out float posZ))
        {
            return new Vector3(posX, posY, posZ);
        }

        return Vector3.zero;
    }

    /// <summary>
    /// 救難信号のチュートリアルを見たことを記録する
    /// </summary>
    public void TutrialViewed()
    {
        this.IsDistressSignalTutrial = true;
        SaveUserData();
    }

    /// <summary>
    /// ユーザー登録処理
    /// </summary>
    public IEnumerator StoreUser(string name, Action<bool> result)
    {
        // サーバーに送信するオブジェクトを作成
        StoreUserRequest requestData = new StoreUserRequest();
        requestData.Name = name;
        // サーバーに送信オブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/store", json, "application/json");

        // 結果を受信するなで待機
        yield return request.SendWebRequest();
        bool isSuccess = false;

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            StoreUserResponse response = JsonConvert.DeserializeObject<StoreUserResponse>(resultJson);
            // ファイルにユーザー情報を保存する
            this.UserName = name;
            this.UserID = response.UserID;
            this.IsDistressSignalTutrial = false;
            SaveUserData();
            isSuccess = true;
        }

        // 呼び出し元のresult処理を呼び出す
        result?.Invoke(isSuccess);
    }

    /// <summary>
    /// ユーザー情報をローカルに保存する
    /// </summary>
    void SaveUserData()
    {
        SaveData saveData = new SaveData();
        saveData.Name = this.UserName;
        saveData.UserID = this.UserID;
        saveData.IsDistressSignalTutrial = this.IsDistressSignalTutrial;
        string json = JsonConvert.SerializeObject(saveData);
        // Application.persistentDataPathはOS毎で保存場所が固定されている
        var writer = new StreamWriter(Application.persistentDataPath + "/saveData.json");
        writer.Write(json);
        writer.Flush();     // すぐに書き出すよう命令する
        writer.Close();
    }

    /// <summary>
    /// ユーザー情報をローカルから読み込む
    /// </summary>
    public bool LoadUserData()
    {
        // ファイルの存在チェック
        if (!File.Exists(Application.persistentDataPath + "/saveData.json")) return false;

        var reader = new StreamReader(Application.persistentDataPath + "/saveData.json");
        string json = reader.ReadToEnd();
        reader.Close();
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
        this.UserID = saveData.UserID;
        this.UserName = saveData.Name;
        this.IsDistressSignalTutrial = saveData.IsDistressSignalTutrial;
        return true;
    }

    /// <summary>
    /// ユーザー情報取得処理
    /// </summary>
    public IEnumerator GetUserData(Action<ShowUserResponse> result)
    {
        // 送信
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/show?user_id=" + UserID);

        // 結果を受信するなで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowUserResponse response = JsonConvert.DeserializeObject<ShowUserResponse>(resultJson);
            this.TitleID = response.TitleID;
            this.StageID = response.StageID;
            this.IconID = response.IconID;
            this.TotalScore = response.TotalScore;
            this.IsDistressSignalEnabled = response.IsDistressSignalEnabled;

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// ユーザー情報更新処理
    /// </summary>
    public IEnumerator UpdateUser(string name, int title_id, int stage_id, int icon_id, Action<ErrorResponse> result)
    {
        // サーバーに送信するオブジェクトを作成
        UpdateUserData requestData = new UpdateUserData();
        requestData.UserID = UserID;
        requestData.Name = name;
        requestData.TitleID = title_id;
        requestData.StageID = stage_id;
        requestData.IconID = icon_id;
        // サーバーに送信オブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/update", json, "application/json");

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、ファイルにユーザー情報を保存する
            this.UserName = name;
            this.TitleID = title_id;
            this.StageID = stage_id;
            this.IconID = icon_id;
            SaveUserData();

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
        else
        {
            // エラー文を取得する
            string resultJson = request.downloadHandler.text;
            ErrorResponse response = JsonConvert.DeserializeObject<ErrorResponse>(resultJson);

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
    }

    /// <summary>
    /// 所持アイテム更新処理
    /// </summary>
    public IEnumerator UpdateUserItem(int itemID, int optionID, int allieAmount, Action<bool> result)
    {
        // サーバーに送信するオブジェクトを作成
        UpdateUserItemRequest requestData = new UpdateUserItemRequest();
        requestData.UserID = UserID;
        requestData.ItemID = itemID;
        requestData.OptionID = optionID;
        requestData.AllieAmount = allieAmount;
        // サーバーに送信オブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/item/update", json, "application/json");

        // 結果を受信するまで待機
        yield return request.SendWebRequest();
        bool isSuccess = false;

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            if (itemID == this.GolfClubItemID) this.ItemCnt = this.ItemCnt + allieAmount < 0 ? 0 : this.ItemCnt + allieAmount; // 所持アイテム数を更新する

            isSuccess = true;
        }

        // 呼び出し元のresult処理を呼び出す
        result?.Invoke(isSuccess);
    }

    /// <summary>
    /// 所持アイテム取得処理
    /// </summary>
    public IEnumerator GetUserItem(int type, Action<ShowUserItemResponse[]> result)
    {
        // 送信
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/item/show?user_id=" + UserID + "&type=" + type);

        // 結果を受信するなで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowUserItemResponse[] response = JsonConvert.DeserializeObject<ShowUserItemResponse[]>(resultJson);

            if (type == 3 && response.Length != 0) this.ItemCnt = response[0].Amount;

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// フォローリスト取得処理
    /// </summary>
    public IEnumerator GetFollowList(Action<ShowUserFollowResponse[]> result)
    {
        // 送信
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/follow/show?user_id=" + UserID);

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowUserFollowResponse[] response = JsonConvert.DeserializeObject<ShowUserFollowResponse[]>(resultJson);

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// おすすめのユーザーリスト取得処理
    /// </summary>
    public IEnumerator GetRecommendedUserList(Action<ShowUserRecommendedResponse[]> result)
    {
        // 送信
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/recommended/show?user_id=" + UserID);

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowUserRecommendedResponse[] response = JsonConvert.DeserializeObject<ShowUserRecommendedResponse[]>(resultJson);

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// フォロー登録処理
    /// </summary>
    public IEnumerator StoreUserFollow(int following_user_id, Action<ErrorResponse> result)
    {
        // サーバーに送信するオブジェクトを作成
        UserFollowRequest requestData = new UserFollowRequest();
        requestData.UserID = UserID;
        requestData.FollowingUserID = following_user_id;
        // サーバーに送信オブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/follow/store", json, "application/json");

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
        else
        {
            // エラー文を取得する
            string resultJson = request.downloadHandler.text;
            ErrorResponse response = JsonConvert.DeserializeObject<ErrorResponse>(resultJson);

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
    }

    /// <summary>
    /// フォロー解除処理
    /// </summary>
    public IEnumerator DestroyUserFollow(int following_user_id, Action<bool> result)
    {
        // サーバーに送信するオブジェクトを作成
        UserFollowRequest requestData = new UserFollowRequest();
        requestData.UserID = UserID;
        requestData.FollowingUserID = following_user_id;
        // サーバーに送信オブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/follow/destroy", json, "application/json");

        // 結果を受信するまで待機
        yield return request.SendWebRequest();
        bool isSuccess = false;

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            isSuccess = true;
        }

        // 呼び出し元のresult処理を呼び出す
        result?.Invoke(isSuccess);
    }

    /// <summary>
    /// ステージリザルトの取得処理
    /// </summary>
    public IEnumerator GetStageResults(Action<ShowStageResultResponse[]> result)
    {
        // 送信
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/stage/result/show?user_id=" + UserID);

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            Debug.Log("成功");
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowStageResultResponse[] response = JsonConvert.DeserializeObject<ShowStageResultResponse[]>(resultJson);
            this.StageResults = new (response);

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// ステージクリア処理
    /// </summary>
    public IEnumerator UpdateStageClear(bool isUpdateUserStageID, ShowStageResultResponse clearData , Action<UpdateStageClearRequest> result)
    {
        // サーバーに送信するオブジェクトを作成
        UpdateStageClearRequest requestData = new UpdateStageClearRequest();
        requestData.UserID = this.UserID;
        requestData.StageID = clearData.StageID;
        requestData.IsMedal1 = clearData.IsMedal1;
        requestData.IsMedal2 = clearData.IsMedal2;
        requestData.Time = clearData.Time;
        requestData.Score = clearData.Score;
        // サーバーに送信オブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/stage/clear/update", json, "application/json");

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowStageResultResponse response = JsonConvert.DeserializeObject<ShowStageResultResponse>(resultJson);

            if(this.StageResults.Count < clearData.StageID)
            {
                // データが存在しない場合は追加
                this.StageResults.Add(response);
            }
            else
            {
                // データが存在する場合は更新
                this.StageResults[clearData.StageID - 1] = response;
            }

            // ユーザーがステージを初クリアした場合
            if (isUpdateUserStageID) this.StageID++;

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// ランキング取得処理
    /// </summary>
    public IEnumerator GetRankingList(Action<ShowRankingResponse[]> result)
    {
        // 送信
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/ranking/show?user_id=" + UserID);

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowRankingResponse[] response = JsonConvert.DeserializeObject<ShowRankingResponse[]>(resultJson);

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// フォロー内でのランキング取得処理
    /// </summary>
    public IEnumerator GetFollowRankingList(Action<ShowUserProfileResponse[]> result)
    {
        // 送信
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/follow/ranking/show?user_id=" + UserID);

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowUserProfileResponse[] response = JsonConvert.DeserializeObject<ShowUserProfileResponse[]>(resultJson);

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// アチーブメント一覧取得処理
    /// </summary>
    public IEnumerator GetAchievementList(Action<ShowAchievementResponse[]> result)
    {
        // 送信
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "achievements?user_id=" + UserID);

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowAchievementResponse[] response = JsonConvert.DeserializeObject<ShowAchievementResponse[]>(resultJson);

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// アチーブメント達成状況更新処理
    /// </summary>
    public IEnumerator UpdateUserAchievement(int type,int allieVal, Action<bool> result)
    {
        // サーバーに送信するオブジェクトを作成
        UpdateUserAchievementRequest requestData = new UpdateUserAchievementRequest();
        requestData.UserID = UserID;
        requestData.Type = type;
        requestData.AllieVal = allieVal;
        // サーバーに送信オブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/achievements/update", json, "application/json");

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        bool isSuccess = false;
        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            isSuccess = true;
        }
        result?.Invoke(isSuccess);
    }

    /// <summary>
    /// アチーブメント報酬受け取り処理
    /// </summary>
    public IEnumerator ReceiveRewardAchievement(int achievement_id, Action<bool> result)
    {
        // サーバーに送信するオブジェクトを作成
        ReceiveRewardAchievementRequest requestData = new ReceiveRewardAchievementRequest();
        requestData.UserID = UserID;
        requestData.AchievementID = achievement_id;
        // サーバーに送信オブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/achievements/receive", json, "application/json");

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        bool isSuccess = false;
        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowUserItemResponse response = JsonConvert.DeserializeObject<ShowUserItemResponse>(resultJson);
            if (response.ItemID == c_DistressSignalEnableItemID) this.IsDistressSignalEnabled = true;    // 救難信号システムを開放
            if (response.ItemID == this.GolfClubItemID) this.ItemCnt = this.ItemCnt + response.Amount < 0 ? 0 : this.ItemCnt + response.Amount; // 所持アイテム数を更新する

            isSuccess = true;
        }
        result?.Invoke(isSuccess);
    }

    /// <summary>
    /// 受信メール一覧取得処理
    /// </summary>
    public IEnumerator GetUserMailList(Action<ShowUserMailResponse[]> result)
    {
        // 送信
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/mail/show?user_id=" + UserID);

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowUserMailResponse[] response = JsonConvert.DeserializeObject<ShowUserMailResponse[]>(resultJson);

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// 受信メール開封処理
    /// </summary>
    public IEnumerator UpdateUserMail(int userMailID, Action<ShowUserItemResponse[]> result)
    {
        // サーバーに送信するオブジェクトを作成
        UpdateUserMailRequest requestData = new UpdateUserMailRequest();
        requestData.UserID = UserID;
        requestData.UserMailID = userMailID;
        // サーバーに送信オブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/mail/update", json, "application/json");

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowUserItemResponse[] response = JsonConvert.DeserializeObject<ShowUserItemResponse[]>(resultJson);

            for (int i = 0; i < response.Length; i++)
            {
                if (response[i].ItemID == c_DistressSignalEnableItemID) this.IsDistressSignalEnabled = true;    // 救難信号システムを開放
                if (response[i].ItemID == this.GolfClubItemID) this.ItemCnt = this.ItemCnt + response[i].Amount < 0 ? 0 : this.ItemCnt + response[i].Amount; // 所持アイテム数を更新する
            }

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
        else
        {
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// 受信メール削除処理
    /// </summary>
    public IEnumerator DestroyUserMail(int userMailID, Action<bool> result)
    {
        // サーバーに送信するオブジェクトを作成
        UpdateUserMailRequest requestData = new UpdateUserMailRequest();
        requestData.UserID = UserID;
        requestData.UserMailID = userMailID;
        // サーバーに送信オブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/mail/destroy", json, "application/json");

        // 結果を受信するまで待機
        yield return request.SendWebRequest();
        bool isSuccess = false;

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            isSuccess = true;
        }

        // 呼び出し元のresult処理を呼び出す
        result?.Invoke(isSuccess);
    }

    /// <summary>
    /// 自分が募集中(未クリア)の救難信号取得処理
    /// </summary>
    public IEnumerator GetDistressSignalList(Action<ShowDistressSignalResponse[]> result)
    {
        // 送信
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "distress_signals/index?user_id=" + UserID);

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowDistressSignalResponse[] response = JsonConvert.DeserializeObject<ShowDistressSignalResponse[]>(resultJson);
            dSignalList = new (response);

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// 救難信号登録処理
    /// </summary>
    public IEnumerator StoreDistressSignal(int stage_id, Action<ShowDistressSignalResponse> result, Action<string> error)
    {
        // サーバーに送信するオブジェクトを作成
        StoreDistressSignalRequest requestData = new StoreDistressSignalRequest();
        requestData.UserID = UserID;
        requestData.StageID = stage_id;
        // サーバーに送信オブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "distress_signals/store", json, "application/json");

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowDistressSignalResponse response = JsonConvert.DeserializeObject<ShowDistressSignalResponse>(resultJson);
            dSignalList.Add(response);
            result?.Invoke(response);
        }
        else if (request.responseCode == 400)
        {
            // 返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ErrorResponse response = JsonConvert.DeserializeObject<ErrorResponse>(resultJson);

            error?.Invoke(response.Error);
        }
        else
        {
            error?.Invoke("通信エラーが発生しました");
        }
    }

    /// <summary>
    /// ゲスト登録（救難信号参加）・配置情報更新処理
    /// </summary>
    public IEnumerator UpdateSignalGuest(int signalID,string pos, string vec, Action<string> result)
    {
        // サーバーに送信するオブジェクトを作成
        UpdateSignalGuestRequest requestData = new UpdateSignalGuestRequest();
        requestData.SignalID = signalID;
        requestData.UserID = UserID;
        requestData.Pos = pos;
        requestData.Vector = vec;
        // サーバーに送信オブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "distress_signals/guest/update", json, "application/json");

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            result?.Invoke(null);
        }
        else if (request.responseCode == 400)
        {
            // 返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ErrorResponse response = JsonConvert.DeserializeObject<ErrorResponse>(resultJson);

            result?.Invoke(response.Error);
        }
        else
        {
            result?.Invoke("通信エラーが発生しました");
        }
    }

    /// <summary>
    /// 募集中or募集した救難信号の削除処理
    /// </summary>
    public IEnumerator DestroyDistressSignal(int signalID, Action<bool> result)
    {
        // サーバーに送信するオブジェクトを作成
        DistressSignalGuestRequest requestData = new DistressSignalGuestRequest();
        requestData.SignalID = signalID;
        // サーバーに送信オブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "distress_signals/destroy", json, "application/json");

        // 結果を受信するまで待機
        yield return request.SendWebRequest();
        bool isSuccess = false;

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            isSuccess = true;

            // 自分が募集中の救難信号リストから削除したい要素を検索して削除する
            ShowDistressSignalResponse dSignal = dSignalList.FirstOrDefault(item => item.SignalID == signalID);
            if (dSignal != null) dSignalList.Remove(dSignal);
        }

        // 呼び出し元のresult処理を呼び出す
        result?.Invoke(isSuccess);
    }

    /// <summary>
    /// ゲスト削除処理
    /// </summary>
    public IEnumerator DestroySignalGuest(int signalID, int userID, Action<bool> result)
    {
        // サーバーに送信するオブジェクトを作成
        DistressSignalGuestRequest requestData = new DistressSignalGuestRequest();
        requestData.SignalID = signalID;
        requestData.UserID = userID;
        // サーバーに送信オブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "distress_signals/guest/destroy", json, "application/json");

        // 結果を受信するまで待機
        yield return request.SendWebRequest();
        bool isSuccess = false;

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            isSuccess = true;
        }

        // 呼び出し元のresult処理を呼び出す
        result?.Invoke(isSuccess);
    }

    /// <summary>
    /// 救難信号に参加しているユーザーのプロフィール取得
    /// </summary>
    public IEnumerator GetSignalUserProfile(int signalID,Action<ShowUserProfileResponse[]> result)
    {
        // 送信
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "distress_signals/user/show?d_signal_id="+ signalID + "&user_id="+ UserID);

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowUserProfileResponse[] response = JsonConvert.DeserializeObject<ShowUserProfileResponse[]>(resultJson);

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// 救難信号に参加しているゲストの配置情報を取得する
    /// </summary>
    public IEnumerator GetSignalGuest(int signalID, Action<ShowSignalGuestResponse[]> result)
    {
        // 送信
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "distress_signals/guest/show?d_signal_id=" + signalID);

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowSignalGuestResponse[] response = JsonConvert.DeserializeObject<ShowSignalGuestResponse[]>(resultJson);

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// 救難信号クリア処理
    /// </summary>
    public IEnumerator UpdateDistressSignal(int signalID,Action<bool> result)
    {
        // サーバーに送信するオブジェクトを作成
        UpdateDistressSignal requestData = new UpdateDistressSignal();
        requestData.SignalID = signalID;
        // サーバーに送信オブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "distress_signals/update", json, "application/json");

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 条件に合う救難信号を取得する
            var dSignal = dSignalList.FirstOrDefault(item => item.SignalID == signalID);
            dSignalList.Remove(dSignal);

            foreach (var test in dSignalList)
            {
                Debug.Log("ID:" + test.SignalID + ",Stage:" + test.StageID);
            }

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(true);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(false);
        }
    }

    /// <summary>
    /// リプレイ情報取得処理
    /// </summary>
    public IEnumerator GetReplayData(int signalID,Action<List<ReplayData>> result)
    {
        // 送信
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "distress_signals/replay/show?d_signal_id=" + signalID);

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            UpdateReplayData response = JsonConvert.DeserializeObject<UpdateReplayData>(resultJson);
            List<ReplayData> replayDatas = new(response.ReplayDatas);

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(replayDatas);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// リプレイ情報更新処理
    /// </summary>
    public IEnumerator UpdateReplayData(int signalID, List<ReplayData> replayDatas, Action<bool> result)
    {
        // サーバーに送信するオブジェクトを作成
        UpdateReplayData requestData = new UpdateReplayData() { SignalID = signalID, ReplayDatas = replayDatas };
        // サーバーに送信オブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "distress_signals/replay/update", json, "application/json");

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        bool isSuccess = false;
        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            isSuccess = true;
        }

        result?.Invoke(isSuccess);
    }

    /// <summary>
    /// 救難信号のログ(ホスト)取得処理
    /// </summary>
    public IEnumerator GetSignalHostLogList(Action<ShowHostLogResponse[]> result)
    {
        // 送信
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "distress_signals/host_log?user_id=" + UserID);

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowHostLogResponse[] response = JsonConvert.DeserializeObject<ShowHostLogResponse[]>(resultJson);

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// 救難信号のログ(ゲスト)取得処理
    /// </summary>
    public IEnumerator GetSignalGuestLogList(Action<ShowGuestLogResponse[]> result)
    {
        // 送信
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "distress_signals/guest_log?user_id=" + UserID);

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowGuestLogResponse[] response = JsonConvert.DeserializeObject<ShowGuestLogResponse[]>(resultJson);

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// ランダムに救難信号取得処理
    /// </summary>
    public IEnumerator GetRndSignalList(Action<ShowRndSignalResponse[]> result)
    {
        // 送信
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "distress_signals/show?user_id=" + UserID);

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowRndSignalResponse[] response = JsonConvert.DeserializeObject<ShowRndSignalResponse[]>(resultJson);

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// ゲストの報酬受け取り処理
    /// </summary>
    public IEnumerator UpdateSignalGuestReward(int signalID, Action<ShowUserItemResponse> result)
    {
        // サーバーに送信するオブジェクトを作成
        DistressSignalGuestRequest requestData = new DistressSignalGuestRequest();
        requestData.SignalID = signalID;
        requestData.UserID = this.UserID;

        // サーバーに送信オブジェクトをJSONに変換
        string json = JsonConvert.SerializeObject(requestData);
        // 送信
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "distress_signals/reward/update", json, "application/json");

        // 結果を受信するなで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowUserItemResponse response = JsonConvert.DeserializeObject<ShowUserItemResponse>(resultJson);

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// 定数取得処理
    /// </summary>
    public IEnumerator GetConstant(int type,Action<ShowConstantResponse> result)
    {
        // 送信
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "constant?type=" + type);

        // 結果を受信するなで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // 通信が成功した場合、返ってきたJSONをオブジェクトに変換
            string resultJson = request.downloadHandler.text;
            ShowConstantResponse response = JsonConvert.DeserializeObject<ShowConstantResponse>(resultJson);

            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(response);
        }
        else
        {
            // 呼び出し元のresult処理を呼び出す
            result?.Invoke(null);
        }
    }

}
