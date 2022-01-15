Shader "MyCustom/MFS/SumTime"
{
    Properties
    {
		// [NoScaleOffset] _MainTex ("Time Texture", 2D) = "white" {}
		[NoScaleOffset] _SpeedTex ("Speed Texture", 2D) = "white" {}
        _Mask ("Mask", Vector) = (1.0, 0.0, 0.0, 0.0)
        _SpeedMin ("Speed Min", Float) = 0
        _SpeedMax ("Speed Max", Float) = 2
        _ZeroThreshold ("Zero Threshold", Range(0, 1)) = 0.02
        _ResetValue ("Reset Value", Range(0, 1)) = 0
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            Name "SumeTime"

            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"

            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag

            // uniform sampler2D _MainTex;
            uniform sampler2D _SpeedTex;
            uniform float4 _Mask;
            uniform float _SpeedMin;
            uniform float _SpeedMax;
            uniform float _ZeroThreshold;
            uniform float _ResetValue;

            float4 frag(v2f_customrendertexture i) : SV_Target
            {
                // const float zeroThreshold = 0.02;
                const float2 uv = float2(0.5, 0.5);

                float t = tex2D(_SelfTexture2D, uv).r;
                float4 speedTex = tex2D(_SpeedTex, uv);
                speedTex *= _Mask;
                float speed = speedTex.r + speedTex.g + speedTex.b + speedTex.a;
                float s = lerp(_SpeedMin, _SpeedMax, speed);

                // 限りなくゼロに近い場合はTimeを0にリセットする
                // スピードが0のときにもどしたときに自動でTimeを0にする
                // float z = 1.0 - step(-FLT_EPSILON, s) * step(s, FLT_EPSILON);
                float time = frac(t + unity_DeltaTime.x * s);
                float zeroThreshold = max(_ZeroThreshold, 1.192092896e-07);
                float z = 1.0 - step(-zeroThreshold, s) * step(s, zeroThreshold);
                time = lerp(_ResetValue, time, z);

                float4 f = float4(time, time, time, 1.0);
                return f;
            }
            ENDCG
        }
    }
}
