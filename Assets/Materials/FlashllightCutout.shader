Shader "UI/FlashlightCutout"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LightPos ("Light Position", Vector) = (0.5, 0.5, 0, 0)
        _LightRadius ("Light Radius", Float) = 0.2
        _LightSoftness ("Light Softness", Range(0, 1)) = 0.3
        _DarknessColor ("Darkness Color", Color) = (0, 0, 0, 1)
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _LightPos;
            float _LightRadius;
            float _LightSoftness;
            fixed4 _DarknessColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Távolság számítás a fényforrástól
                float dist = distance(i.uv, _LightPos);
                
                // Smooth falloff a fény széleinél
                float alpha = smoothstep(_LightRadius - _LightSoftness, _LightRadius, dist);
                
                // Sötétség alkalmazása
                fixed4 col = _DarknessColor;
                col.a *= alpha;
                
                return col;
            }
            ENDCG
        }
    }
}