#ifndef GAUSSIAN_BLUR_1D_INCLUDED
#define GAUSSIAN_BLUR_1D_INCLUDED

// Shader Graph Custom Function (File mode).
// Source is a UnityTexture2D so the texture and sampler state travel together.
// TexelSize must be the texel size of the texture currently being blurred.

void GaussianBlur1D_float(
    UnityTexture2D Source,
    float2 UV,
    float2 TexelSize,
    float2 Direction,
    float Radius,
    out float4 Out
)
{
    float directionLengthSq = dot(Direction, Direction);
    float2 dir = directionLengthSq > 1e-8
        ? Direction * rsqrt(directionLengthSq)
        : float2(1.0, 0.0);

    float2 offset = TexelSize * dir * max(Radius, 0.0);

    // 9-tap Gaussian weights.
    const float w0 = 0.2270270270;
    const float w1 = 0.1945945946;
    const float w2 = 0.1216216216;
    const float w3 = 0.0540540541;
    const float w4 = 0.0162162162;

    float4 color = SAMPLE_TEXTURE2D(Source.tex, Source.samplerstate, UV) * w0;

    color += SAMPLE_TEXTURE2D(Source.tex, Source.samplerstate, UV + offset * 1.0) * w1;
    color += SAMPLE_TEXTURE2D(Source.tex, Source.samplerstate, UV - offset * 1.0) * w1;

    color += SAMPLE_TEXTURE2D(Source.tex, Source.samplerstate, UV + offset * 2.0) * w2;
    color += SAMPLE_TEXTURE2D(Source.tex, Source.samplerstate, UV - offset * 2.0) * w2;

    color += SAMPLE_TEXTURE2D(Source.tex, Source.samplerstate, UV + offset * 3.0) * w3;
    color += SAMPLE_TEXTURE2D(Source.tex, Source.samplerstate, UV - offset * 3.0) * w3;

    color += SAMPLE_TEXTURE2D(Source.tex, Source.samplerstate, UV + offset * 4.0) * w4;
    color += SAMPLE_TEXTURE2D(Source.tex, Source.samplerstate, UV - offset * 4.0) * w4;

    Out = color;
}

void GaussianBlur1D_half(
    UnityTexture2D Source,
    half2 UV,
    half2 TexelSize,
    half2 Direction,
    half Radius,
    out half4 Out
)
{
    half directionLengthSq = dot(Direction, Direction);
    half2 dir = directionLengthSq > 1e-4h
        ? Direction * rsqrt(directionLengthSq)
        : half2(1.0h, 0.0h);

    half2 offset = TexelSize * dir * max(Radius, 0.0h);

    const half w0 = 0.2270270270h;
    const half w1 = 0.1945945946h;
    const half w2 = 0.1216216216h;
    const half w3 = 0.0540540541h;
    const half w4 = 0.0162162162h;

    half4 color = SAMPLE_TEXTURE2D(Source.tex, Source.samplerstate, UV) * w0;

    color += SAMPLE_TEXTURE2D(Source.tex, Source.samplerstate, UV + offset * 1.0h) * w1;
    color += SAMPLE_TEXTURE2D(Source.tex, Source.samplerstate, UV - offset * 1.0h) * w1;

    color += SAMPLE_TEXTURE2D(Source.tex, Source.samplerstate, UV + offset * 2.0h) * w2;
    color += SAMPLE_TEXTURE2D(Source.tex, Source.samplerstate, UV - offset * 2.0h) * w2;

    color += SAMPLE_TEXTURE2D(Source.tex, Source.samplerstate, UV + offset * 3.0h) * w3;
    color += SAMPLE_TEXTURE2D(Source.tex, Source.samplerstate, UV - offset * 3.0h) * w3;

    color += SAMPLE_TEXTURE2D(Source.tex, Source.samplerstate, UV + offset * 4.0h) * w4;
    color += SAMPLE_TEXTURE2D(Source.tex, Source.samplerstate, UV - offset * 4.0h) * w4;

    Out = color;
}

#endif
