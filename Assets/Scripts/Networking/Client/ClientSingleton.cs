using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{

    public ClientGameManager ClientGameManager { get; private set; }
    private static ClientSingleton instance;

    public static ClientSingleton Instance
    {
        get
        {
            if (instance != null) { return instance; }
            instance = FindObjectOfType<ClientSingleton>();
            if (instance == null)
            {
                Debug.LogError("No ClientSingleton in scene!");
            }
            return instance;
        }

    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task<bool> CreateClient()
    {
        ClientGameManager = new ClientGameManager();
        return await ClientGameManager.InitAsync();
    }

    private void OnDestroy()
    {
        ClientGameManager.Dispose();
    }

}
