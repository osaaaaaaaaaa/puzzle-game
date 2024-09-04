using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.IO;

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
#if UNITY_EDITOR
    const string API_BASE_URL = "http://localhost:8000/api/";
#else
    const string API_BASE_URL = "https://api-tikokukaihi.japaneast.cloudapp.azure.com/api/";
#endif
    #endregion

    #region ユーザー情報
    public int UserID { get; private set; } = 0;
    public string UserName { get; private set; } = "";
    public int AchievementID { get; private set; } = 0;
    public int StageID { get; private set; } = 0;
    public int IconID { get; private set; } = 0;
    public int TotalScore { get; private set; } = 0;
    public List<ShowStageResultResponse> StageResults { get; private set; } = new List<ShowStageResultResponse>();
    #endregion

    /// <summary>
    /// ユーザー登録処理
    /// </summary>
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
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
    /// <returns></returns>
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
        return true;
    }

    /// <summary>
    /// ユーザー情報取得処理
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
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
            this.AchievementID = response.AchievementID;
            this.StageID = response.StageID;
            this.IconID = response.IconID;
            this.TotalScore = response.TotalScore;

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
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public IEnumerator UpdateUser(string name, int achievement_id, int stage_id, int icon_id, Action<ErrorResponse> result)
    {
        // サーバーに送信するオブジェクトを作成
        UpdateUserData requestData = new UpdateUserData();
        requestData.UserID = UserID;
        requestData.Name = name;
        requestData.AchievementID = achievement_id;
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
            this.AchievementID = achievement_id;
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
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
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
            // 通信が成功した場合、ファイルにユーザー情報を保存する
            this.UserName = name;
            SaveUserData();
            isSuccess = true;
        }

        // 呼び出し元のresult処理を呼び出す
        result?.Invoke(isSuccess);
    }

    /// <summary>
    /// 所持アイテム取得処理
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
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
    /// <param name="result"></param>
    /// <returns></returns>
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
    /// <param name="result"></param>
    /// <returns></returns>
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
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
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
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
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
    /// <param name="result"></param>
    /// <returns></returns>
    public IEnumerator GetStageResults(Action<ShowStageResultResponse[]> result)
    {
        // 送信
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/stage/result/show?user_id=" + UserID);

        // 結果を受信するまで待機
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
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
    /// <returns></returns>
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
    /// <param name="result"></param>
    /// <returns></returns>
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
    /// <param name="result"></param>
    /// <returns></returns>
    public IEnumerator GetFollowRankingList(Action<ShowRankingResponse[]> result)
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
    /// 救難信号のログ(ホスト)取得処理
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
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
    /// <param name="result"></param>
    /// <returns></returns>
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
}
