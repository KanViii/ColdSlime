Shader "Custom/NewUnlitUniversalRenderPipelineShader"
{
    Properties // giống serializefield
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1) // red, green, blue, alpha 
        _AmbientColor ("Ambient", Color) = (0.3, 0.3, 0.3, 1)

        _BaseMap("Base Map", 2D) = "white" {} // 2D --> reference to texture
        // tên hiển thị trong sctipts (" tên hiển thị inspector", type) = default value 

        // _Tilling ("Tilling", Vector) = (1, 1, 1, 1)
        // _Offset("Offset", Vector) = (1, 1, 1, 1)

        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
        _Smoothness("Smoothness", Range(0.01, 256)) = 32.0 //tại sao lại là range từ 0.1 đến 256? --> kênh độ bóng 8bit (max 256), 0**0 (undefined)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct MyMeshData
            {
                float4 positionOS : POSITION; // chiều thứ 4 là direction (0) hoặc position (1)
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                // : POSITION (simantic) --> cho unity biết phải đọc data nào
                // : TEXCOORD0; --> đọc vị trí của cái mesh texture layout 
            };

            struct VertextOutput
            {
                float4 positionHCS : SV_POSITION; //tọa độ clipspace 
                float2 uv : TEXCOORD0; // chạy từ 1-32, nó chỉ là data dùng trong các việc tính toán sau, ví dụ như sample (tilling)

                float3 normal : TEXCOORD2; 
                float3 positionWS: TEXCOORD3; // tọa độ world space
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            // float4 _Tilling;  // sampling state 
            // float4 _Offset;  // sampling state 

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor; 
                float4 _BaseMap_ST; // biến dùng cho hàm TRANSFORM_TEX 
                // những biến được phép khác nhau trong 1 patch 
                // texture không cần nằm trong CBuffer
                float3 _AmbientColor;

                half4 _SpecularColor;
                float _Smoothness;
            CBUFFER_END

            VertextOutput vert(MyMeshData IN)
            {
                VertextOutput output;
                output.positionHCS = TransformObjectToHClip(IN.positionOS.xyz); // chuyển từ world space sang clip space 
                output.uv = TRANSFORM_TEX(IN.uv, _BaseMap); 
                // output.uv = IN.uv * _Tilling.xy + _Offset.xy;

                output.normal = TransformObjectToWorldNormal(IN.normalOS);
                output.positionWS = TransformObjectToWorld(IN.positionOS.xyz);

                return output;
            }

            half4 frag(VertextOutput IN) : SV_Target
            {
                Light mainLight = GetMainLight();
                IN.normal = normalize(IN.normal);
                half NdotL = saturate(dot(mainLight.direction, IN.normal));

                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - IN.positionWS); // tại sao :)) --> Camera = viewpoint = A, điểm: B | vector từ mắt tới điểm (A -> B) = B - A;
                float3 halfDir = normalize(mainLight.direction + viewDir);

                float NdotH = saturate(dot(IN.normal, halfDir));
                float specularIntensity = pow(NdotH, _Smoothness); 

                half3 specular = _SpecularColor.rbg * mainLight.color * specularIntensity;


                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                // half4 color = tex2D(_BaseMap, IN.uv) *_BaseColor; //thường không được dùng vì nó không hỗ trợ blitmap - xa thì sẽ giảm độ phân giải.

                color.rgb = color.rgb * (mainLight.color * NdotL + SampleSH(IN.normal)) +specular;
                return color;
            }
            ENDHLSL
        }
    }
}
