Shader "Custom Post-Processing/Gaussian Blur"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {} // Main texture input
        _Spread("Standard Deviation", Float) = 0 // Spread (standard deviation)
        _GridSize("Grid Size", Integer) = 1 // Grid size
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        // Render settings
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        // Include necessary HLSL code from Universal Render Pipeline's Core
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #define E 2.7182818
        ENDHLSL

        // Horizontal blur pass
        Pass
        {
            Name "Horizontal"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag_horizontal

            // Declare the main texture and shader properties
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            int _GridSize;
            float _Spread;

            // Define a Gaussian function for blurring
            float gaussian(int x)
            {
                float sigmaSqu = _Spread * _Spread;
                return (1.0 / sqrt(TWO_PI * sigmaSqu)) * pow(E, -(x * x) / (2 * sigmaSqu));
            }

            // Define input and output structures for vertex and fragment shaders
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

            // Vertex shader transformation
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            // Fragment shader for horizontal blur
            float4 frag_horizontal(v2f i) : SV_Target
            {
                float3 col = float3(0.0f, 0.0f, 0.0f);
                float gridSum = 0.0f;

                int upper = (_GridSize - 1) / 2;
                int lower = -upper;

                // Apply Gaussian blur horizontally
                for (int x = lower; x <= upper; ++x)
                {
                    float gauss = gaussian(x);
                    gridSum += gauss;
                    float2 uv = i.uv + float2(_MainTex_TexelSize.x * x, 0.0f);
                    col += gauss * tex2D(_MainTex, uv).xyz;
                }

                col /= gridSum;
                return float4(col, 1.0f);
            }
            ENDHLSL
        }

        // Vertical blur pass
        Pass
        {
            Name "Vertical"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag_vertical

            // Fragment shader for vertical blur
            float4 frag_vertical(v2f i) : SV_Target
            {
                float3 col = float3(0.0f, 0.0f, 0.0f);
                float gridSum = 0.0f;

                int upper = (_GridSize - 1) / 2;
                int lower = -upper;

                // Apply Gaussian blur vertically
                for (int y = lower; y <= upper; ++y)
                {
                    float gauss = gaussian(y);
                    gridSum += gauss;
                    float2 uv = i.uv + float2(0.0f, _MainTex_TexelSize.y * y);
                    col += gauss * tex2D(_MainTex, uv).xyz;
                }

                col /= gridSum;
                return float4(col, 1.0f);
            }
            ENDHLSL
        }
    }
}