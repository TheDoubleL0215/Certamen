Shader "Custom/VertexColor" {

    Properties {

    }



    SubShader {

        Tags { "Queue"="Geometry" "RenderType"="Opaque" }

        LOD 100



        CGPROGRAM

        #pragma surface surf Lambert vertex:vert addshadow



        struct Input {

            float4 vertexColor : COLOR;

        };



        void vert (inout appdata_full v, out Input o) {

            o.vertexColor = v.color;

        }



        void surf (Input IN, inout SurfaceOutput o) {

            o.Albedo = IN.vertexColor.rgb;

            o.Alpha = IN.vertexColor.a;

        }

        ENDCG

    }

    FallBack "Diffuse"

}