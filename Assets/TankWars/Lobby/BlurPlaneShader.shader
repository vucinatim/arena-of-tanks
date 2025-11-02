Shader "Custom/BlurPlaneShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurAmount ("Blur Amount", Range(0, 10)) = 1
        _MainTex_TexelSize ("MainTex TexelSize", Vector) = (1, 1, 0, 0)
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        float _BlurAmount;
        float4 _MainTex_TexelSize;

        struct Input {
            float2 uv_MainTex;
        };

        float Gaussian (float x, float sigma) {
            return exp(-x * x / (2.0 * sigma * sigma)) / (sqrt(2.0 * 3.14159265359) * sigma);
        }

        void surf (Input IN, inout SurfaceOutput o) {
            half4 col = tex2D(_MainTex, IN.uv_MainTex);
            float2 uvBlur = IN.uv_MainTex;
            int numBlurPixels = 10;
            float sigma = _BlurAmount;
            float2 blurStep = _MainTex_TexelSize.xy * sigma;
            float weightSum = Gaussian(0, sigma);

            for (int i = -numBlurPixels; i <= numBlurPixels; i++) {
                for (int j = -numBlurPixels; j <= numBlurPixels; j++) {
                    if (i == 0 && j == 0) continue;
                    uvBlur = IN.uv_MainTex + float2(i, j) * blurStep;
                    float weight = Gaussian(sqrt(i * i + j * j), sigma);
                    col += tex2D(_MainTex, uvBlur) * weight;
                    weightSum += weight;
                }
            }

            col /= weightSum;
            o.Albedo = col.rgb;
            o.Alpha = col.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
