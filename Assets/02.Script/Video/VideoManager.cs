
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager
{
    public static VideoManager Instance 
    {
        get
        {
            if (instance == null)
            {
                instance = new VideoManager();
                instance.cacheVideo = VideoDataSheet.VideoList;
            }
            return instance;
        }
    }
    
    private static VideoManager instance;
    private Dictionary<VideoData, string> cacheVideo = new Dictionary<VideoData, string>();

    public VideoClip Prepare(VideoData name)
    {
        VideoClip clip = Resources.Load<VideoClip>(cacheVideo[name]);
        return clip;
    }

}
