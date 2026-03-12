Shader "Custom/GridOutlineShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,0.5)
        _OutlineThickness ("Outline Thickness (Voxel Units)", Float) = 0.5
        _VoxelResolution ("Voxel Resolution per Grid", Float) = 16.0
        _CellSize ("Cell Size (World Units)", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 positionOS   : TEXCOORD1;
                float3 normalWS     : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _OutlineColor;
                float _OutlineThickness;
                float _VoxelResolution;
                float _CellSize;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.positionOS = input.positionOS.xyz;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float lineThicknessOS = _OutlineThickness / _VoxelResolution;

                float distX = 0.5 - abs(input.positionOS.x);
                float distY = 0.5 - abs(input.positionOS.y);
                float distZ = 0.5 - abs(input.positionOS.z);
                
                int edgeCount = 0;
                if (distX <= lineThicknessOS) edgeCount++;
                if (distY <= lineThicknessOS) edgeCount++;
                if (distZ <= lineThicknessOS) edgeCount++;

                half4 finalColor = _BaseColor;

                // A fragment is on a 3D edge if it is near the boundary on at least TWO axes
                if (edgeCount >= 2)
                {
                    finalColor = lerp(_BaseColor, _OutlineColor, _OutlineColor.a);
                }

                return finalColor;
            }
            ENDHLSL
        }
    }
}
