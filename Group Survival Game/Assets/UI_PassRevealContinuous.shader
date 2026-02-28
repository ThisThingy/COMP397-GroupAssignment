Shader "UI/PassRevealContinuous"
{
    Properties
    {
        [PerRendererData] _MainTex ("BlackWhite Sprite", 2D) = "white" {}
        _ColorTex ("Color Texture", 2D) = "white" {}
        _PassMaskTex ("Pass Mask Texture", 2D) = "black" {}
        _Tint ("Tint", Color) = (1,1,1,1)

        _ScrollSpeed ("Scroll Speed", Float) = 0.3
        _TilingX ("Mask Tiling X", Float) = 1.0
        _TilingY ("Mask Tiling Y", Float) = 1.0
        _OffsetY ("Mask Offset Y", Float) = 0.0
        _MaskStrength ("Mask Strength", Range(0,1)) = 1.0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _ColorTex;
            sampler2D _PassMaskTex;

            fixed4 _Tint;
            float _ScrollSpeed;
            float _TilingX;
            float _TilingY;
            float _OffsetY;
            float _MaskStrength;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 bw = tex2D(_MainTex, i.uv) * i.color * _Tint;
                fixed4 col = tex2D(_ColorTex, i.uv) * i.color * _Tint;

                // 关键：让 passbar 纹理持续滚动
                float2 maskUV;
                maskUV.x = frac(i.uv.x * _TilingX - _Time.y * _ScrollSpeed);
                maskUV.y = frac(i.uv.y * _TilingY + _OffsetY);

                fixed mask = tex2D(_PassMaskTex, maskUV).a * _MaskStrength;

                fixed4 finalCol = lerp(bw, col, mask);
                finalCol.a = bw.a;
                return finalCol;
            }
            ENDCG
        }
    }
}