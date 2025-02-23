using System;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using CancellationToken = System.Threading.CancellationToken;

public class ApiRequest : MonoBehaviour
{
    [SerializeField] private ApiConfigSO _apiConfig;
    private int _timeoutMs = 5000;

    private string Url(string path) => $"{_apiConfig.ApiUrl}/{path}";

private async UniTask<Result<string>> SendRequest(UnityWebRequest request, CancellationToken cancellationToken = default)
    {
        using (request)
        {
            try
            {
                await request.SendWebRequest().ToUniTask(cancellationToken: cancellationToken)
                    .Timeout(TimeSpan.FromMilliseconds(_timeoutMs));
            }
            catch (OperationCanceledException)
            {
                return Result<string>.Failure(EResultError.Timeout, "Request cancelled or timed out.");
            }
            catch (TimeoutException)
            {
                return Result<string>.Failure(EResultError.Timeout, "Request timed out.");
            }

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                return Result<string>.Failure(EResultError.ProtocolError, request.error);
            }
            return Result<string>.Success(request.downloadHandler.text);
        }
    }

    public async UniTask<Result<string>> GetRequest(string path, CancellationToken cancellationToken = default)
    {
        var request = UnityWebRequest.Get(Url(path));
        return await SendRequest(request, cancellationToken);
    }

    public async UniTask<Result<string>> PostRequest(string path, string requestBody, CancellationToken cancellationToken = default)
    {
        var request = new UnityWebRequest(Url(path), "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(requestBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        return await SendRequest(request, cancellationToken);
    }

    public async UniTask<Result<string>> PatchRequest(string path, string jsonBody, CancellationToken cancellationToken = default)
    {
        var request = new UnityWebRequest(Url(path), "PATCH");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        return await SendRequest(request, cancellationToken);
    }

    public async UniTask<Result<string>> DeleteRequest(string path, CancellationToken cancellationToken = default)
    {
        var request = UnityWebRequest.Delete(Url(path));
        return await SendRequest(request, cancellationToken);
    }

    public async UniTask<Result<string>> PostRequestForm(string path, CancellationToken cancellationToken = default, params FormItem[] formItems)
    {
        WWWForm form = new WWWForm();
        foreach (var item in formItems)
        {
            switch (item.Type)
            {
                case EFormItemType.StringValue:
                    form.AddField(item.Key, (string)item.Value);
                    break;
                case EFormItemType.ByteArray:
                    form.AddBinaryData(item.Key, (byte[])item.Value);
                    break;
                default:
                    return Result<string>.Failure(EResultError.InvalidInput, $"{item.Type} is not supported!");
            }
        }

        var request = UnityWebRequest.Post(Url(path), form);
        return await SendRequest(request, cancellationToken);
    }

    public async Task<Result<string>> PatchRequestForm(string path, CancellationToken cancellationToken = default, params FormItem[] formItems)
    {
        WWWForm form = new WWWForm();
        foreach (var item in formItems)
        {
            switch (item.Type)
            {
                case EFormItemType.StringValue:
                    form.AddField(item.Key, (string)item.Value);
                    break;
                case EFormItemType.ByteArray:
                    form.AddBinaryData(item.Key, (byte[])item.Value);
                    break;
                default:
                    Debug.LogError($"{item.Type} is not supported!");
                    return Result<string>.Failure(EResultError.InvalidInput, $"{item.Type} is not supported!");
            }
        }

        var request = new UnityWebRequest(Url(path), "PATCH");
        request.uploadHandler = new UploadHandlerRaw(form.data);
        request.downloadHandler = new DownloadHandlerBuffer();

        foreach (var header in form.headers)
        {
            request.SetRequestHeader(header.Key, header.Value);
        }

        return await SendRequest(request, cancellationToken);
    }
}

internal class ApiConfigSO
{
    public string ApiUrl { get; set; }
}

public class FormItem
{
    public string Key;
    public object Value;
    public EFormItemType Type;

    public FormItem(string key, object data, EFormItemType type)
    {
        Key = key;
        Value = data;
        Type = type;
    }
}

public enum EFormItemType
{
    StringValue,
    ByteArray,
}