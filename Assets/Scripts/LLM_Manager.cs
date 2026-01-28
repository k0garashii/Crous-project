using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class LLM_Manager : MonoBehaviour
{
    //public PlayerController playerController;
    public BuilderAgent builderAgent;
    public string prompt;

    private const string OLLAMA_URL = "http://localhost:11434/api/generate";
    private const string COMPREHENSION_MODEL = "llama3";

    private const string EMBEDDING_URL = "http://localhost:11434/api/embeddings";
    private const string EMBEDDING_MODEL = "nomic-embed-text";

    private string systemPrompt =
        "Tu es un interpréteur de commandes pour un jeu vidéo de simulation urbaine. "
        + "Ton rôle est de convertir les ordres du joueur en un objet JSON structuré, sans phrase conversationnelle. "
        + "Format attendu : {\"intention\": string, \"entites\": [{\"type\": string, \"valeur\": string}]}.\n\n"
        + "Les intentions possibles : CONSTRUIRE, AMELIORER, ASSIGNER, DEFINIR_REGLE.\n"
        + "Exemple : \"Construis une scierie dans la forêt.\" -> "
        + "{\"intention\":\"CONSTRUIRE\",\"entites\":[{\"type\":\"BATIMENT\",\"valeur\":\"scierie\"},{\"type\":\"ZONE\",\"valeur\":\"forêt\"}]}.\n\n"
        + "Si la commande est ambiguë : renvoie {\"intention\": \"INCONNU\", \"entites\": []}."
        + "Input de l'utilisateur : \n\n";

    private void Start()
    {
        SendPrompt(prompt);
    }

    [System.Serializable]
    private class RequestBody
    {
        public string model;
        public string prompt;
        public bool stream;
    }

    [System.Serializable]
    public class OllamaStringResponse
    {
        public string response;
    }

    [System.Serializable]
    private class EmbeddingResponse
    {
        public float[] embedding;
    }

    [System.Serializable]
    public class LLMResponseEntity
    {
        public string type;
        public string valeur;
    }

    [System.Serializable]
    public class LLMResponse
    {
        public string intention;
        public LLMResponseEntity[] entites;
    }

    public void SendPrompt(string userPrompt)
    {
        StartCoroutine(SendPromptCoroutine(userPrompt));
    }

    private IEnumerator SendPromptCoroutine(string userPrompt)
    {
        RequestBody body = new RequestBody
        {
            model = COMPREHENSION_MODEL,
            prompt = systemPrompt + userPrompt,
            stream = false
        };

        string jsonBody = JsonUtility.ToJson(body);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        Debug.Log("Envoi de la requête à l'URL : " + OLLAMA_URL);
        Debug.Log("Avec le corps JSON : " + jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(OLLAMA_URL, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string rawJsonResponse = request.downloadHandler.text;

                OllamaStringResponse fullResponse = JsonUtility.FromJson<OllamaStringResponse>(rawJsonResponse);
                Debug.Log("Réponse parsée : " + fullResponse.response);

                ParseAndExecuteCommand(fullResponse.response);
            }
            else
            {
                Debug.LogError("Erreur Ollama : " + request.error);
                Debug.LogError("Réponse du serveur (si disponible): " + request.downloadHandler.text);
            }
        }
    }
    public static async Task<float[]> GetEmbedding(string text)
    {
        RequestBody body = new RequestBody
        {
            model = EMBEDDING_MODEL,
            prompt = text
        };

        string jsonBody = JsonUtility.ToJson(body);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(EMBEDDING_URL, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                EmbeddingResponse response = JsonUtility.FromJson<EmbeddingResponse>(jsonResponse);
                return response.embedding;
            }
            else
            {
                Debug.LogError("Erreur d'embedding Ollama : " + request.error);
                Debug.LogError("Réponse du serveur : " + request.downloadHandler.text);
                return null;
            }
        }
    }
    public void ParseAndExecuteCommand(string jsonResponse)
    {
        try
        {
            LLMResponse command = JsonUtility.FromJson<LLMResponse>(jsonResponse);
            Debug.Log("command.intention : " + command.intention);

            switch (command.intention)
            {
                case "CONSTRUIRE":
                    Debug.Log($"ACTION: Construire un {command.entites[0].valeur} dans la zone {command.entites[1].valeur}");
                    //Fonction : est ce que j'ai assez de matériaux ? Est ce que la zone est libre ?
                    builderAgent.SetBuildCell(4, 4); // Exemple d'action
                    break;

                case "ASSIGNER":
                    Debug.Log($"ACTION: Assigner le PNJ {command.entites[0].valeur} au bâtiment {command.entites[1].valeur}");
                    break;

                case "DEFINIR_REGLE":
                    Debug.Log($"ACTION: Définir une règle pour {command.entites[0].valeur} à une quantité de {command.entites[1].valeur}");
                    break;

                case "INCONNU":
                    Debug.LogWarning("Intention inconnue ou commande ambiguë.");
                    break;

                default:
                    Debug.LogError("Intention non gérée: " + command.intention);
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur de parsing du JSON de la commande: " + e.Message);
        }
    }
}
