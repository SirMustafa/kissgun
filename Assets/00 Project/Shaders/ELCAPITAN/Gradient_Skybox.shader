﻿Shader "ELCAPITAN/Skybox/Gradient Skybox"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1, 1, 1, 0)
        _Color2 ("Color 2", Color) = (1, 1, 1, 0)
        
        _Intensity ("Intensity", Float) = 1.0
        _Exponent ("Exponent", Float) = 1.0
        
        _DirX ("Direction X", Range(0, 180)) = 0
        _DirY ("Direction Y", Range(0, 180)) = 0
        
        [HideInInspector]_UpVector ("Direction", Vector) = (0, 1, 0, 0)
    }

    SubShader
    {
        Tags {
            "Queue"="Background"
            "RenderType"="Background"
            "PreviewType"="Skybox" 
			}
        Pass
        {
            ZWrite Off
            Cull Off
            CGPROGRAM
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert
            #pragma fragment frag
			#include "UnityCG.cginc"

			half4 _Color1;
			half4 _Color2;
			half4 _UpVector;
			half _Intensity;
			half _Exponent;

			struct appdata
			{
				float4 position : POSITION;
				float3 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};
    
			struct v2f
			{
				float4 position : SV_POSITION;
				float3 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert (appdata v)
			{
				v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.position = UnityObjectToClipPos(v.position);
				o.texcoord = v.texcoord;
				return o;
			}
    
			fixed4 frag (v2f i) : COLOR
			{
                UNITY_SETUP_INSTANCE_ID(i);
				return lerp (_Color1, _Color2, pow((dot (normalize (i.texcoord), _UpVector)/2) + 0.5, _Exponent)) * _Intensity;
			}
            ENDCG
        }
    }
    CustomEditor "ElCapitan.SkyEditor"
}