Shader "Custom/FocusMaskShader"
{
    Properties
    {
        _OverlayColor ("Overlay Color", Color) = (0,0,0,0.9)
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _Center ("Center", Vector) = (0.5,0.5,0,0)
        _Scale ("Scale", Float) = 0.5
        _Cutoff ("Cutoff", Range(0,1)) = 0.5
        _Softness ("Softness", Range(0.001,0.5)) = 0.05
        _DebugMode ("Debug Mode", Float) = 0
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _BlitTexture;
            sampler2D _MaskTex;

            float4 _OverlayColor;
            float4 _MaskTex_TexelSize;
            float4 _ScreenParams;
            float4 _Center;
            float _Scale;
            float _Cutoff;
            float _Softness;
            float _DebugMode;

            Varyings Vert(Attributes input)
            {
                Varyings output;

                float2 uv = float2(
                    (input.vertexID << 1) & 2,
                    input.vertexID & 2
                );

                output.uv = uv;
                output.uv.y = 1.0 - output.uv.y;

                output.positionCS = float4(uv * 2.0 - 1.0, 0.0, 1.0);

                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;

                half4 sceneColor = tex2D(_BlitTexture, uv);

                float2 p = uv - _Center.xy;
                p.x *= _ScreenParams.x / _ScreenParams.y;

                float maskAspect = max(_MaskTex_TexelSize.z, 1.0) / max(_MaskTex_TexelSize.w, 1.0);

                float maskHeight = max(_Scale, 0.0001);
                float maskWidth = maskHeight * maskAspect;

                float2 maskUV = p / float2(maskWidth, maskHeight) + 0.5;

                float inside =
                    step(0.0, maskUV.x) * step(maskUV.x, 1.0) *
                    step(0.0, maskUV.y) * step(maskUV.y, 1.0);

                float maskAlpha = tex2D(_MaskTex, maskUV).a * inside;

                if (_DebugMode > 0.5)
                    return half4(maskAlpha, maskAlpha, maskAlpha, 1);

                float hole = smoothstep(
                    _Cutoff - _Softness,
                    _Cutoff + _Softness,
                    maskAlpha
                );

                float overlayAlpha = _OverlayColor.a * (1.0 - hole);
                float3 finalColor = lerp(sceneColor.rgb, _OverlayColor.rgb, overlayAlpha);

                return half4(finalColor, sceneColor.a);
            }
            ENDHLSL
        }
    }
}