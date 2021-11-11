Shader "Custom/FireShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo", 2D) = "white" {}

        
        _PColor("Paint Color",Color)=(1,1,1,1)

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Glossiness("Roughness", Range(0.0, 1.0)) = 0.5
        _SpecGlossMap("Roughness Map", 2D) = "white" {}

        _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap("Metallic", 2D) = "white" {}

        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

        _BumpScale("Scale", Float) = 1.0
        [Normal] _BumpMap("Normal Map", 2D) = "bump" {}

        _Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02
        _ParallaxMap ("Height Map", 2D) = "black" {}

        _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        _DetailMask("Detail Mask", 2D) = "white" {}

        _DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
        _DetailNormalMapScale("Scale", Float) = 1.0
        [Normal] _DetailNormalMap("Normal Map", 2D) = "bump" {}

        [Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0


        // Blending state
        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
    }

    CGINCLUDE
        #define UNITY_SETUP_BRDF_INPUT RoughnessSetup
    ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 300


        // ------------------------------------------------------------------
        //  Base forward pass (directional light, emission, lightmaps, ...)
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]

            CGPROGRAM
            #pragma target 3.5

            // -------------------------------------
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SPECGLOSSMAP
            #pragma shader_feature_local _DETAIL_MULX2
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature_local _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityStandardCoreForward.cginc"

            struct v2f
            {
                float3 worldPos : TEXCOORD10;
                VertexOutputForwardBase base;
            };

       
            float _Power[20];
            int _CoordinatesCount = 0;
            fixed3 _Coordinate[20];
            fixed4 _PColor;

            v2f vert (VertexInput v)
            {
                v2f o;
                o.base = vertBase(v);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half4 col = fragBase(i.base);
                float draw = 0;
                if(_CoordinatesCount > 0) {
                    for (int j = 0; j < _CoordinatesCount; j++) {
                        float posdistance = distance(i.worldPos,_Coordinate[j].xyz);
                        if(posdistance <  1+_Power[j]) {
                            float draw2 = pow(1+_Power[j]-posdistance,8);
                            
                                draw += draw2;
                        }
                    }
                }
                fixed4 drawcol = _PColor * (draw);
                return saturate(col+drawcol);
            }
            

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Additive forward pass (one light per pass)
        Pass
        {
            Name "FORWARD_DELTA"
            Tags { "LightMode" = "ForwardAdd" }
            Blend [_SrcBlend] One
            Fog { Color (0,0,0,0) } // in additive pass fog should be black
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
            #pragma target 3.5

            // -------------------------------------

            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SPECGLOSSMAP
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _DETAIL_MULX2
            #pragma shader_feature_local _PARALLAXMAP

            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog

            #pragma vertex vertAdd
            #pragma fragment fragAdd
            #include "UnityStandardCoreForward.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Shadow rendering pass
        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On ZTest LEqual

            CGPROGRAM
            #pragma target 3.5

            // -------------------------------------

            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _PARALLAXMAP
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing

            #pragma vertex vertShadowCaster
            #pragma fragment fragShadowCaster

            #include "UnityStandardShadow.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Deferred pass
        Pass
        {
            Name "DEFERRED"
            Tags { "LightMode" = "Deferred" }

            CGPROGRAM
            #pragma target 3.0
            #pragma exclude_renderers nomrt


            // -------------------------------------
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SPECGLOSSMAP
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _DETAIL_MULX2
            #pragma shader_feature_local _PARALLAXMAP

            #pragma multi_compile_prepassfinal
            #pragma multi_compile_instancing

            #pragma vertex vertDeferred
            #pragma fragment fragDeferred

            #include "UnityStandardCore.cginc"

            ENDCG
        }

        // ------------------------------------------------------------------
        // Extracts information for lightmapping, GI (emission, albedo, ...)
        // This pass it not used during regular rendering.
        Pass
        {
            Name "META"
            Tags { "LightMode"="Meta" }

            Cull Off

            CGPROGRAM
            #pragma vertex vert_meta
            #pragma fragment frag_meta

            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SPECGLOSSMAP
            #pragma shader_feature_local _DETAIL_MULX2
            #pragma shader_feature EDITOR_VISUALIZATION

            #include "UnityStandardMeta.cginc"
            ENDCG
        }

      
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 150

        // ------------------------------------------------------------------
        //  Base forward pass (directional light, emission, lightmaps, ...)
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]

            CGPROGRAM
            #pragma target 2.0
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SPECGLOSSMAP
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
            // SM2.0: NOT SUPPORTED shader_feature_local _DETAIL_MULX2
            // SM2.0: NOT SUPPORTED shader_feature_local _PARALLAXMAP

            #pragma skip_variants SHADOWS_SOFT DIRLIGHTMAP_COMBINED

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog

            #pragma vertex vertBase
            #pragma fragment fragBase
            #include "UnityStandardCoreForward.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Additive forward pass (one light per pass)
        Pass
        {
            Name "FORWARD_DELTA"
            Tags { "LightMode" = "ForwardAdd" }
            Blend [_SrcBlend] One
            Fog { Color (0,0,0,0) } // in additive pass fog should be black
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
            #pragma target 2.0
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SPECGLOSSMAP
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            // SM2.0: NOT SUPPORTED #pragma shader_feature_local _DETAIL_MULX2
            // SM2.0: NOT SUPPORTED shader_feature_local _PARALLAXMAP
            #pragma skip_variants SHADOWS_SOFT

            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog

            #pragma vertex vertAdd
            #pragma fragment fragAdd
            #include "UnityStandardCoreForward.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Shadow rendering pass
        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On ZTest LEqual

            CGPROGRAM
            #pragma target 2.0
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SPECGLOSSMAP
            #pragma skip_variants SHADOWS_SOFT
            #pragma multi_compile_shadowcaster

            #pragma vertex vertShadowCaster
            #pragma fragment fragShadowCaster

            #include "UnityStandardShadow.cginc"

            ENDCG
        }

        // ------------------------------------------------------------------
        // Extracts information for lightmapping, GI (emission, albedo, ...)
        // This pass it not used during regular rendering.
        Pass
        {
            Name "META"
            Tags { "LightMode"="Meta" }

            Cull Off

            CGPROGRAM
            #pragma vertex vert_meta
            #pragma fragment frag_meta

            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SPECGLOSSMAP
            #pragma shader_feature_local _DETAIL_MULX2
            #pragma shader_feature EDITOR_VISUALIZATION

            #include "UnityStandardMeta.cginc"
            ENDCG
        }
        
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" }
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
            fixed4 _Coordinate,_PColor;

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
                float posdistance = distance(i.worldPos,_Coordinate.xyz);
                float draw = 0;
                if(posdistance < 1+_Power)
                    draw = pow(1+_Power-posdistance,8);
                fixed4 drawcol = _PColor * (draw);
                return saturate(drawcol);
            }
            ENDCG
        }
    }


    FallBack "VertexLit"
  
}


