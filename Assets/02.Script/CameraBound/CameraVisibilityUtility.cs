using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CameraVisibilityUtility
{
    private sealed class CameraView
    {
        public readonly Plane[] planes = new Plane[6];
        public Matrix4x4 viewMatrix;
        public Matrix4x4 projectionMatrix;
        public Bounds bounds;
        public float depthCenter;
        public bool initialized;
        public bool orthographic;
        public bool standardProjection;
        public bool useBounds;
    }

    private static readonly Dictionary<Camera, CameraView> views = new();
    private static readonly List<Camera> destroyedCameras = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetViews()
    {
        views.Clear();
        destroyedCameras.Clear();
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private static void OnSceneUnloaded(Scene scene)
    {
        // Persistent cameras can recreate their entries on the next request.
        views.Clear();
    }

    public static bool IsInGameCameraView(Bounds bounds, int layer, Camera onlyCamera = null)
    {
        if (onlyCamera == null)
            onlyCamera = Camera.main;

        if (onlyCamera == null || !onlyCamera.isActiveAndEnabled || onlyCamera.cameraType != CameraType.Game)
            return false;

        if ((onlyCamera.cullingMask & (1 << layer)) == 0)
            return false;

        if (!views.TryGetValue(onlyCamera, out CameraView view))
        {
            RemoveDestroyedCameras();
            view = new CameraView();
            views.Add(onlyCamera, view);
        }

        RefreshView(onlyCamera, view);

        if (view.useBounds)
            return view.bounds.Intersects(bounds);

        return GeometryUtility.TestPlanesAABB(view.planes, bounds);
    }

    private static void RefreshView(Camera camera, CameraView view)
    {
        Matrix4x4 viewMatrix = camera.worldToCameraMatrix;
        Matrix4x4 projectionMatrix = camera.projectionMatrix;
        bool orthographic = camera.orthographic;
        bool projectionChanged = !view.initialized ||
                                 !view.projectionMatrix.Equals(projectionMatrix) ||
                                 view.orthographic != orthographic;
        bool viewChanged = !view.initialized || !view.viewMatrix.Equals(viewMatrix);

        // A frame-number cache would miss a later camera move in the same frame.
        if (!projectionChanged && !viewChanged)
            return;

        if (projectionChanged)
        {
            view.standardProjection = false;
            if (orthographic)
                view.standardProjection = TrySetOrthographicSize(camera, projectionMatrix, view);
        }

        view.useBounds = view.standardProjection && IsUnrotatedView(viewMatrix);
        if (view.useBounds)
        {
            // Unity's unrotated view matrix flips Z. Translation changes only the center.
            view.bounds.center = new Vector3(-viewMatrix.m03, -viewMatrix.m13,
                viewMatrix.m23 + view.depthCenter);
        }
        else
        {
            // Rotated, perspective and custom projections retain the general path.
            GeometryUtility.CalculateFrustumPlanes(projectionMatrix * viewMatrix, view.planes);
        }

        view.viewMatrix = viewMatrix;
        view.projectionMatrix = projectionMatrix;
        view.orthographic = orthographic;
        view.initialized = true;
    }

    private static bool TrySetOrthographicSize(Camera camera, Matrix4x4 projection, CameraView view)
    {
        float halfHeight = camera.orthographicSize;
        float halfWidth = halfHeight * camera.aspect;
        float near = camera.nearClipPlane;
        float far = camera.farClipPlane;
        if (halfHeight <= 0f || halfWidth <= 0f || far <= near)
            return false;

        Matrix4x4 standard = Matrix4x4.Ortho(-halfWidth, halfWidth, -halfHeight, halfHeight, near, far);
        // Exact comparison deliberately falls back for even a slightly custom projection.
        if (!projection.Equals(standard))
            return false;

        view.bounds.size = new Vector3(halfWidth * 2f, halfHeight * 2f, far - near);
        view.depthCenter = (near + far) * 0.5f;
        return true;
    }

    private static bool IsUnrotatedView(Matrix4x4 matrix)
    {
        // Translation is unrestricted; rotation, scaling and custom view transforms fall back.
        return matrix.m00 == 1f && matrix.m01 == 0f && matrix.m02 == 0f &&
               matrix.m10 == 0f && matrix.m11 == 1f && matrix.m12 == 0f &&
               matrix.m20 == 0f && matrix.m21 == 0f && matrix.m22 == -1f &&
               matrix.m30 == 0f && matrix.m31 == 0f && matrix.m32 == 0f && matrix.m33 == 1f;
    }

    private static void RemoveDestroyedCameras()
    {
        // Cleanup on camera creation avoids retained entries without scanning every query.
        foreach (Camera camera in views.Keys)
        {
            if (camera == null)
                destroyedCameras.Add(camera);
        }

        for (int i = 0; i < destroyedCameras.Count; i++)
            views.Remove(destroyedCameras[i]);

        destroyedCameras.Clear();
    }
}
