using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AssetDownLoader : MonoBehaviour
{
    [SerializeField] TopManager m_topManager;
    [SerializeField] GameObject m_loadingSlider;

    /// <summary>
    /// �X�V�\�t�@�C�������邩�m�F
    /// </summary>
    /// <returns></returns>
    public IEnumerator checkCatalog()
    {
        var checkHandle = Addressables.CheckForCatalogUpdates(false);
        yield return checkHandle;

        var updates = checkHandle.Result;
        Addressables.Release(checkHandle);
        if(updates.Count >= 1)
        {
            // �X�V������ꍇ�̓��[�h����
            StartCoroutine(Loading());
        }
        else
        {
            // �X�V���Ȃ��ꍇ�̓��[�U�[�o�^����
            m_topManager.StoreUser();
        }
    }

    /// <summary>
    /// �ǂݍ��ݏ���
    /// </summary>
    /// <returns></returns>
    public IEnumerator Loading()
    {
        m_loadingSlider.SetActive(true);

        // �J�^���O�X�V����
        var handle = Addressables.UpdateCatalogs();
        yield return handle;

        // bundle�_�E�����[�h���s
        AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync("default", false); // ���x���w��
        // �_�E�����[�h��������܂ŃX���C�_�[���X�V����
        while (downloadHandle.Status == AsyncOperationStatus.None)
        {
            m_loadingSlider.GetComponent<Slider>().value = downloadHandle.GetDownloadStatus().Percent * 100;   // �X���C�_�[���E�l��0~1�ɂȂ��Ă���
            yield return null;  // 1�t���[���ҋ@
        }
        m_loadingSlider.GetComponent<Slider>().value = 100;
        Addressables.Release(downloadHandle);

        m_loadingSlider.SetActive(false);

        // ���[�U�[�o�^����
        m_topManager.StoreUser();
    }
}
