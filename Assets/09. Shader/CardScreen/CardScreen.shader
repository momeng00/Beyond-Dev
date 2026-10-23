Shader "Beyond/UI/Card Screen Additive"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Digital Screen)]
        _DigitalStrength ("Digital Strength", Range(0,1)) = 1
        _DigitalScanlineStrength ("Scanline Strength", Range(0,0.6)) = 0.22
        _DigitalScanlineSpacing ("Scanline Spacing (Pixels)", Range(2,12)) = 3.5
        _DigitalNoiseStrength ("Noise Strength", Range(0,0.3)) = 0.045
        _DigitalFlickerStrength ("Flicker Strength", Range(0,0.2)) = 0.035
        _PixelMaskStrength ("RGB Pixel Mask", Range(0,0.5)) = 0.10
        _ScreenSpeed ("Screen Animation Speed", Range(0,3)) = 1

        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
        [HideInInspector] [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One One
        ColorMask [_ColorMask]

        Pass
        {
            Name "CardScreen"
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            #pragma multi_compile_local _ UNITY_UI_VERTEX_COLOR_GAMMA
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct v2f
            {
                float4 vertex : SV_POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                half4 mask : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _Color;
            half4 _TextureSampleAdd;
            float4 _ClipRect;
            float _UIMaskSoftnessX, _UIMaskSoftnessY;
            float _DigitalStrength;
            float _DigitalScanlineStrength, _DigitalScanlineSpacing;
            float _DigitalNoiseStrength, _DigitalFlickerStrength;
            float _PixelMaskStrength, _ScreenSpeed;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                #ifdef UNITY_UI_VERTEX_COLOR_GAMMA
                if (!IsGammaSpace()) o.color.rgb = UIGammaToLinear(o.color.rgb);
                #endif

                float2 pixelSize = o.vertex.w;
                pixelSize /= abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
                float4 rect = clamp(_ClipRect, -2e10, 2e10);
                o.mask = half4(v.vertex.xy * 2 - rect.xy - rect.zw,
                    0.25 / (0.25 * float2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize)));
                return o;
            }

            float Noise(float2 pixel, float tick)
            {
                return frac(sin(dot(pixel, float2(127.1,311.7)) + tick * 74.7) * 43758.5453);
            }

            half4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                half4 original = (tex2D(_MainTex, i.texcoord) + _TextureSampleAdd) * i.color;
                half4 color = original;
                float strength = saturate(_DigitalStrength);
                if (strength > 0)
                {
                    float digitalAlpha = original.a;
                    float3 digitalColor = original.rgb;

                    float2 pixels = i.vertex.xy;
                    float time = _Time.y * max(_ScreenSpeed, 0);
                    float scan = 0.5 + 0.5 * sin(6.2831853 *
                        (pixels.y / max(_DigitalScanlineSpacing, 2) - time * 0.12));
                    float grain = Noise(floor(pixels), floor(time * 12));
                    float flutter = 0.5 + 0.5 * sin(time * 11 + sin(time * 2.3));
                    float modulation = (1 - saturate(_DigitalScanlineStrength) * (1 - scan))
                        * (1 - saturate(_DigitalNoiseStrength) * grain)
                        * (1 - saturate(_DigitalFlickerStrength) * flutter);
                    float column = fmod(floor(pixels.x), 3);
                    float3 pixelMask = column < 1 ? float3(1,0.72,0.72) :
                        (column < 2 ? float3(0.72,1,0.72) : float3(0.72,0.72,1));
                    digitalColor *= modulation * lerp(1.0.xxx, pixelMask, saturate(_PixelMaskStrength));
                    digitalAlpha *= 1 - strength * saturate(_DigitalScanlineStrength) * (1 - scan) * 0.18;
                    color.rgb = lerp(original.rgb, digitalColor, strength);
                    color.a = lerp(original.a, digitalAlpha, strength);
                }

                // Apply UI clipping after the screen texture modulation.
                #ifdef UNITY_UI_CLIP_RECT
                half2 mask = saturate((_ClipRect.zw - _ClipRect.xy - abs(i.mask.xy)) * i.mask.zw);
                color.a *= mask.x * mask.y;
                #endif
                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif
                color.rgb *= color.a;
                return color;
            }
            ENDCG
        }
    }
    Fallback "UI/Default"
}
