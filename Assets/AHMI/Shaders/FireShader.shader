Shader "Custom/FireShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Coordinate("Coordinate",Vector)=(0,0,0,0)
        _Power("Power",Float)=0.0
        _Color("Paint Color",Color)=(1,1,1,1)
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
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float _Power;
            float4 _MainTex_ST;
            fixed4 _Coordinate,_Color;

            v2f vert (appdata v)
            {
                v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float posdistance = distance(i.worldPos,_Coordinate.xyz);
                float draw = 0;
                if(posdistance < 1+_Power)
                    draw = pow(1+_Power-posdistance,8);
                fixed4 drawcol = _Color * (draw);
                return saturate(col + drawcol);
            }
            ENDCG
        }
    }
}