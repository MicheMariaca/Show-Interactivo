using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(VideoPlayer))]
public class IntroVideos : MonoBehaviour
{
    public VideoClip[] videos;
    public string nextScene = "Lanzamiento";

    VideoPlayer player;
    int index = 0;
    bool cambiandoEscena = false;

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

    void Update()
    {
        if (SpacePressed())
            Saltar();
    }

    bool SpacePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Space);
#endif
    }

    void Saltar()
    {
        if (cambiandoEscena) return;
        player.Stop();
        OnVideoFinished(player);
    }

    void PlayCurrent()
    {
        player.clip = videos[index];
        player.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (cambiandoEscena) return;

        index++;

        if (index < videos.Length)
        {
            PlayCurrent();
        }
        else
        {
            cambiandoEscena = true;
            SceneManager.LoadScene(nextScene);
        }
    }

    void OnDestroy()
    {
        if (player != null)
            player.loopPointReached -= OnVideoFinished;
    }
}