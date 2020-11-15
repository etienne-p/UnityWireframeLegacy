Shader "Unlit/Wireframe"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
                float4 position : POSITION;
                float2 barycentricCoords : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 barycentricCoords : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.position);
                o.barycentricCoords = v.barycentricCoords;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Barycentric coordinates sum up to one, so we only pass 2 components and infer the third.
                float3 barycentricCoords = float3(i.barycentricCoords, 1 - dot(i.barycentricCoords, float2(1, 1)));
                return float4(barycentricCoords, 1);
            }
            ENDCG
        }
    }
}
