using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using UnityEngine.UI;

public class WebTool : MonoBehaviour
{
    public string urlForUploading = "http://139.162.14.162:3027/api/fileTrans/upload";

    public string bodyKey = "upload";
    public string mimeType = "image/png";
    public string myUrlPrefix = "http://139.162.14.162:3027";
    JsonData jsonData;

    public bool UploadSuccess { get { return uploadSuccess; } }
    bool uploadSuccess;

    string photoUrl;

    // An event which will be triggered when the uploading request is done
    public delegate void UploadPhotoReqFinish(bool success, string photoUrl);
    public static event UploadPhotoReqFinish OnUploadPhotoReqFinish;

    void Start()
    {

    }

    public void UploadPhoto(Texture2D _textureToUpload, string _fileName)
    {
        StartCoroutine(E_UploadPhoto(_textureToUpload, _fileName));
    }

    IEnumerator E_UploadPhoto(Texture2D _textureToUpload, string _fileName)
    {
        byte[] bytes = _textureToUpload.EncodeToPNG();

        WWWForm form = new WWWForm();
        form.AddBinaryData(bodyKey, bytes, _fileName, mimeType);

        using (UnityWebRequest req = UnityWebRequest.Post(urlForUploading, form))
        {
            yield return req.SendWebRequest();

            // Status Code
            Debug.Log("WebRequest Status Code: " + req.responseCode);

            if (req.isNetworkError || req.isHttpError)
            {
                uploadSuccess = false;
                Debug.LogError(req.error);
            }
            else
            {
                Debug.Log("Uploaded!");

                // Get the downloaded json string
                string downloadedString = req.downloadHandler.text;
                Debug.Log(downloadedString);

                // Parse the downloaded json string and get the url
                string parsedUrl = GetPhotoUrl(downloadedString);
                if (parsedUrl != null)
                {
                    uploadSuccess = true;

                    //photoUrl = myUrlPrefix + parsedUrl;
                    photoUrl = parsedUrl;
                    Debug.Log(photoUrl);
                }
                else
                {
                    uploadSuccess = false;
                    Debug.LogError("Something's wrong when parsing the json and get the url.");
                }
            }

            if (OnUploadPhotoReqFinish != null)
                OnUploadPhotoReqFinish(uploadSuccess, photoUrl);
        }
    }

    string GetPhotoUrl(string _jsonString)
    {
        jsonData = JsonMapper.ToObject(_jsonString);

        if (jsonData["url"] != null)
        {
            return jsonData["url"].ToString();
        }
        else
        {
            Debug.LogError("jsonData[url] is null!");
        }

        return null;
    }
}
