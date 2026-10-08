using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(VideoPlayer))]
public class IntroVideos : MonoBehaviour
{
    public VideoClip[] videos;                // aquí arrastras tus 2 videos
    public string nextScene = "Lanzamiento";  // nombre de la escena del micrófono

    VideoPlayer player;
    int index = 0;

    void Start()
    {
        if (videos.Length == 0)
        {
            SceneManager.LoadScene(nextScene);
            return;
        }

        player = GetComponent<VideoPlayer>();
        player.isLooping = false;
        player.playOnAwake = false;
        player.loopPointReached += OnVideoFinished;

        PlayCurrent();
    }

    void PlayCurrent()
    {
        player.clip = videos[index];
        player.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        index++;

        if (index < videos.Length)
            PlayCurrent();
        else
            SceneManager.LoadScene(nextScene);
    }

    void OnDestroy()
    {
        if (player != null)
            player.loopPointReached -= OnVideoFinished;
    }
}