// Shader "Custom/FireShader"
// {
//     Properties
//     {
//         _Color("Color", Color) = (1,1,1,1)
//         _MainTex("Albedo", 2D) = "white" {}
//         _Coordinate("Coordinate",Vector)=(0,0,0,0)
//         _Power("Power",Float)=0.0
//         _PColor("Paint Color",Color)=(1,1,1,1)

//         _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

//         _Glossiness("Roughness", Range(0.0, 1.0)) = 0.5
//         _SpecGlossMap("Roughness Map", 2D) = "white" {}

//         _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
//         _MetallicGlossMap("Metallic", 2D) = "white" {}

//         [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
//         [ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

//         _BumpScale("Scale", Float) = 1.0
//         [Normal] _BumpMap("Normal Map", 2D) = "bump" {}

//         _Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02
//         _ParallaxMap ("Height Map", 2D) = "black" {}

//         _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
//         _OcclusionMap("Occlusion", 2D) = "white" {}

//         _EmissionColor("Color", Color) = (0,0,0)
//         _EmissionMap("Emission", 2D) = "white" {}

//         _DetailMask("Detail Mask", 2D) = "white" {}

//         _DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
//         _DetailNormalMapScale("Scale", Float) = 1.0
//         [Normal] _DetailNormalMap("Normal Map", 2D) = "bump" {}

//         [Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0


//         // Blending state
//         [HideInInspector] _Mode ("__mode", Float) = 0.0
//         [HideInInspector] _SrcBlend ("__src", Float) = 1.0
//         [HideInInspector] _DstBlend ("__dst", Float) = 0.0
//         [HideInInspector] _ZWrite ("__zw", Float) = 1.0
//     }

