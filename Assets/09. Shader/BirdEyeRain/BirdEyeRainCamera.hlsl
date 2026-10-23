#ifndef BEYOND_BIRD_EYE_RAIN_CAMERA_INCLUDED
#define BEYOND_BIRD_EYE_RAIN_CAMERA_INCLUDED

// Updated for each camera by BirdEyeRainCameraHistory; no scene component is needed.
float4 _BirdEyeRainCameraHistory[128];
float4 _BirdEyeRainCameraNow;
float4 _BirdEyeRainCameraHistoryInfo; // x = valid, y = sampling interval

float3 BirdEyeRainBirthCamera(float age)
{
    float birthTime = _BirdEyeRainCameraNow.w - age;
    float sampleIndex = clamp((birthTime - _BirdEyeRainCameraHistory[0].w)
        / max(_BirdEyeRainCameraHistoryInfo.y, 0.0001), 0.0, 127.0);
    int lo = (int)floor(sampleIndex);
    int hi = min(lo + 1, 127);
    float3 birth = lerp(_BirdEyeRainCameraHistory[lo].xyz,
        _BirdEyeRainCameraHistory[hi].xyz, frac(sampleIndex));
    float4 newest = _BirdEyeRainCameraHistory[127];
    if (birthTime >= newest.w)
    {
        float blend = saturate((birthTime - newest.w) / max(_BirdEyeRainCameraNow.w - newest.w, 0.0001));
        birth = lerp(newest.xyz, _BirdEyeRainCameraNow.xyz, blend);
    }
    return birth;
}

float2 BirdEyeRainCameraOffset(float age, float perspective, float strength, float distance, float aspect)
{
    if (_BirdEyeRainCameraHistoryInfo.x < 0.5 || strength <= 0.0)
        return 0.0;
    float3 delta = _BirdEyeRainCameraNow.xyz - BirdEyeRainBirthCamera(age);
    float2 viewDelta = mul((float3x3)UNITY_MATRIX_V, delta).xy;
    float projectionDepth = lerp(max(distance, 0.01), 1.0, unity_OrthoParams.w);
    float2 projectionScale = float2(UNITY_MATRIX_P._m00 * aspect, UNITY_MATRIX_P._m11 * _ProjectionParams.x) * 0.5;
    return viewDelta * projectionScale * perspective * max(strength, 0.0) / projectionDepth;
}

void BirdEyeRandomRain_float(float2 ScreenUV, float Time, float OriginalAlpha,
    float Randomness, float Seed, float2 Focus, float Speed, float StreakLength,
    float CameraParallax, float ParallaxDistance, out float Mask)
{
    // Screen-space perspective: the head moves inward; its tail points back toward the camera.
    float aspect = _ScreenParams.x / max(_ScreenParams.y, 1.0);
    float2 p = (ScreenUV - Focus) * float2(aspect, 1.0);
    float radius = length(p);
    float angle = atan2(p.y, p.x);
    float rain = 0.0;
    const float TAU = 6.28318530718;
    const float lanes = 64.0;
    float pixelWidth = 1.0 / max(_ScreenParams.y, 1.0);
    [unroll] for (int layer = 0; layer < 2; layer++)
    {
        float rotation = layer * 0.381966;
        float sector = (angle / TAU + 0.5 + rotation) * lanes;
        float baseLane = floor(sector);
        [unroll] for (int neighbor = -5; neighbor <= 5; neighbor++)
        {
            float lane = baseLane + neighbor;
            float wrappedLane = lane - floor(lane / lanes) * lanes;
            float3 h = frac(float3(wrappedLane, layer + 1, wrappedLane + layer * 17.0) * 0.1031 + Seed * 0.0137);
            h += dot(h, h.yzx + 33.33);
            float3 clockHash = frac((h.xxy + h.yzz) * h.zyx);
            float clock = Time * max(Speed, 0.0) * lerp(0.35, 0.65, clockHash.x) + clockHash.y;
            float cycle = floor(clock);
            float phase = frac(clock);
            float3 e = frac(float3(wrappedLane, cycle, layer + 1) * float3(0.1031, 0.1030, 0.0973) + Seed * 0.0191);
            e += dot(e, e.yxz + 33.33);
            float3 eventHash = frac((e.xxy + e.yzz) * e.zyx);
            float theta = ((lane + 0.2 + 0.6 * eventHash.x) / lanes - 0.5 - rotation) * TAU;
            float2 direction = float2(cos(theta), sin(theta));
            // Start beyond this viewport's farthest corner, including off-center focus.
            float2 corner = max(abs(Focus), abs(1.0 - Focus)) * float2(aspect, 1.0);
            float spawnRadius = length(corner) * lerp(1.05, 1.35, eventHash.y);
            // Perspective projection: distance, width and length all shrink with depth.
            float perspective = rcp(1.0 + phase * lerp(9.0, 14.0, clockHash.z));
            float headRadius = spawnRadius * perspective;
            float width = lerp(0.0018, 0.0036, eventHash.z) * perspective;
            float streak = lerp(0.045, 0.12, clockHash.z) * perspective * max(StreakLength, 0.0);
            // The same event keeps its birth camera position even after the camera stops.
            float age = phase / max(max(Speed, 0.0) * lerp(0.35, 0.65, clockHash.x), 0.0001);
            float2 cameraOffset = BirdEyeRainCameraOffset(age, perspective, CameraParallax, ParallaxDistance, aspect);
            // Bound fast pans so the neighboring lane search cannot clip live streaks.
            float maxOffset = spawnRadius * 0.08;
            cameraOffset *= min(1.0, maxOffset / max(length(cameraOffset), 0.00001));
            float2 relative = p + cameraOffset - direction * headRadius;
            float along = dot(relative, direction);
            float2 closest = relative - direction * clamp(along, 0.0, streak);
            float distanceToStreak = length(closest);
            // Pixel filtering keeps distant drops from sparkling or disappearing abruptly.
            float aa = pixelWidth * 0.75;
            float drop = 1.0 - smoothstep(max(0.0, width - aa), width + aa, distanceToStreak);
            drop *= saturate(width / aa);
            drop *= smoothstep(0.0, 0.025, phase) * (1.0 - smoothstep(0.07, 0.22, phase));
            rain += drop * lerp(0.65, 1.15, eventHash.y);
        }
    }
    // Let the motion remain visible before fading near the focal point.
    rain *= smoothstep(0.025, 0.18, radius);
    Mask = lerp(OriginalAlpha, saturate(rain), saturate(Randomness));
}

#endif
