Shader "AIE/PostProcess/Outliner"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _DeltaX ("Delta X", Float) = 0.01
        _DeltaY ("Delta Y", Float) = 0.01


    }
    SubShader
    {
        Tags {"Render Type" = "Opaque"}
        LOD 200

        CGINCLUDE

        #include "UnityCG.cginc"
        
        sampler2D _MainTex;
        float _DeltaX;
        float _DeltaY;
        sampler2D _CameraDepthTexture;

        float Sobel(sampler2D tex, float2 uv, float mult)
        {
            float2 delta = float2(_DeltaX, _DeltaY);
            float4 hr = float4(0,0,0,0);
            float4 vt = float4(0,0,0,0);

            hr += tex2D(tex, (uv + float2(-1.0, -1.0) * delta)) *  1.0;
            hr += tex2D(tex, (uv + float2( 1.0, -1.0) * delta)) * -1.0;
            hr += tex2D(tex, (uv + float2(-1.0,  0.0) * delta)) *  2.0;
            hr += tex2D(tex, (uv + float2( 1.0,  0.0) * delta)) * -2.0;
            hr += tex2D(tex, (uv + float2(-1.0,  1.0) * delta)) *  1.0;
            hr += tex2D(tex, (uv + float2( 1.0,  1.0) * delta)) * -1.0;

            vt += tex2D(tex, (uv + float2(-1.0, -1.0) * delta)) *  1.0;
            vt += tex2D(tex, (uv + float2( 0.0, -1.0) * delta)) *  2.0;
            vt += tex2D(tex, (uv + float2( 1.0, -1.0) * delta)) *  1.0;
            vt += tex2D(tex, (uv + float2(-1.0,  1.0) * delta)) * -1.0;
            vt += tex2D(tex, (uv + float2( 0.0,  1.0) * delta)) * -2.0;
            vt += tex2D(tex, (uv + float2( 1.0,  1.0) * delta)) * -1.0;

            return saturate(mult * sqrt(hr * hr + vt * vt));

        }

        fixed4 frag (v2f_img IN) : COLOR
        {
            float depth = 10 * tex2D(_CameraDepthTexture, IN.uv);

            float s = 1 - depth * saturate(Sobel(_CameraDepthTexture, IN.uv, 10));

            return tex2D(_MainTex, IN.uv) * float4(s,s,s,1);
        }
        ENDCG

        Pass {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
        
    }
    FallBack "Diffuse"
}