//     CGINCLUDE
//         #define UNITY_SETUP_BRDF_INPUT RoughnessSetup
//     ENDCG

//     SubShader
//     {
//         Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
//         LOD 300


//         // ------------------------------------------------------------------
//         //  Base forward pass (directional light, emission, lightmaps, ...)
//         Pass
//         {
//             Name "FORWARD"
//             Tags { "LightMode" = "ForwardBase" }

//             Blend [_SrcBlend] [_DstBlend]
//             ZWrite [_ZWrite]

//             CGPROGRAM
//             #pragma target 3.5

//             // -------------------------------------
//             #pragma shader_feature_local _NORMALMAP
//             #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
//             #pragma shader_feature _EMISSION
//             #pragma shader_feature_local _METALLICGLOSSMAP
//             #pragma shader_feature_local _SPECGLOSSMAP
//             #pragma shader_feature_local _DETAIL_MULX2
//             #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
//             #pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
//             #pragma shader_feature_local _PARALLAXMAP

//             #pragma multi_compile_fwdbase
//             #pragma multi_compile_fog
//             #pragma multi_compile_instancing

//             #pragma vertex vert
//             #pragma fragment frag
//             #include "UnityStandardCoreForward.cginc"


//             struct v2f
//             {
//                 float3 worldPos : TEXCOORD10;
//                 VertexOutputForwardBase base;
//             };

       
//             float _Power;
       
//             fixed4 _Coordinate,_PColor;

//             v2f vert (VertexInput v)
//             {
//                 v2f o;
//                 o.base = vertBase(v);
//                 o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

//                 return o;
//             }

//             fixed4 frag (v2f i) : SV_Target
//             {
//                 // sample the texture
//                 half4 col = fragBase(i.base);
//                 float posdistance = distance(i.worldPos,_Coordinate.xyz);
//                 float draw = 0;
//                 if(posdistance < 1+_Power)
//                     draw = pow(1+_Power-posdistance,8);
//                 fixed4 drawcol = _PColor * (draw);
//                 return saturate(col+drawcol);
//             }
            

//             ENDCG
//         }
//         // ------------------------------------------------------------------
//         //  Additive forward pass (one light per pass)
//         Pass
//         {
//             Name "FORWARD_DELTA"
//             Tags { "LightMode" = "ForwardAdd" }
//             Blend [_SrcBlend] One
//             Fog { Color (0,0,0,0) } // in additive pass fog should be black
//             ZWrite Off
//             ZTest LEqual

//             CGPROGRAM
//             #pragma target 3.5

//             // -------------------------------------

//             #pragma shader_feature_local _NORMALMAP
//             #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
//             #pragma shader_feature_local _METALLICGLOSSMAP
//             #pragma shader_feature_local _SPECGLOSSMAP
//             #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
//             #pragma shader_feature_local _DETAIL_MULX2
//             #pragma shader_feature_local _PARALLAXMAP

//             #pragma multi_compile_fwdadd_fullshadows
//             #pragma multi_compile_fog

//             #pragma vertex vertAdd
//             #pragma fragment fragAdd
//             #include "UnityStandardCoreForward.cginc"

//             ENDCG
//         }
//         // ------------------------------------------------------------------
//         //  Shadow rendering pass
//         Pass {
//             Name "ShadowCaster"
//             Tags { "LightMode" = "ShadowCaster" }

//             ZWrite On ZTest LEqual

//             CGPROGRAM
//             #pragma target 3.5

//             // -------------------------------------

//             #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
//             #pragma shader_feature_local _METALLICGLOSSMAP
//             #pragma shader_feature_local _PARALLAXMAP
//             #pragma multi_compile_shadowcaster
//             #pragma multi_compile_instancing

//             #pragma vertex vertShadowCaster
//             #pragma fragment fragShadowCaster

//             #include "UnityStandardShadow.cginc"

//             ENDCG
//         }
//         // ------------------------------------------------------------------
//         //  Deferred pass
//         Pass
//         {
//             Name "DEFERRED"
//             Tags { "LightMode" = "Deferred" }

//             CGPROGRAM
//             #pragma target 3.0
//             #pragma exclude_renderers nomrt


