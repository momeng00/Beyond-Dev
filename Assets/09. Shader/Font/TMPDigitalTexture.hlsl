#ifndef BEYOND_TMP_DIGITAL_TEXTURE_INCLUDED
#define BEYOND_TMP_DIGITAL_TEXTURE_INCLUDED

float BeyondDigitalCoverage(float distanceField, float ssr, float gradientScale, float perimeter, float softness)
{
    float scale = max(ssr * gradientScale, 0.0001);
    float blur = max(softness, 0.0) * scale;
    return saturate(((distanceField - 0.5) * 2.0 * scale + 0.5 + perimeter * scale + blur * 0.5) / (1.0 + blur));
}

float BeyondDigitalNoise(float2 pixel, float tick)
{
    return frac(sin(dot(pixel, float2(127.1, 311.7)) + tick * 74.7) * 43758.5453);
}

void TMPDigitalTexture_float(float3 BaseColor, float BaseAlpha, UnityTexture2D Atlas,
    float2 UV, float2 ScreenUV, float FaceSDF, float SSR, float GradientScale,
    float4 Perimeter, float4 Softness, float4 VertexColor, float4 FaceColor,
    float Strength, float ChromaticPixels, float ScanlineStrength, float ScanlineSpacing,
    float NoiseStrength, float FlickerStrength, float EmissionStrength,
    out float3 Color, out float Alpha, out float3 Emission)
{
    Strength = saturate(Strength);
    float2 pixels = ScreenUV * _ScreenParams.xy;
    float time = _Time.y;
    // UV derivatives express the split in screen pixels, regardless of font atlas size.
    float2 splitUV = ddx(UV) * clamp(ChromaticPixels, 0.0, 3.0) * Strength;
    float redSDF = SAMPLE_TEXTURE2D(Atlas.tex, Atlas.samplerstate, UV - splitUV).a;
    float blueSDF = SAMPLE_TEXTURE2D(Atlas.tex, Atlas.samplerstate, UV + splitUV).a;
    float center = BeyondDigitalCoverage(FaceSDF, SSR, GradientScale, Perimeter.x, Softness.x);
    float red = BeyondDigitalCoverage(redSDF, SSR, GradientScale, Perimeter.x, Softness.x);
    float blue = BeyondDigitalCoverage(blueSDF, SSR, GradientScale, Perimeter.x, Softness.x);
    float opacity = saturate(VertexColor.a * VertexColor.a * FaceColor.a);
    float2 fringe = max(float2(red, blue) - center, 0.0) * opacity * Strength * 0.7;

    // Keep the original face and outline; add narrow red/cyan fringes outside them.
    float3 channelMask = saturate(float3(red, center, blue) / max(center, 0.001));
    // The existing graph amplifies RGB by 4. Normalize that gain so the display
    // pattern remains visible on white letters instead of clipping to solid white.
    float3 original = max(BaseColor, 0.0);
    float3 face = original / max(1.0, max(original.r, max(original.g, original.b)));
    face *= lerp(1.0.xxx, channelMask, Strength * 0.4 * center);
    float alpha = max(saturate(BaseAlpha), max(fringe.x, fringe.y));
    float3 premultiplied = face * saturate(BaseAlpha)
        + fringe.x * float3(1.0, 0.12, 0.3) + fringe.y * float3(0.1, 0.75, 1.0);
    float3 color = premultiplied / max(alpha, 0.0001);

    // A low-contrast scan pattern, grain updated at 12 Hz, and gentle luminance flutter.
    float scan = 0.5 + 0.5 * sin(6.2831853 * (pixels.y / max(ScanlineSpacing, 2.0) - time * 0.12));
    float grain = BeyondDigitalNoise(floor(pixels), floor(time * 12.0));
    float flutter = 0.5 + 0.5 * sin(time * 11.0 + sin(time * 2.3));
    float modulation = (1.0 - saturate(ScanlineStrength) * (1.0 - scan))
        * (1.0 - saturate(NoiseStrength) * grain)
        * (1.0 - saturate(FlickerStrength) * flutter);
    color *= lerp(1.0, modulation, Strength);
    alpha *= 1.0 - Strength * saturate(ScanlineStrength) * (1.0 - scan) * 0.18;

    Color = lerp(BaseColor, color, Strength);
    Alpha = lerp(BaseAlpha, alpha, Strength);
    Emission = max(Color, 0.0) * max(EmissionStrength, 0.0) * Strength;
}
#endif
