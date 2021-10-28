using System;
using System.Collections;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine;

namespace Assets.Scripts
{
    public class ReinforcementLearningService : MonoBehaviour
    {
        // The Learning Table
        public static string[][] reinforcementLearningResult;

        // Model Settings
        public ModelSettingsScript modelSettingsScript;

        // Singleton Instance of ReinforcementLearningService
        private static ReinforcementLearningService instance = null; 
        public static ReinforcementLearningService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (ReinforcementLearningService)FindObjectOfType(typeof(ReinforcementLearningService));
                }
                return instance;
            }
        }


        /*
         * Keep Game Object when going between Scenes.
         */
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }


        /*
         * Get Rest API.
         */
        public IEnumerator GetReinforcementLearningResult(string learningMethod, string numberGames, string lambda)
        {
            // Set URL with parameters
            string url = Constants.apiBaseURL + learningMethod;
            if ( string.IsNullOrEmpty(numberGames) )
            {
                numberGames = Constants.defaultNumberGames.ToString();
            }
            url += Constants.getRequestParameters + Constants.apiGameParam + Constants.getRequestValueParameter + numberGames;
            if ( learningMethod == Constants.apiUriSARSA )
            {
                if (string.IsNullOrEmpty(lambda) )
                {
                    lambda = Constants.defaultLambdaValue.ToString();
                }
                url += Constants.getRequestAnotherParameter + Constants.apiLambdaParam + Constants.getRequestValueParameter + lambda;
            }
            // Before Request
            modelSettingsScript.BeforeApiCall();
            // Get Request
            UnityWebRequest getRequest = UnityWebRequest.Get(url);
            getRequest.SetRequestHeader("Access-Control-Allow-Credentials", "true");
            getRequest.SetRequestHeader("Access-Control-Allow-Headers", "Accept, Content-Type, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
            getRequest.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, PUT, OPTIONS");
            getRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");
            yield return getRequest.SendWebRequest();
            // Store the result
            reinforcementLearningResult = StringToMatrix(getRequest.downloadHandler.text);
            // After Request
            modelSettingsScript.AfterApiCall(getRequest);
        }


        /*
         * Format the get response to a jagged array (matrix).
         */
        private static string[][] StringToMatrix(string inputString)
        {
            string[][] values =
            inputString.Trim(Constants.getResponseLastCharacter, Constants.getResponseFirstCharacter)
            .Split(new[] { Constants.getResponseMiddleCharacters }, StringSplitOptions.RemoveEmptyEntries)
                   .Select(t => t.Split(Constants.getResponseSplitCharacter)
                        .Select(s => s.Trim(Constants.getResponseTrimCharacter1, Constants.getResponseTrimCharacter2))
                        .ToArray())
                   .ToArray();

            return values;
        }
    }
}
