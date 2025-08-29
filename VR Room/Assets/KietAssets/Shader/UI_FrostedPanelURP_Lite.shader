Shader "UI/FrostedPanelURP_Lite"
{
    Properties
    {
        _MainTex    ("Mask (Sprite)", 2D) = "white" {}
        _TintColor  ("Tint", Color) = (1,1,1,0.25)
        _Opacity    ("Opacity", Range(0,1)) = 1
        _BlurRadius ("Blur Radius (px)", Range(0,8)) = 3
        _DepthGuard ("Use Depth Guard (0/1)", Float) = 1
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
        ZWrite Off
        ZTest LEqual                    // vật ở trước sẽ che panel (không bị tint/blur)
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "FrostedPanelDepth"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; };
            struct v2f {
                float4 pos:SV_POSITION;
                float2 uv:TEXCOORD0;
                float4 scrPos:TEXCOORD1;
                float3 worldPos:TEXCOORD2;
                float4 color:COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
            float4 _TintColor;
            float  _Opacity;
            float  _BlurRadius;
            float  _DepthGuard;
            CBUFFER_END

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos      = TransformObjectToHClip(v.vertex.xyz);
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                o.uv       = TRANSFORM_TEX(v.uv, _MainTex);
                o.scrPos   = ComputeScreenPos(o.pos);
                o.color    = v.color;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // mask theo sprite (bo góc)
                half maskA = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).a;
                clip(maskA - 0.001h);

                float2 uv = i.scrPos.xy / i.scrPos.w;

                // ---- chỉ blur nền phía SAU panel
                if (_DepthGuard > 0.5f)
                {
                    // độ sâu của cảnh (m)
                    float sceneRaw  = SampleSceneDepth(uv);
                    float sceneEye  = LinearEyeDepth(sceneRaw, _ZBufferParams);

                    // độ sâu của chính panel (m)
                    float panelEye  = -TransformWorldToView(i.worldPos).z;

                    // có vật thể gần hơn panel -> KHÔNG vẽ pixel này (tránh nhuộm/tím đồ phía trước)
                    if (sceneEye + 1e-4 < panelEye) return 0;
                }

                // ---- blur 9-tap nhẹ từ _CameraOpaqueTexture
                half4 col;
                if (_BlurRadius <= 0.0001)
                {
                    col = SampleSceneColor(uv);
                }
                else
                {
                    float2 t = _CameraOpaqueTexture_TexelSize.xy * _BlurRadius;
                    col  = SampleSceneColor(uv) * 0.2h;
                    col += SampleSceneColor(uv + float2( t.x,  0)) * 0.1h;
                    col += SampleSceneColor(uv + float2(-t.x,  0)) * 0.1h;
                    col += SampleSceneColor(uv + float2( 0,  t.y)) * 0.1h;
                    col += SampleSceneColor(uv + float2( 0, -t.y)) * 0.1h;
                    col += SampleSceneColor(uv +  t)               * 0.1h;
                    col += SampleSceneColor(uv -  t)               * 0.1h;
                    col += SampleSceneColor(uv + float2( t.x,-t.y))* 0.1h;
                    col += SampleSceneColor(uv + float2(-t.x, t.y))* 0.1h;
                }

                // pha tint kiểu kính mờ
                half3 rgb = lerp(col.rgb, _TintColor.rgb, _TintColor.a);
                half  a   = saturate(maskA * _Opacity * i.color.a);
                return half4(rgb, a);
            }
            ENDHLSL
        }
    }
    FallBack Off
}