using UnityEngine;
namespace TP
{

    public class DeepLinkManager : SingletonMonoBehaviour<DeepLinkManager>
    {
        public string deeplinkURL;
        protected override void Awake()
        {
            base.Awake();
            Application.deepLinkActivated += onDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                // Cold start and Application.absoluteURL not null so process Deep Link.
                onDeepLinkActivated(Application.absoluteURL);
            }
            // Initialize DeepLink Manager global variable.
            else deeplinkURL = "[none]";
        }

        private void onDeepLinkActivated(string url)
        {
            // Update DeepLink Manager global variable, so URL can be accessed from anywhere.
            deeplinkURL = url;

            // Decode the URL to determine action. 
            // In this example, the app expects a link formatted like this:
            // unitydl://mylink?scene1
            string sceneName = url.Split("?"[0])[1];
            Debug.Log("RoomCode::"+ sceneName);
        }
    }

}