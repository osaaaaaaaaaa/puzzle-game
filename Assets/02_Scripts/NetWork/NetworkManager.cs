using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.IO;

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
#if UNITY_EDITOR
    const string API_BASE_URL = "http://localhost:8000/api/";
#else
    const string API_BASE_URL = "https://api-tikokukaihi.japaneast.cloudapp.azure.com/api/";
#endif
    #endregion

    #region ���[�U�[���
    public int UserID { get; private set; } = 0;
    public string UserName { get; private set; } = "";
    public int AchievementID { get; private set; } = 0;
    public int StageID { get; private set; } = 0;
    public int IconID { get; private set; } = 0;
    public int TotalScore { get; private set; } = 0;
    public List<ShowStageResultResponse> StageResults { get; private set; } = new List<ShowStageResultResponse>();
    #endregion

    /// <summary>
    /// ���[�U�[�o�^����
    /// </summary>
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
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
    /// <returns></returns>
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
        return true;
    }

    /// <summary>
    /// ���[�U�[���擾����
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
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
            this.AchievementID = response.AchievementID;
            this.StageID = response.StageID;
            this.IconID = response.IconID;
            this.TotalScore = response.TotalScore;

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
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public IEnumerator UpdateUser(string name, int achievement_id, int stage_id, int icon_id, Action<ErrorResponse> result)
    {
        // �T�[�o�[�ɑ��M����I�u�W�F�N�g���쐬
        UpdateUserData requestData = new UpdateUserData();
        requestData.UserID = UserID;
        requestData.Name = name;
        requestData.AchievementID = achievement_id;
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
            this.AchievementID = achievement_id;
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
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
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
            // �ʐM�����������ꍇ�A�t�@�C���Ƀ��[�U�[����ۑ�����
            this.UserName = name;
            SaveUserData();
            isSuccess = true;
        }

        // �Ăяo������result�������Ăяo��
        result?.Invoke(isSuccess);
    }

    /// <summary>
    /// �����A�C�e���擾����
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
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
    /// <param name="result"></param>
    /// <returns></returns>
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
    /// <param name="result"></param>
    /// <returns></returns>
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
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
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
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
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
    /// <param name="result"></param>
    /// <returns></returns>
    public IEnumerator GetStageResults(Action<ShowStageResultResponse[]> result)
    {
        // ���M
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/stage/result/show?user_id=" + UserID);

        // ���ʂ���M����܂őҋ@
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success
            && request.responseCode == 200)
        {
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
    /// <returns></returns>
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
    /// <param name="result"></param>
    /// <returns></returns>
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
    /// <param name="result"></param>
    /// <returns></returns>
    public IEnumerator GetFollowRankingList(Action<ShowRankingResponse[]> result)
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
    /// �~��M���̃��O(�z�X�g)�擾����
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
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
    /// <param name="result"></param>
    /// <returns></returns>
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
}