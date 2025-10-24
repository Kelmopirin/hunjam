using UnityEngine;
using UnityEngine.Video; // ✅ Needed for VideoPlayer

public class VidPlayer : MonoBehaviour
{
    [SerializeField] string videoFileName;

    void Start()
    {
        PlayVideo();
    }

    public void PlayVideo()
    {
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer != null)
        {
            // Combine the path to StreamingAssets
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            Debug.Log(videoPath);

            videoPlayer.url = videoPath;
            videoPlayer.Play(); // ✅ Capital P
        }
        else
        {
            Debug.LogError("No VideoPlayer component found on this GameObject.");
        }
    }
}
