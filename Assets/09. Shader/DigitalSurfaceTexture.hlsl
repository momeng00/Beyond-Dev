#ifndef BEYOND_DIGITAL_SURFACE_TEXTURE_INCLUDED
#define BEYOND_DIGITAL_SURFACE_TEXTURE_INCLUDED

float BeyondSurfaceNoise(float2 pixel, float tick)
{
    return frac(sin(dot(pixel, float2(127.1, 311.7)) + tick * 74.7) * 43758.5453);
}

void DigitalSurfaceTexture_float(float3 BaseColor, float BaseAlpha, float2 ScreenUV,
    float Strength, float ChromaticPixels, float ScanlineStrength, float ScanlineSpacing,
    float NoiseStrength, float FlickerStrength, out float3 Color, out float Alpha)
{
    Strength = saturate(Strength);
    float opacity = saturate(BaseAlpha);
    float3 original = max(BaseColor, 0.0);
    float3 face = original / max(1.0, max(original.r, max(original.g, original.b)));
    float split = clamp(ChromaticPixels, 0.0, 3.0) * Strength;

    // Differentiate the final composite, so procedural masks, hover/reveal effects,
    // and sprite tint all take part in the split. This is a small-offset approximation
    // in screen pixels, without re-evaluating the original graph three times.
    float3 premultiplied = face * opacity;
    float3 slope = ddx(premultiplied);
    float alphaSlope = ddx(opacity);
    float redAlpha = saturate(opacity - alphaSlope * split);
    float blueAlpha = saturate(opacity + alphaSlope * split);
    float2 fringe = max(float2(redAlpha, blueAlpha) - opacity, 0.0) * 0.7;
    float3 shifted = max(float3(premultiplied.r - slope.r * split,
        premultiplied.g, premultiplied.b + slope.b * split), 0.0);
    shifted += fringe.x * float3(1.0, 0.12, 0.3) + fringe.y * float3(0.1, 0.75, 1.0);
    float alpha = max(opacity, max(fringe.x, fringe.y));
    float3 color = min(shifted / max(alpha, 0.0001), 1.0.xxx);

    float2 pixels = ScreenUV * _ScreenParams.xy;
    float time = _Time.y;
    float scan = 0.5 + 0.5 * sin(6.2831853 * (pixels.y / max(ScanlineSpacing, 2.0) - time * 0.12));
    float grain = BeyondSurfaceNoise(floor(pixels), floor(time * 12.0));
    float flutter = 0.5 + 0.5 * sin(time * 11.0 + sin(time * 2.3));
    float modulation = (1.0 - saturate(ScanlineStrength) * (1.0 - scan))
        * (1.0 - saturate(NoiseStrength) * grain)
        * (1.0 - saturate(FlickerStrength) * flutter);
    color *= lerp(1.0, modulation, Strength);
    alpha *= 1.0 - Strength * saturate(ScanlineStrength) * (1.0 - scan) * 0.18;

    Color = lerp(BaseColor, color, Strength);
    Alpha = lerp(BaseAlpha, alpha, Strength);
}
#endif