//             // -------------------------------------
//             #pragma shader_feature_local _NORMALMAP
//             #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
//             #pragma shader_feature _EMISSION
//             #pragma shader_feature_local _METALLICGLOSSMAP
//             #pragma shader_feature_local _SPECGLOSSMAP
//             #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
//             #pragma shader_feature_local _DETAIL_MULX2
//             #pragma shader_feature_local _PARALLAXMAP

//             #pragma multi_compile_prepassfinal
//             #pragma multi_compile_instancing

//             #pragma vertex vertDeferred
//             #pragma fragment fragDeferred

//             #include "UnityStandardCore.cginc"

//             ENDCG
//         }

//         // ------------------------------------------------------------------
//         // Extracts information for lightmapping, GI (emission, albedo, ...)
//         // This pass it not used during regular rendering.
//         Pass
//         {
//             Name "META"
//             Tags { "LightMode"="Meta" }

//             Cull Off

//             CGPROGRAM
//             #pragma vertex vert_meta
//             #pragma fragment frag_meta

//             #pragma shader_feature _EMISSION
//             #pragma shader_feature_local _METALLICGLOSSMAP
//             #pragma shader_feature_local _SPECGLOSSMAP
//             #pragma shader_feature_local _DETAIL_MULX2
//             #pragma shader_feature EDITOR_VISUALIZATION

//             #include "UnityStandardMeta.cginc"
//             ENDCG
//         }

      
//     }

//     SubShader
//     {
//         Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
//         LOD 150

//         // ------------------------------------------------------------------
//         //  Base forward pass (directional light, emission, lightmaps, ...)
//         Pass
//         {
//             Name "FORWARD"
//             Tags { "LightMode" = "ForwardBase" }

//             Blend [_SrcBlend] [_DstBlend]
//             ZWrite [_ZWrite]

//             CGPROGRAM
//             #pragma target 2.0
//             #pragma shader_feature_local _NORMALMAP
//             #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
//             #pragma shader_feature _EMISSION
//             #pragma shader_feature_local _METALLICGLOSSMAP
//             #pragma shader_feature_local _SPECGLOSSMAP
//             #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
//             #pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
//             // SM2.0: NOT SUPPORTED shader_feature_local _DETAIL_MULX2
//             // SM2.0: NOT SUPPORTED shader_feature_local _PARALLAXMAP

//             #pragma skip_variants SHADOWS_SOFT DIRLIGHTMAP_COMBINED

//             #pragma multi_compile_fwdbase
//             #pragma multi_compile_fog

//             #pragma vertex vertBase
//             #pragma fragment fragBase
//             #include "UnityStandardCoreForward.cginc"

//             ENDCG
//         }
//         // ------------------------------------------------------------------
//         //  Additive forward pass (one light per pass)
//         Pass
//         {
//             Name "FORWARD_DELTA"
//             Tags { "LightMode" = "ForwardAdd" }
//             Blend [_SrcBlend] One
//             Fog { Color (0,0,0,0) } // in additive pass fog should be black
//             ZWrite Off
//             ZTest LEqual

//             CGPROGRAM
//             #pragma target 2.0
//             #pragma shader_feature_local _NORMALMAP
//             #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
//             #pragma shader_feature_local _METALLICGLOSSMAP
//             #pragma shader_feature_local _SPECGLOSSMAP
//             #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
//             // SM2.0: NOT SUPPORTED #pragma shader_feature_local _DETAIL_MULX2
//             // SM2.0: NOT SUPPORTED shader_feature_local _PARALLAXMAP
//             #pragma skip_variants SHADOWS_SOFT

//             #pragma multi_compile_fwdadd_fullshadows
//             #pragma multi_compile_fog

//             #pragma vertex vertAdd
//             #pragma fragment fragAdd
//             #include "UnityStandardCoreForward.cginc"

//             ENDCG
//         }
//         // ------------------------------------------------------------------
//         //  Shadow rendering pass
//         Pass {
//             Name "ShadowCaster"
//             Tags { "LightMode" = "ShadowCaster" }

//             ZWrite On ZTest LEqual

//             CGPROGRAM
//             #pragma target 2.0
//             #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
//             #pragma shader_feature_local _METALLICGLOSSMAP
//             #pragma shader_feature_local _SPECGLOSSMAP
//             #pragma skip_variants SHADOWS_SOFT
//             #pragma multi_compile_shadowcaster

