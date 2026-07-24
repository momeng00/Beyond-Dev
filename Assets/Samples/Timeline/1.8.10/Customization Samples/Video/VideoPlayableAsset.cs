using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;

namespace Timeline.Samples
{
    // Editor representation of a Clip to play video in Timeline.
    [Serializable]
    public class VideoPlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        public enum RenderMode
        {
            CameraFarPlane,
            CameraNearPlane,
            RenderTexture
        }

        [Tooltip("The video clip to play.")]
        public VideoClip videoClip;

        [Tooltip("Mutes the audio from the video.")]
        public bool mute;

        [Tooltip("Loops the video.")]
        public bool loop = true;

        [Tooltip("The amount of time before the video begins to start preloading the video stream.")]
        public double preloadTime = 0.3;

        [Tooltip("The aspect ratio of the video to playback.")]
        public VideoAspectRatio aspectRatio = VideoAspectRatio.FitHorizontally;

        [Tooltip("Where the video content will be drawn.")]
        public RenderMode renderMode = RenderMode.CameraFarPlane;

        [Tooltip("Specifies which camera to render to. If unassigned, the main camera will be used.")]
        public ExposedReference<Camera> targetCamera;

        [Tooltip("The Render Texture to output the video to when Render Mode is Render Texture.")]
        public RenderTexture targetTexture;

        [Tooltip("Specifies an optional audio source to output to.")]
        public ExposedReference<AudioSource> audioSource;

        // These are set by the track prior to CreatePlayable being called and are used
        // by the VideoSchedulePlayableBehaviour to schedule preloading of the video clip.
        public double clipInTime { get; set; }
        public double startTime { get; set; }

        // Creates the playable that represents the instance that plays this clip.
        // A hidden VideoPlayer is created for the PlayableBehaviour to control.
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            Camera camera = null;

            if (renderMode != RenderMode.RenderTexture)
            {
                camera = targetCamera.Resolve(graph.GetResolver());

                if (camera == null)
                    camera = Camera.main;
            }

            AudioSource resolvedAudioSource =
                audioSource.Resolve(graph.GetResolver());

            VideoPlayer player = CreateVideoPlayer(
                camera,
                resolvedAudioSource
            );

            // If we are unable to create a player, return a playable
            // with no behaviour attached.
            if (player == null)
                return Playable.Create(graph);

            ScriptPlayable<VideoPlayableBehaviour> playable =
                ScriptPlayable<VideoPlayableBehaviour>.Create(graph);

            VideoPlayableBehaviour playableBehaviour =
                playable.GetBehaviour();

            playableBehaviour.videoPlayer = player;
            playableBehaviour.preloadTime = preloadTime;
            playableBehaviour.clipInTime = clipInTime;
            playableBehaviour.startTime = startTime;

            return playable;
        }

        // The playable asset duration specifies the default duration
        // of the Timeline clip.
        public override double duration
        {
            get
            {
                if (videoClip == null)
                    return base.duration;

                return videoClip.length;
            }
        }

        // Specifies the capabilities of this Timeline clip.
        public ClipCaps clipCaps
        {
            get
            {
                ClipCaps caps =
                    ClipCaps.Blending |
                    ClipCaps.ClipIn |
                    ClipCaps.SpeedMultiplier;

                if (loop)
                    caps |= ClipCaps.Looping;

                return caps;
            }
        }

        private VideoPlayer CreateVideoPlayer(
            Camera camera,
            AudioSource targetAudioSource
        )
        {
            if (videoClip == null)
                return null;

            if (renderMode == RenderMode.RenderTexture &&
                targetTexture == null)
            {
                Debug.LogWarning(
                    $"Video clip '{videoClip.name}' is set to RenderTexture mode, " +
                    "but no Target Texture has been assigned."
                );
            }

            GameObject gameObject = new GameObject(videoClip.name)
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            VideoPlayer videoPlayer =
                gameObject.AddComponent<VideoPlayer>();

            videoPlayer.playOnAwake = false;
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = videoClip;
            videoPlayer.waitForFirstFrame = false;
            videoPlayer.skipOnDrop = true;
            videoPlayer.aspectRatio = aspectRatio;
            videoPlayer.isLooping = loop;

            switch (renderMode)
            {
                case RenderMode.CameraFarPlane:
                    videoPlayer.renderMode =
                        VideoRenderMode.CameraFarPlane;

                    videoPlayer.targetCamera = camera;
                    break;

                case RenderMode.CameraNearPlane:
                    videoPlayer.renderMode =
                        VideoRenderMode.CameraNearPlane;

                    videoPlayer.targetCamera = camera;
                    break;

                case RenderMode.RenderTexture:
                    videoPlayer.renderMode =
                        VideoRenderMode.RenderTexture;

                    videoPlayer.targetTexture = targetTexture;
                    break;

                default:
                    videoPlayer.renderMode =
                        VideoRenderMode.CameraFarPlane;

                    videoPlayer.targetCamera = camera;
                    break;
            }

            videoPlayer.audioOutputMode =
                VideoAudioOutputMode.Direct;

            if (mute)
            {
                videoPlayer.audioOutputMode =
                    VideoAudioOutputMode.None;
            }
            else if (targetAudioSource != null)
            {
                videoPlayer.audioOutputMode =
                    VideoAudioOutputMode.AudioSource;

                for (
                    ushort i = 0;
                    i < videoPlayer.clip.audioTrackCount;
                    ++i
                )
                {
                    videoPlayer.SetTargetAudioSource(
                        i,
                        targetAudioSource
                    );
                }
            }

            return videoPlayer;
        }
    }
}