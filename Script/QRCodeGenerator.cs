using System.Collections;
using UnityEngine;
using ZXing;
using ZXing.QrCode;
using System;

public class QRCodeGenerator : MonoBehaviour {

    public int textureSize = 256;

    public delegate void QRCodeGenerated(Texture2D _texture);
    public static event QRCodeGenerated OnQRCodeGenerated;

	void Start ()
    {

    }

    public void GenerateQRCode(string _textToEncode)
    {
        StartCoroutine(E_GenerateQRCode(_textToEncode));
    }

    // TODO Fix the resolution problem
    IEnumerator E_GenerateQRCode(string _textToEncode)
    {
        Texture2D texture_QRCode = new Texture2D(textureSize, textureSize);

        texture_QRCode.SetPixels32(Encode(_textToEncode));

        yield return null;

        texture_QRCode.Apply();

        if (OnQRCodeGenerated != null)
            OnQRCodeGenerated(texture_QRCode);

    }

    Color32[] Encode(string _textToEncode)
    {
        BarcodeWriter barcodeWriter = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = textureSize,
                Width = textureSize
            }
        };

        return barcodeWriter.Write(_textToEncode);
    }

    /*
    public void SaveQRCodeFile(Texture2D _qrCodeTex)
    {

        try
        {
            string fileName = gameManager.PhotoFileName + "_QR.png";
            string fullPath = gameManager.qrCodeSavePath + "\\" + fileName;

            byte[] bytes = _qrCodeTex.EncodeToPNG();

            System.IO.File.WriteAllBytes(fullPath, bytes);

            //Debug.Log("QRCode File Saved: " + fullPath);
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
        }
       
    }
    */
}