//             #pragma vertex vertShadowCaster
//             #pragma fragment fragShadowCaster

//             #include "UnityStandardShadow.cginc"

//             ENDCG
//         }

//         // ------------------------------------------------------------------
//         // Extracts information for lightmapping, GI (emission, albedo, ...)
//         // This pass it not used during regular rendering.
//         Pass
//         {
//             Name "META"
//             Tags { "LightMode"="Meta" }

//             Cull Off

//             CGPROGRAM
//             #pragma vertex vert_meta
//             #pragma fragment frag_meta

//             #pragma shader_feature _EMISSION
//             #pragma shader_feature_local _METALLICGLOSSMAP
//             #pragma shader_feature_local _SPECGLOSSMAP
//             #pragma shader_feature_local _DETAIL_MULX2
//             #pragma shader_feature EDITOR_VISUALIZATION

//             #include "UnityStandardMeta.cginc"
//             ENDCG
//         }
        
//     }

//     SubShader
//     {
//         Tags { "RenderType"="Transparent" }
//         LOD 100

//         Pass
//         {
//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag

//             #include "UnityCG.cginc"

//             struct appdata
//             {
//                 float4 vertex : POSITION;
//                 float2 uv : TEXCOORD0;
//             };

//             struct v2f
//             {
//                 float2 uv : TEXCOORD0;
//                 float4 vertex : SV_POSITION;
//                 float3 worldPos : TEXCOORD1;
//             };

//             sampler2D _MainTex;
//             float _Power;
//             float4 _MainTex_ST;
//             fixed4 _Coordinate,_PColor;

//             v2f vert (appdata v)
//             {
//                 v2f o;
//     o.vertex = UnityObjectToClipPos(v.vertex);
//     o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//     o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

//                 return o;
//             }

//             fixed4 frag (v2f i) : SV_Target
//             {
//                 // sample the texture
//                 float posdistance = distance(i.worldPos,_Coordinate.xyz);
//                 float draw = 0;
//                 if(posdistance < 1+_Power)
//                     draw = pow(1+_Power-posdistance,8);
//                 fixed4 drawcol = _PColor * (draw);
//                 return saturate(drawcol);
//             }
//             ENDCG
//         }
//     }


//     FallBack "VertexLit"
  
// }
// // {
// //     Properties
// //     {
// //         _MainTex ("Texture", 2D) = "white" {}
// //         _Coordinate("Coordinate",Vector)=(0,0,0,0)
// //         _Power("Power",Float)=0.0
// //         _Color("Paint Color",Color)=(1,1,1,1)
// //     }
// //     SubShader
// //     {
// //         Tags { "RenderType"="Opaque" }
// //         LOD 100

// //         Pass
// //         {
// //             CGPROGRAM
// //             #pragma vertex vert
// //             #pragma fragment frag

// //             #include "UnityCG.cginc"

// //             struct appdata
// //             {
// //                 float4 vertex : POSITION;
// //                 float2 uv : TEXCOORD0;
// //             };

// //             struct v2f
// //             {
// //                 float2 uv : TEXCOORD0;
// //                 float4 vertex : SV_POSITION;
// //                 float3 worldPos : TEXCOORD1;
// //             };

// //             sampler2D _MainTex;
// //             float _Power;
// //             float4 _MainTex_ST;
// //             fixed4 _Coordinate,_Color;

// //             v2f vert (appdata v)
// //             {
// //                 v2f o;
// //     o.vertex = UnityObjectToClipPos(v.vertex);
// //     o.uv = TRANSFORM_TEX(v.uv, _MainTex);
// //     o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

// //                 return o;
// //             }

// //             fixed4 frag (v2f i) : SV_Target
// //             {
// //                 // sample the texture
// //                 fixed4 col = tex2D(_MainTex, i.uv);
// //                 float posdistance = distance(i.worldPos,_Coordinate.xyz);
// //                 float draw = 0;
// //                 if(posdistance < 1+_Power)
// //                     draw = pow(1+_Power-posdistance,8);
// //                 fixed4 drawcol = _Color * (draw);
// //                 return saturate(col + drawcol);
// //             }
// //             ENDCG
// //         }
// //     }
// // }