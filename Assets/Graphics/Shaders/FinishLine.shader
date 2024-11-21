Shader "Unlit/FinishLine"
{
    Properties
    {
        _InnerColor1("Inner color 1", Color) = (1,1,1,1)
        _InnerColor2("Inner color 2", Color) = (0,0,0,1)
        _NbRow("Number of stripe", Float) = 10
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            fixed4 _InnerColor1;
            fixed4 _InnerColor2;
            fixed _NbRow;

            fixed isEven(fixed nb) {
                return fmod(nb, 2);
            }

            fixed4 frag (v2f i) : SV_Target
            {                
                fixed stagedX = stagedDistance(i.uv.x, _NbRow);
                fixed nbCase = floor(i.uv.x * _NbRow);
                
                // sample the texture
                fixed4 col = lerp(_InnerColor1, _InnerColor2, isEven(nbCase));
    
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}