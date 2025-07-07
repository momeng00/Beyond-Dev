
Shader "Skybox/Parallax"
{
    Properties
    {
        _MainTex("Main", 2D) = "white" {}

        _Background("Back",2D) = "white" {}
        _Betweenground("Between",2D) = "white" {}
        _Foreground("Fore",2D) = "white" {}

        _OffsetBack("OffsetBack",float) = 0
        _OffsetBetween("OffsetBetween",float) = 0
        _OffsetFore("OffsetFore",float) = 0
    }

        CGINCLUDE

#include "UnityCG.cginc"

    struct appdata
    {
        float4 position : POSITION;
        float2 texcoord : TEXCOORD0;
    };

    struct v2f
    {
        float4 position : SV_POSITION;
        float2 texcoord : TEXCOORD0;
    };

    sampler2D _MainTex;
    float4 _MainTex_ST;

    sampler2D _Background;
    sampler2D _Betweenground;
    sampler2D _Foreground;

    float _OffsetBack;
    float _OffsetBetween;
    float _OffsetFore;

    v2f vert(appdata v)
    {
        v2f o;
        o.position = UnityObjectToClipPos(v.position);
        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
        return o;
    }


    half4 frag(v2f i) : COLOR
    {
        return lerp((tex2D(_Foreground,i.texcoord + half4(_OffsetFore, 0, 0, 0))),
                    lerp((tex2D(_Betweenground, i.texcoord + half4(_OffsetBetween, 0, 0, 0))),
                        (tex2D(_Background, i.texcoord + half4(_OffsetBack, 0, 0, 0))), 1 - saturate((tex2D(_Betweenground, i.texcoord + half4(_OffsetBetween, 0, 0, 0))).a))
                    ,1 - saturate((tex2D(_Foreground,i.texcoord + half4(_OffsetFore, 0, 0, 0))).a));
    }

    ENDCG

    SubShader
    {
        Tags{ "RenderType" = "Background" "Queue" = "Background" }
            Pass
        {
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            CGPROGRAM
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
    CustomEditor "GradientSkyboxInspector"
}
