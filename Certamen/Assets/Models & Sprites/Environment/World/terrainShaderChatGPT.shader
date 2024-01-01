Shader "Custom/PerlinNoiseWithGradient" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Scale ("Scale", Range(0.1, 10.0)) = 1.0
        _PerlinNoise ("Perlin Noise", 2D) = "white" { }
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        struct Input {
            float2 uv_MainTex;
        };

        sampler2D _PerlinNoise;

        fixed4 _Color;
        float _Scale;

        void surf (Input IN, inout SurfaceOutput o) {
            // Perlin noise coordinates
            float2 p = IN.uv_MainTex * _Scale;

            // Sample Perlin noise
            float noise = tex2D(_PerlinNoise, p).r;

            // Apply color and noise to albedo
            o.Albedo = _Color.rgb * noise;
            o.Alpha = _Color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}