// Shader for Black and White Post-Processing Effect
Shader "Custom Post-Processing/B&W Post-Processing"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {} // Main texture input
        _blend("B & W blend", Range(0,1)) = 0 // Blend intensity between original and black and white
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        // Render settings
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            // Vertex and fragment shaders inclusion
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Uniforms
            uniform sampler2D _MainTex; // Main texture
            uniform float _blend; // Blend intensity

            // Input structure for vertex shader
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            // Output structure from vertex shader
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Vertex shader
            v2f vert(appdata v)
            {
                v2f o;
                // Transform vertex position to clip space
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Fragment shader
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv); // Sample color from the main texture

                // Calculate luminance
                float lum = c.r * .3 + c.g * .59 + c.b * .11;
                // Create a black and white color
                float3 bwc = float3(lum, lum, lum);

                float4 result = c;
                // Lerp between original color and black and white based on blend intensity
                result.rgb = lerp(c.rgb, bwc, _blend);
                return result; // Return the final color
            }
            ENDHLSL
        }
    }
}