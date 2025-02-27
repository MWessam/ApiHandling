using ApiHandling.Generated.Facade;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

public class ApiTest : MonoBehaviour
{
    [Inject] private ApiFacade _apiFacade;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    private async UniTask RunTests()
    {
        var result = await _apiFacade.GetUser("16").Fetch();
        if (result)
        {
            Debug.Log("User fetched successfully");
            Debug.Log("User name: " + result.Value.Name);
            Debug.Log("User handle: " + result.Value.UserHandle);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            RunTests().Forget();
        }
    }
}
