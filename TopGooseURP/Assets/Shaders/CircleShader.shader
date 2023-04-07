Shader "Example/CircleShader"
{
    // The properties block of the Unity shader. In this example this block is empty
    // because the output color is predefined in the fragment shader code.
    Properties
    { 
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _Thickness("Thickness", Float) = 10
        _Fade("Fade", Float) = 0.005
    }

    // The SubShader block containing the Shader code. 
    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags { "RenderType" = "Trasparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalRenderPipeline" }
        Cull Off

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZWrite Off
            LOD 150
            
            // The HLSL code block. Unity SRP uses the HLSL language.
            HLSLPROGRAM
            // This line defines the name of the vertex shader. 
            #pragma vertex vert
            // This line defines the name of the fragment shader. 
            #pragma fragment frag

            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Thickness;
            float _Fade;

            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            struct Attributes
            {
                // The positionOS variable contains the vertex positions in object
                // space.
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR; 
            };

            struct Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float4 LocalPos     : TEXCOORD1;
                float4 color        : COLOR;
            };            

            // The vertex shader definition with properties defined in the Varyings 
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
            Varyings vert(Attributes IN)
            {
                // Declaring the output object (OUT) with the Varyings struct.
                Varyings OUT;
                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous space
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);

                //OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.uv = IN.uv;

                OUT.color = IN.color;

                // Returning the output.
                return OUT;
            }

            // The fragment shader definition.            
            half4 frag(Varyings IN) : SV_Target
            {
                float thickness = _Thickness/ _ScreenParams.x;
                float fade = _Fade/_ScreenParams.x;

                // -1 -> 1 local space, adjusted for aspect ratio
                float2 uv = IN.uv * 2.0 - 1.0;
                //float2 uv = (IN.positionHCS.xy - _ScreenParams.xy * 0.5) / min(_ScreenParams.x, _ScreenParams.y) * 2.0;
                //float aspect = iResolution.x / iResolution.y;
                //uv.x *= aspect;
    
                // Calculate distance and fill circle with white
                float distance = 1.0 - length(uv);
                float alpha = smoothstep(0.0, fade, distance);
                alpha *= smoothstep(thickness + fade, thickness, distance);

                // Set output color
                float4 color = IN.color;
                color.a *= alpha;
                return color;
            }
            ENDHLSL
        }
    }
}