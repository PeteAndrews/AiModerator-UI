Shader "Political2D/bubble_merge"
{
    Properties
    {
        [mainTex] _MainTex ("Mask", 2D) = "white" {}
        _Video ("Video", 2D) = "white" {}
        _Bubble ("Bubble", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _Bubble;
            sampler2D _Video;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 mask = tex2D(_MainTex, i.uv);
                mask=step(0.3,mask);
                fixed4 bubble = tex2D(_Bubble, i.uv);
                fixed4 video = tex2D(_Video, i.uv);
                fixed4 col=lerp(video, bubble, mask);

                return col;
            }
            ENDCG
        }
    }
}
