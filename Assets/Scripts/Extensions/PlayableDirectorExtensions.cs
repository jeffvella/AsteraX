using UnityEngine.Playables;

public static class PlayableDirectorExtensions
{
    public static void Restart(this PlayableDirector director)
    {
        director.time = director.initialTime;
        director.Play();
    }
}