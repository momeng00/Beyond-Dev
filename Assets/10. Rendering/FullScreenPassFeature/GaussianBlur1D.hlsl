#ifndef GAUSSIAN_BLUR_1D_INCLUDED
#define GAUSSIAN_BLUR_1D_INCLUDED

// Shader Graph Custom Function (File mode) 용
// Texture2D / SamplerState 는 Custom Function의 Texture2D 입력에서 같이 전달됨

void GaussianBlur1D_float(
    UnityTexture2D Source,
    float2 UV,
    float2 TexelSize,
    float2 Direction,
    float Radius,
    out float4 Out
)
{
    float2 dir = normalize(Direction);
    float2 offset = TexelSize * dir * Radius;

    // 9-tap Gaussian weights
    float w0 = 0.2270270270;
    float w1 = 0.1945945946;
    float w2 = 0.1216216216;
    float w3 = 0.0540540541;
    float w4 = 0.0162162162;

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
    half2 dir = normalize(Direction);
    half2 offset = TexelSize * dir * Radius;

    half w0 = 0.2270270270h;
    half w1 = 0.1945945946h;
    half w2 = 0.1216216216h;
    half w3 = 0.0540540541h;
    half w4 = 0.0162162162h;

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