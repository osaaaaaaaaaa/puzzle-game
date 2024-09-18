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
    // �V���O���g�� ... �C���X�^���X�𕡐����݂����Ȃ��A�f�U�C���p�^�[��

    // �C���X�^���X�쐬
    private static NetworkManager instance;
    public static NetworkManager Instance
    {
        get
        {
            // GET�v���p�e�B���Ă΂ꂽ�Ƃ��ɃC���X�^���X���쐬����(����̂�)
            if (instance == null)
            {
                GameObject gameObj = new GameObject("NetworkManager");
                instance = gameObj.AddComponent<NetworkManager>();
                DontDestroyOnLoad(gameObj);
            }
            return instance;
        }
    }

    #region API�ڑ����
    const string API_BASE_URL = "https://api-tikokukaihi.japaneast.cloudapp.azure.com/api/";
    #endregion

    #region ���[�U�[���
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

    #region �X�e�[�W���U���g���E�~��M�����
    public List<ShowStageResultResponse> StageResults { get; private set; } = new List<ShowStageResultResponse>();
    public List<ShowDistressSignalResponse> dSignalList { get; private set; } = new List<ShowDistressSignalResponse>();
    #endregion

    // �~��M���V�X�e�����������A�C�e��ID
    const int c_DistressSignalEnableItemID = 35;
    // �X�e�[�W�Ŏg�p�ł���A�C�e��ID
    public int GolfClubItemID { get; private set; } = 34;

    /// <summary>
    /// string�^����Vector3�^�ɕϊ�����
    /// </summary>
    public Vector3 StringToVector3(string strVector)
    {
        // �s�v��()���폜
        strVector = strVector.Replace("(", "").Replace(")", "");
        // �C�ŕ�������xyz���擾����
        string[] strValues = strVector.Split(",");

        // float�^�ɕϊ�����Vector3���쐬����
        if(float.TryParse(strValues[0], out float posX) 
            && float.TryParse(strValues[1], out float posY) 
            && float.TryParse(strValues[2], out float posZ))
        {
            return new Vector3(posX, posY, posZ);
        }

        return Vector3.zero;
    }

    /// <summary>
    /// �~��M���̃`���[�g���A�����������Ƃ��L�^����
    /// </summary>
    public void TutrialViewed()
    {
        this.IsDistressSignalTutrial = true;
        SaveUserData();
    }

    /// <summary>
    /// ���[�U�[�o�^����
    /// </summary>
    public IEnumerator StoreUser(string name, Action<bool> result)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        StoreUserRequest requestData = new StoreUserRequest();
        requestData.Name = name;
        // �T�[�o�[�ɑ��M�I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/store", json, "application/json");

        // ���ʂ���M����Ȃőҋ@
        yield return request.SendWebRequest();
        bool isSuccess = false;

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            StoreUserResponse response = JsonConvert.DeserializeObject<StoreUserResponse>(resultJson);
            // �t�@�C���Ƀ��[�U�[����ۑ�����
            this.UserName = name;
            this.UserID = response.UserID;
            this.IsDistressSignalTutrial = false;
            SaveUserData();
            isSuccess = true;
        }

        // �Ăяo������result�������Ăяo��
        result?.Invoke(isSuccess);
    }

    /// <summary>
    /// ���[�U�[�������[�J���ɕۑ�����
    /// </summary>
    void SaveUserData()
    {
        SaveData saveData = new SaveData();
        saveData.Name = this.UserName;
        saveData.UserID = this.UserID;
        saveData.IsDistressSignalTutrial = this.IsDistressSignalTutrial;
        string json = JsonConvert.SerializeObject(saveData);
        // Application.persistentDataPath��OS���ŕۑ��ꏊ���Œ肳��Ă���
        var writer = new StreamWriter(Application.persistentDataPath + "/saveData.json");
        writer.Write(json);
        writer.Flush();     // �����ɏ����o���悤���߂���
        writer.Close();
    }

    /// <summary>
    /// ���[�U�[�������[�J������ǂݍ���
    /// </summary>
    public bool LoadUserData()
    {
        // �t�@�C���̑��݃`�F�b�N
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
    /// ���[�U�[���擾����
    /// </summary>
    public IEnumerator GetUserData(Action<ShowUserResponse> result)
    {
        // ���M
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/show?user_id=" + UserID);

        // ���ʂ���M����Ȃőҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowUserResponse response = JsonConvert.DeserializeObject<ShowUserResponse>(resultJson);
            this.TitleID = response.TitleID;
            this.StageID = response.StageID;
            this.IconID = response.IconID;
            this.TotalScore = response.TotalScore;
            this.IsDistressSignalEnabled = response.IsDistressSignalEnabled;

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// ���[�U�[���X�V����
    /// </summary>
    public IEnumerator UpdateUser(string name, int title_id, int stage_id, int icon_id, Action<ErrorResponse> result)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        UpdateUserData requestData = new UpdateUserData();
        requestData.UserID = UserID;
        requestData.Name = name;
        requestData.TitleID = title_id;
        requestData.StageID = stage_id;
        requestData.IconID = icon_id;
        // �T�[�o�[�ɑ��M�I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/update", json, "application/json");

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�t�@�C���Ƀ��[�U�[����ۑ�����
            this.UserName = name;
            this.TitleID = title_id;
            this.StageID = stage_id;
            this.IconID = icon_id;
            SaveUserData();

            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
        else
        {
            // �G���[�����擾����
            string resultJson = request.downloadHandler.text;
            ErrorResponse response = JsonConvert.DeserializeObject<ErrorResponse>(resultJson);

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
    }

    /// <summary>
    /// �����A�C�e���X�V����
    /// </summary>
    public IEnumerator UpdateUserItem(int itemID, int optionID, int allieAmount, Action<bool> result)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        UpdateUserItemRequest requestData = new UpdateUserItemRequest();
        requestData.UserID = UserID;
        requestData.ItemID = itemID;
        requestData.OptionID = optionID;
        requestData.AllieAmount = allieAmount;
        // �T�[�o�[�ɑ��M�I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/item/update", json, "application/json");

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();
        bool isSuccess = false;

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            if (itemID == this.GolfClubItemID) this.ItemCnt = this.ItemCnt + allieAmount < 0 ? 0 : this.ItemCnt + allieAmount; // �����A�C�e�������X�V����

            isSuccess = true;
        }

        // �Ăяo������result�������Ăяo��
        result?.Invoke(isSuccess);
    }

    /// <summary>
    /// �����A�C�e���擾����
    /// </summary>
    public IEnumerator GetUserItem(int type, Action<ShowUserItemResponse[]> result)
    {
        // ���M
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/item/show?user_id=" + UserID + "&type=" + type);

        // ���ʂ���M����Ȃőҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowUserItemResponse[] response = JsonConvert.DeserializeObject<ShowUserItemResponse[]>(resultJson);

            if (type == 3 && response.Length != 0) this.ItemCnt = response[0].Amount;

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// �t�H���[���X�g�擾����
    /// </summary>
    public IEnumerator GetFollowList(Action<ShowUserFollowResponse[]> result)
    {
        // ���M
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/follow/show?user_id=" + UserID);

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowUserFollowResponse[] response = JsonConvert.DeserializeObject<ShowUserFollowResponse[]>(resultJson);

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// �������߂̃��[�U�[���X�g�擾����
    /// </summary>
    public IEnumerator GetRecommendedUserList(Action<ShowUserRecommendedResponse[]> result)
    {
        // ���M
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/recommended/show?user_id=" + UserID);

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowUserRecommendedResponse[] response = JsonConvert.DeserializeObject<ShowUserRecommendedResponse[]>(resultJson);

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// �t�H���[�o�^����
    /// </summary>
    public IEnumerator StoreUserFollow(int following_user_id, Action<ErrorResponse> result)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        UserFollowRequest requestData = new UserFollowRequest();
        requestData.UserID = UserID;
        requestData.FollowingUserID = following_user_id;
        // �T�[�o�[�ɑ��M�I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/follow/store", json, "application/json");

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
        else
        {
            // �G���[�����擾����
            string resultJson = request.downloadHandler.text;
            ErrorResponse response = JsonConvert.DeserializeObject<ErrorResponse>(resultJson);

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
    }

    /// <summary>
    /// �t�H���[��������
    /// </summary>
    public IEnumerator DestroyUserFollow(int following_user_id, Action<bool> result)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        UserFollowRequest requestData = new UserFollowRequest();
        requestData.UserID = UserID;
        requestData.FollowingUserID = following_user_id;
        // �T�[�o�[�ɑ��M�I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/follow/destroy", json, "application/json");

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();
        bool isSuccess = false;

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            isSuccess = true;
        }

        // �Ăяo������result�������Ăяo��
        result?.Invoke(isSuccess);
    }

    /// <summary>
    /// �X�e�[�W���U���g�̎擾����
    /// </summary>
    public IEnumerator GetStageResults(Action<ShowStageResultResponse[]> result)
    {
        // ���M
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/stage/result/show?user_id=" + UserID);

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            Debug.Log("����");
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowStageResultResponse[] response = JsonConvert.DeserializeObject<ShowStageResultResponse[]>(resultJson);
            this.StageResults = new (response);

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// �X�e�[�W�N���A����
    /// </summary>
    public IEnumerator UpdateStageClear(bool isUpdateUserStageID, ShowStageResultResponse clearData , Action<UpdateStageClearRequest> result)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        UpdateStageClearRequest requestData = new UpdateStageClearRequest();
        requestData.UserID = this.UserID;
        requestData.StageID = clearData.StageID;
        requestData.IsMedal1 = clearData.IsMedal1;
        requestData.IsMedal2 = clearData.IsMedal2;
        requestData.Time = clearData.Time;
        requestData.Score = clearData.Score;
        // �T�[�o�[�ɑ��M�I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/stage/clear/update", json, "application/json");

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowStageResultResponse response = JsonConvert.DeserializeObject<ShowStageResultResponse>(resultJson);

            if(this.StageResults.Count < clearData.StageID)
            {
                // �f�[�^�����݂��Ȃ��ꍇ�͒ǉ�
                this.StageResults.Add(response);
            }
            else
            {
                // �f�[�^�����݂���ꍇ�͍X�V
                this.StageResults[clearData.StageID - 1] = response;
            }

            // ���[�U�[���X�e�[�W�����N���A�����ꍇ
            if (isUpdateUserStageID) this.StageID++;

            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// �����L���O�擾����
    /// </summary>
    public IEnumerator GetRankingList(Action<ShowRankingResponse[]> result)
    {
        // ���M
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/ranking/show?user_id=" + UserID);

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowRankingResponse[] response = JsonConvert.DeserializeObject<ShowRankingResponse[]>(resultJson);

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// �t�H���[���ł̃����L���O�擾����
    /// </summary>
    public IEnumerator GetFollowRankingList(Action<ShowUserProfileResponse[]> result)
    {
        // ���M
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/follow/ranking/show?user_id=" + UserID);

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowUserProfileResponse[] response = JsonConvert.DeserializeObject<ShowUserProfileResponse[]>(resultJson);

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// �A�`�[�u�����g�ꗗ�擾����
    /// </summary>
    public IEnumerator GetAchievementList(Action<ShowAchievementResponse[]> result)
    {
        // ���M
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "achievements?user_id=" + UserID);

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowAchievementResponse[] response = JsonConvert.DeserializeObject<ShowAchievementResponse[]>(resultJson);

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// �A�`�[�u�����g�B���󋵍X�V����
    /// </summary>
    public IEnumerator UpdateUserAchievement(int type,int allieVal, Action<bool> result)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        UpdateUserAchievementRequest requestData = new UpdateUserAchievementRequest();
        requestData.UserID = UserID;
        requestData.Type = type;
        requestData.AllieVal = allieVal;
        // �T�[�o�[�ɑ��M�I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/achievements/update", json, "application/json");

        // ���ʂ���M����܂őҋ@
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
    /// �A�`�[�u�����g��V�󂯎�菈��
    /// </summary>
    public IEnumerator ReceiveRewardAchievement(int achievement_id, Action<bool> result)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        ReceiveRewardAchievementRequest requestData = new ReceiveRewardAchievementRequest();
        requestData.UserID = UserID;
        requestData.AchievementID = achievement_id;
        // �T�[�o�[�ɑ��M�I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/achievements/receive", json, "application/json");

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        bool isSuccess = false;
        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowUserItemResponse response = JsonConvert.DeserializeObject<ShowUserItemResponse>(resultJson);
            if (response.ItemID == c_DistressSignalEnableItemID) this.IsDistressSignalEnabled = true;    // �~��M���V�X�e�����J��
            if (response.ItemID == this.GolfClubItemID) this.ItemCnt = this.ItemCnt + response.Amount < 0 ? 0 : this.ItemCnt + response.Amount; // �����A�C�e�������X�V����

            isSuccess = true;
        }
        result?.Invoke(isSuccess);
    }

    /// <summary>
    /// ��M���[���ꗗ�擾����
    /// </summary>
    public IEnumerator GetUserMailList(Action<ShowUserMailResponse[]> result)
    {
        // ���M
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/mail/show?user_id=" + UserID);

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowUserMailResponse[] response = JsonConvert.DeserializeObject<ShowUserMailResponse[]>(resultJson);

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// ��M���[���J������
    /// </summary>
    public IEnumerator UpdateUserMail(int userMailID, Action<ShowUserItemResponse[]> result)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        UpdateUserMailRequest requestData = new UpdateUserMailRequest();
        requestData.UserID = UserID;
        requestData.UserMailID = userMailID;
        // �T�[�o�[�ɑ��M�I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/mail/update", json, "application/json");

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowUserItemResponse[] response = JsonConvert.DeserializeObject<ShowUserItemResponse[]>(resultJson);

            for (int i = 0; i < response.Length; i++)
            {
                if (response[i].ItemID == c_DistressSignalEnableItemID) this.IsDistressSignalEnabled = true;    // �~��M���V�X�e�����J��
                if (response[i].ItemID == this.GolfClubItemID) this.ItemCnt = this.ItemCnt + response[i].Amount < 0 ? 0 : this.ItemCnt + response[i].Amount; // �����A�C�e�������X�V����
            }

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
        else
        {
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// ��M���[���폜����
    /// </summary>
    public IEnumerator DestroyUserMail(int userMailID, Action<bool> result)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        UpdateUserMailRequest requestData = new UpdateUserMailRequest();
        requestData.UserID = UserID;
        requestData.UserMailID = userMailID;
        // �T�[�o�[�ɑ��M�I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/mail/destroy", json, "application/json");

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();
        bool isSuccess = false;

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            isSuccess = true;
        }

        // �Ăяo������result�������Ăяo��
        result?.Invoke(isSuccess);
    }

    /// <summary>
    /// ��������W��(���N���A)�̋~��M���擾����
    /// </summary>
    public IEnumerator GetDistressSignalList(Action<ShowDistressSignalResponse[]> result)
    {
        // ���M
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "distress_signals/index?user_id=" + UserID);

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowDistressSignalResponse[] response = JsonConvert.DeserializeObject<ShowDistressSignalResponse[]>(resultJson);
            dSignalList = new (response);

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// �~��M���o�^����
    /// </summary>
    public IEnumerator StoreDistressSignal(int stage_id, Action<ShowDistressSignalResponse> result, Action<string> error)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        StoreDistressSignalRequest requestData = new StoreDistressSignalRequest();
        requestData.UserID = UserID;
        requestData.StageID = stage_id;
        // �T�[�o�[�ɑ��M�I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "distress_signals/store", json, "application/json");

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowDistressSignalResponse response = JsonConvert.DeserializeObject<ShowDistressSignalResponse>(resultJson);
            dSignalList.Add(response);
            result?.Invoke(response);
        }
        else if (request.responseCode == 400)
        {
            // �Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ErrorResponse response = JsonConvert.DeserializeObject<ErrorResponse>(resultJson);

            error?.Invoke(response.Error);
        }
        else
        {
            error?.Invoke("�ʐM�G���[���������܂���");
        }
    }

    /// <summary>
    /// �Q�X�g�o�^�i�~��M���Q���j�E�z�u���X�V����
    /// </summary>
    public IEnumerator UpdateSignalGuest(int signalID,string pos, string vec, Action<string> result)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        UpdateSignalGuestRequest requestData = new UpdateSignalGuestRequest();
        requestData.SignalID = signalID;
        requestData.UserID = UserID;
        requestData.Pos = pos;
        requestData.Vector = vec;
        // �T�[�o�[�ɑ��M�I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "distress_signals/guest/update", json, "application/json");

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            result?.Invoke(null);
        }
        else if (request.responseCode == 400)
        {
            // �Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ErrorResponse response = JsonConvert.DeserializeObject<ErrorResponse>(resultJson);

            result?.Invoke(response.Error);
        }
        else
        {
            result?.Invoke("�ʐM�G���[���������܂���");
        }
    }

    /// <summary>
    /// ��W��or��W�����~��M���̍폜����
    /// </summary>
    public IEnumerator DestroyDistressSignal(int signalID, Action<bool> result)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        DistressSignalGuestRequest requestData = new DistressSignalGuestRequest();
        requestData.SignalID = signalID;
        // �T�[�o�[�ɑ��M�I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "distress_signals/destroy", json, "application/json");

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();
        bool isSuccess = false;

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            isSuccess = true;

            // ��������W���̋~��M�����X�g����폜�������v�f���������č폜����
            ShowDistressSignalResponse dSignal = dSignalList.FirstOrDefault(item => item.SignalID == signalID);
            if (dSignal != null) dSignalList.Remove(dSignal);
        }

        // �Ăяo������result�������Ăяo��
        result?.Invoke(isSuccess);
    }

    /// <summary>
    /// �Q�X�g�폜����
    /// </summary>
    public IEnumerator DestroySignalGuest(int signalID, int userID, Action<bool> result)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        DistressSignalGuestRequest requestData = new DistressSignalGuestRequest();
        requestData.SignalID = signalID;
        requestData.UserID = userID;
        // �T�[�o�[�ɑ��M�I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "distress_signals/guest/destroy", json, "application/json");

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();
        bool isSuccess = false;

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            isSuccess = true;
        }

        // �Ăяo������result�������Ăяo��
        result?.Invoke(isSuccess);
    }

    /// <summary>
    /// �~��M���ɎQ�����Ă��郆�[�U�[�̃v���t�B�[���擾
    /// </summary>
    public IEnumerator GetSignalUserProfile(int signalID,Action<ShowUserProfileResponse[]> result)
    {
        // ���M
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "distress_signals/user/show?d_signal_id="+ signalID + "&user_id="+ UserID);

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowUserProfileResponse[] response = JsonConvert.DeserializeObject<ShowUserProfileResponse[]>(resultJson);

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// �~��M���ɎQ�����Ă���Q�X�g�̔z�u�����擾����
    /// </summary>
    public IEnumerator GetSignalGuest(int signalID, Action<ShowSignalGuestResponse[]> result)
    {
        // ���M
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "distress_signals/guest/show?d_signal_id=" + signalID);

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowSignalGuestResponse[] response = JsonConvert.DeserializeObject<ShowSignalGuestResponse[]>(resultJson);

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// �~��M���N���A����
    /// </summary>
    public IEnumerator UpdateDistressSignal(int signalID,Action<bool> result)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        UpdateDistressSignal requestData = new UpdateDistressSignal();
        requestData.SignalID = signalID;
        // �T�[�o�[�ɑ��M�I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "distress_signals/update", json, "application/json");

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �����ɍ����~��M�����擾����
            var dSignal = dSignalList.FirstOrDefault(item => item.SignalID == signalID);
            dSignalList.Remove(dSignal);

            foreach (var test in dSignalList)
            {
                Debug.Log("ID:" + test.SignalID + ",Stage:" + test.StageID);
            }

            // �Ăяo������result�������Ăяo��
            result?.Invoke(true);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(false);
        }
    }

    /// <summary>
    /// ���v���C���擾����
    /// </summary>
    public IEnumerator GetReplayData(int signalID,Action<List<ReplayData>> result)
    {
        // ���M
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "distress_signals/replay/show?d_signal_id=" + signalID);

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            UpdateReplayData response = JsonConvert.DeserializeObject<UpdateReplayData>(resultJson);
            List<ReplayData> replayDatas = new(response.ReplayDatas);

            // �Ăяo������result�������Ăяo��
            result?.Invoke(replayDatas);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// ���v���C���X�V����
    /// </summary>
    public IEnumerator UpdateReplayData(int signalID, List<ReplayData> replayDatas, Action<bool> result)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        UpdateReplayData requestData = new UpdateReplayData() { SignalID = signalID, ReplayDatas = replayDatas };
        // �T�[�o�[�ɑ��M�I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "distress_signals/replay/update", json, "application/json");

        // ���ʂ���M����܂őҋ@
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
    /// �~��M���̃��O(�z�X�g)�擾����
    /// </summary>
    public IEnumerator GetSignalHostLogList(Action<ShowHostLogResponse[]> result)
    {
        // ���M
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "distress_signals/host_log?user_id=" + UserID);

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowHostLogResponse[] response = JsonConvert.DeserializeObject<ShowHostLogResponse[]>(resultJson);

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// �~��M���̃��O(�Q�X�g)�擾����
    /// </summary>
    public IEnumerator GetSignalGuestLogList(Action<ShowGuestLogResponse[]> result)
    {
        // ���M
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "distress_signals/guest_log?user_id=" + UserID);

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowGuestLogResponse[] response = JsonConvert.DeserializeObject<ShowGuestLogResponse[]>(resultJson);

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// �����_���ɋ~��M���擾����
    /// </summary>
    public IEnumerator GetRndSignalList(Action<ShowRndSignalResponse[]> result)
    {
        // ���M
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "distress_signals/show?user_id=" + UserID);

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowRndSignalResponse[] response = JsonConvert.DeserializeObject<ShowRndSignalResponse[]>(resultJson);

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// �Q�X�g�̕�V�󂯎�菈��
    /// </summary>
    public IEnumerator UpdateSignalGuestReward(int signalID, Action<ShowUserItemResponse> result)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        DistressSignalGuestRequest requestData = new DistressSignalGuestRequest();
        requestData.SignalID = signalID;
        requestData.UserID = this.UserID;

        // �T�[�o�[�ɑ��M�I�u�W�F�N�g��JSON�ɕϊ�
        string json = JsonConvert.SerializeObject(requestData);
        // ���M
        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "distress_signals/reward/update", json, "application/json");

        // ���ʂ���M����Ȃőҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowUserItemResponse response = JsonConvert.DeserializeObject<ShowUserItemResponse>(resultJson);

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

    /// <summary>
    /// �萔�擾����
    /// </summary>
    public IEnumerator GetConstant(int type,Action<ShowConstantResponse> result)
    {
        // ���M
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "constant?type=" + type);

        // ���ʂ���M����Ȃőҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
            // �ʐM�����������ꍇ�A�Ԃ��Ă���JSON���I�u�W�F�N�g�ɕϊ�
            string resultJson = request.downloadHandler.text;
            ShowConstantResponse response = JsonConvert.DeserializeObject<ShowConstantResponse>(resultJson);

            // �Ăяo������result�������Ăяo��
            result?.Invoke(response);
        }
        else
        {
            // �Ăяo������result�������Ăяo��
            result?.Invoke(null);
        }
    }

}
