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
    /// 更新可能ファイルがあるか確認
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
            // 更新がある場合はロード処理
            StartCoroutine(Loading());
        }
        else
        {
            // 更新がない場合はユーザー登録処理
            m_topManager.StoreUser();
        }
    }

    /// <summary>
    /// 読み込み処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator Loading()
    {
        m_loadingSlider.SetActive(true);

        // カタログ更新処理
        var handle = Addressables.UpdateCatalogs();
        yield return handle;

        // bundleダウンロード実行
        AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync("default", false); // ラベル指定
        // ダウンロード完了するまでスライダーを更新する
        while (downloadHandle.Status == AsyncOperationStatus.None)
        {
            m_loadingSlider.GetComponent<Slider>().value = downloadHandle.GetDownloadStatus().Percent * 100;   // スライダー限界値が0~1になっている
            yield return null;  // 1フレーム待機
        }
        m_loadingSlider.GetComponent<Slider>().value = 100;
        Addressables.Release(downloadHandle);

        m_loadingSlider.SetActive(false);

        // ユーザー登録処理
        m_topManager.StoreUser();
    }
}
