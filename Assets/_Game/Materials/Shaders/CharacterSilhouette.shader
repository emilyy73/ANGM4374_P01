Shader "Unlit/CharacterSilhouette"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "White" {}
        _ShadowColor ("Shadow Color", Color) = (0.3, 0.3, 0.3, 0.3)
    }
    SubShader
    {
        Tags {  "RenderType"="Transparent"
                "Queue"="Transparent"       }
        LOD 100

        Pass
        {
            ZTest GEqual
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ShadowColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return (col.w > 0 ? _ShadowColor : col);
            }
            ENDCG
        }
    }
}
