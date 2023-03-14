// Toony Colors Pro+Mobile 2
// (c) 2014-2021 Jean Moreno

Shader "ELCAPITAN/EL_Water"
{
	Properties
	{
		[TCP2HeaderHelp(Base)]
		_BaseColor ("Color", Color) = (1,1,1,1)
		[TCP2ColorNoAlpha] _HColor ("Highlight Color", Color) = (0.75,0.75,0.75,1)
		[TCP2ColorNoAlpha] _SColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
		_BaseMap ("Albedo", 2D) = "white" {}
		[TCP2Vector4FloatsDrawer(Speed,Amplitude,Frequency,Offset)] _BaseMap_SinAnimParams ("Albedo UV Sine Distortion Parameters", Float) = (1, 0.05, 1, 0)
		[TCP2HeaderHelp(Albedo HSV)]
		[HideInInspector] __BeginGroup_AlbedoHSV ("Albedo HSV", Float) = 0
		_HSV_H ("Hue", Range(-180,180)) = 0
		_HSV_S ("Saturation", Range(-1,1)) = 0
		_HSV_V ("Value", Range(-1,1)) = 0
		[HideInInspector] __EndGroup ("Albedo HSV", Float) = 0
		[TCP2Separator]

		[TCP2Header(Ramp Shading)]
		
		_RampThreshold ("Threshold", Range(0.01,1)) = 0.5
		_RampSmoothing ("Smoothing", Range(0.001,1)) = 0.5
		[TCP2Separator]
		
		[TCP2HeaderHelp(Rim Lighting)]
		[TCP2ColorNoAlpha] _RimColor ("Rim Color", Color) = (0.8,0.8,0.8,0.5)
		_RimMin ("Rim Min", Range(0,2)) = 0.5
		_RimMax ("Rim Max", Range(0,2)) = 1
		[TCP2Separator]
		[TCP2HeaderHelp(Ambient Lighting)]
		//AMBIENT CUBEMAP
		_AmbientCube ("Ambient Cubemap", Cube) = "_Skybox" {}
		_TCP2_AMBIENT_RIGHT ("+X (Right)", Color) = (0,0,0,1)
		_TCP2_AMBIENT_LEFT ("-X (Left)", Color) = (0,0,0,1)
		_TCP2_AMBIENT_TOP ("+Y (Top)", Color) = (0,0,0,1)
		_TCP2_AMBIENT_BOTTOM ("-Y (Bottom)", Color) = (0,0,0,1)
		_TCP2_AMBIENT_FRONT ("+Z (Front)", Color) = (0,0,0,1)
		_TCP2_AMBIENT_BACK ("-Z (Back)", Color) = (0,0,0,1)
		[TCP2Separator]
		
		[TCP2HeaderHelp(Vertex Displacement)]
		_DisplacementTex ("Displacement Texture", 2D) = "black" {}
		 _DisplacementStrength ("Displacement Strength", Range(-1,1)) = 0.01
		[TCP2Separator]
		
		_StylizedThreshold ("Stylized Threshold", 2D) = "gray" {}
		[TCP2Separator]
		
		[TCP2ColorNoAlpha] _DiffuseTint ("Diffuse Tint", Color) = (1,0.5,0,1)
		[TCP2Separator]
		
		[TCP2HeaderHelp(Wind)]
		_WindDirection ("Direction", Vector) = (1,0,0,0)
		_WindStrength ("Strength", Range(0,0.2)) = 0.025
		_WindSpeed ("Speed", Range(0,10)) = 2.5
		
		[TCP2HeaderHelp(Vertex Waves Animation)]
		_WavesSpeed ("Speed", Float) = 2
		_WavesHeight ("Height", Float) = 0.1
		_WavesFrequency ("Frequency", Range(0,10)) = 1
		
		[TCP2HeaderHelp(Depth Based Effects)]
		[TCP2ColorNoAlpha] _DepthColor ("Depth Color", Color) = (0,0,1,1)
		[PowerSlider(5.0)] _DepthColorDistance ("Depth Color Distance", Range(0.01,3)) = 0.5
		[PowerSlider(5.0)] _DepthAlphaDistance ("Depth Alpha Distance", Range(0.01,10)) = 0.5
		_DepthAlphaMin ("Depth Alpha Min", Range(0,1)) = 0.5
		_FoamSpread ("Foam Spread", Range(0,5)) = 2
		_FoamStrength ("Foam Strength", Range(0,1)) = 0.8
		_FoamColor ("Foam Color (RGB) Opacity (A)", Color) = (0.9,0.9,0.9,1)
		_FoamTex ("Foam Texture", 2D) = "black" {}
		_FoamSpeed ("Foam Speed", Vector) = (2,2,2,2)
		_FoamSmoothness ("Foam Smoothness", Range(0,0.5)) = 0.02
		
		[TCP2HeaderHelp(Silhouette Pass)]
		_SilhouetteColor ("Silhouette Color", Color) = (0,0,0,0.33)
		[TCP2Separator]

		[TCP2HeaderHelp(Curved World 2020)]
		[CurvedWorldBendSettings] _CurvedWorldBendSettings("0,2|1|1", Vector) = (0, 0, 0, 0)

		[ToggleOff(_RECEIVE_SHADOWS_OFF)] _ReceiveShadowsOff ("Receive Shadows", Float) = 1

		//Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"RenderPipeline" = "UniversalPipeline"
			"RenderType"="Opaque"
			"Queue"="Geometry+10" //Make sure that the objects are rendered later to avoid sorting issues with the transparent silhouette
		}

		HLSLINCLUDE
		#define fixed half
		#define fixed2 half2
		#define fixed3 half3
		#define fixed4 half4

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

		// Uniforms

		// Shader Properties
		sampler2D _DisplacementTex;
		sampler2D _BaseMap;
		sampler2D _FoamTex;
		sampler2D _StylizedThreshold;

		CBUFFER_START(UnityPerMaterial)
			
			// Shader Properties
			float4 _DisplacementTex_ST;
			float _DisplacementStrength;
			float _WindSpeed;
			float4 _WindDirection;
			float _WindStrength;
			float _WavesFrequency;
			float _WavesHeight;
			float _WavesSpeed;
			fixed4 _SilhouetteColor;
			float4 _BaseMap_ST;
			half4 _BaseMap_SinAnimParams;
			float _HSV_H;
			float _HSV_S;
			float _HSV_V;
			fixed4 _BaseColor;
			fixed4 _DepthColor;
			float _DepthColorDistance;
			float _FoamSpread;
			float _FoamStrength;
			fixed4 _FoamColor;
			float4 _FoamSpeed;
			float4 _FoamTex_ST;
			float _FoamSmoothness;
			float _DepthAlphaDistance;
			float _DepthAlphaMin;
			float4 _StylizedThreshold_ST;
			float _RampThreshold;
			float _RampSmoothing;
			fixed4 _SColor;
			fixed4 _HColor;
			fixed4 _DiffuseTint;
			float _RimMin;
			float _RimMax;
			fixed4 _RimColor;
			samplerCUBE _AmbientCube;
			fixed4 _TCP2_AMBIENT_RIGHT;
			fixed4 _TCP2_AMBIENT_LEFT;
			fixed4 _TCP2_AMBIENT_TOP;
			fixed4 _TCP2_AMBIENT_BOTTOM;
			fixed4 _TCP2_AMBIENT_FRONT;
			fixed4 _TCP2_AMBIENT_BACK;
		CBUFFER_END

		//--------------------------------
		// HSV HELPERS
		// source: http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
		
		float3 rgb2hsv(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
			float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
		
			float d = q.x - min(q.w, q.y);
			float e = 1.0e-10;
			return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}
		
		float3 hsv2rgb(float3 c)
		{
			c.g = max(c.g, 0.0); //make sure that saturation value is positive
			float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
			float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
			return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
		}
		
		float3 ApplyHSV_3(float3 color, float h, float s, float v)
		{
			float3 hsv = rgb2hsv(color.rgb);
			hsv += float3(h/360,s,v);
			return hsv2rgb(hsv);
		}
		float3 ApplyHSV_3(float color, float h, float s, float v) { return ApplyHSV_3(color.xxx, h, s ,v); }
		
		float4 ApplyHSV_4(float4 color, float h, float s, float v)
		{
			float3 hsv = rgb2hsv(color.rgb);
			hsv += float3(h/360,s,v);
			return float4(hsv2rgb(hsv), color.a);
		}
		float4 ApplyHSV_4(float color, float h, float s, float v) { return ApplyHSV_4(color.xxxx, h, s, v); }
		
		half3 DirAmbient (half3 normal)
		{
			fixed3 retColor =
				saturate( normal.x * _TCP2_AMBIENT_RIGHT) +
				saturate(-normal.x * _TCP2_AMBIENT_LEFT) +
				saturate( normal.y * _TCP2_AMBIENT_TOP) +
				saturate(-normal.y * _TCP2_AMBIENT_BOTTOM) +
				saturate( normal.z * _TCP2_AMBIENT_FRONT) +
				saturate(-normal.z * _TCP2_AMBIENT_BACK);
			return retColor * 2.0;
		}
		
		// Built-in renderer (CG) to SRP (HLSL) bindings
		#define UnityObjectToClipPos TransformObjectToHClip
		#define _WorldSpaceLightPos0 _MainLightPosition
		
		ENDHLSL

		//Silhouette Pass
		Pass
		{
			Name "Silhouette"
			Tags { "LightMode" = "Silhouette" }
			Tags
			{
			}
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Greater
			ZWrite Off

			HLSLPROGRAM
			#pragma vertex vertex_silhouette
			#pragma fragment fragment_silhouette
			#pragma multi_compile_instancing
			#pragma target 3.0

			struct appdata_sil
			{
				float4 vertex : POSITION;
				float4 texcoord0 : TEXCOORD0;
				fixed4 vertexColor : COLOR;
				float3 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f_sil
			{
				float4 vertex : SV_POSITION;
				float4 screenPosition : TEXCOORD0;
				float3 pack1 : TEXCOORD1; /* pack1.xyz = worldPos */
				float3 pack2 : TEXCOORD2; /* pack2.xyz = worldNormal */
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f_sil vertex_silhouette (appdata_sil v)
			{
				v2f_sil output = (v2f_sil)0;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 worldNormalUv = mul(unity_ObjectToWorld, float4(v.normal, 1.0)).xyz;
				// Shader Properties Sampling
				float3 __vertexDisplacement = ( v.normal.xyz * tex2Dlod(_DisplacementTex, float4(v.texcoord0.xy * _DisplacementTex_ST.xy + _DisplacementTex_ST.zw, 0, 0)).rgb * _DisplacementStrength );
				float __windTimeOffset = ( v.vertexColor.g );
				float __windSpeed = ( _WindSpeed );
				float __windFrequency = ( 1.0 );
				float4 __windSineScale2 = ( float4(2.3,1.7,1.4,1.2) );
				float __windSineStrength2 = ( .6 );
				float4 __windSineScale3 = ( float4(1.3,2.9,2.1,0.8) );
				float __windSineStrength3 = ( .5 );
				float4 __windSineScale4 = ( float4(3.4,2.6,3.1,1.5) );
				float __windSineStrength4 = ( .2 );
				float4 __windSineScale5 = ( float4(1.4,2.3,2.7,1.1) );
				float __windSineStrength5 = ( .4 );
				float4 __windSineScale6 = ( float4(2.9,1.6,3.3,0.9) );
				float __windSineStrength6 = ( .3 );
				float3 __windDirection = ( _WindDirection.xyz );
				float3 __windMask = ( v.vertexColor.rrr );
				float __windStrength = ( _WindStrength );
				float __wavesFrequency = ( _WavesFrequency );
				float __wavesHeight = ( _WavesHeight );
				float __wavesSpeed = ( _WavesSpeed );

				#if defined(CURVEDWORLD_IS_INSTALLED) && !defined(CURVEDWORLD_DISABLED_ON)
					#ifdef CURVEDWORLD_NORMAL_TRANSFORMATION_ON
						CURVEDWORLD_TRANSFORM_VERTEX_AND_NORMAL(v.vertex, v.normal, v.tangent)
					#else
						CURVEDWORLD_TRANSFORM_VERTEX(v.vertex)
					#endif
				#endif

				v.vertex.xyz += __vertexDisplacement;
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				// Wind Animation
				float windTimeOffset = __windTimeOffset;
				float windSpeed = __windSpeed;
				float3 windFrequency = worldPos.xyz * __windFrequency;
				float windPhase = (_Time.y + windTimeOffset) * windSpeed;
				float3 windFactor = sin(windPhase + windFrequency);
				float4 windSin2scale = __windSineScale2;
				float windSin2strength = __windSineStrength2;
				windFactor += sin(windPhase.xxx * windSin2scale.www + windFrequency * windSin2scale.xyz) * windSin2strength;
				float4 windSin3scale = __windSineScale3;
				float windSin3strength = __windSineStrength3;
				windFactor += sin(windPhase.xxx * windSin3scale.www + windFrequency * windSin3scale.xyz) * windSin3strength;
				float4 windSin4scale = __windSineScale4;
				float windSin4strength = __windSineStrength4;
				windFactor += sin(windPhase.xxx * windSin4scale.www + windFrequency * windSin4scale.xyz) * windSin4strength;
				float4 windSin5scale = __windSineScale5;
				float windSin5strength = __windSineStrength5;
				windFactor += sin(windPhase.xxx * windSin5scale.www + windFrequency * windSin5scale.xyz) * windSin5strength;
				float4 windSin6scale = __windSineScale6;
				float windSin6strength = __windSineStrength6;
				windFactor += sin(windPhase.xxx * windSin6scale.www + windFrequency * windSin6scale.xyz) * windSin6strength;
				float3 windDir = normalize(__windDirection);
				float3 windMask = __windMask;
				float windStrength = __windStrength;
				worldPos.xyz += windDir * windFactor * windMask * windStrength;
				v.vertex.xyz = mul(unity_WorldToObject, float4(worldPos, 1)).xyz;
				
				// Vertex water waves
				float _waveFrequency = __wavesFrequency;
				float _waveHeight = __wavesHeight;
				float3 _vertexWavePos = v.vertex.xyz * _waveFrequency;
				float _phase = _Time.y * __wavesSpeed;
				float waveFactorX = sin(_vertexWavePos.x + _phase) * _waveHeight;
				float waveFactorZ = sin(_vertexWavePos.z + _phase) * _waveHeight;
				v.vertex.xyz += v.normal.xyz * (waveFactorX + waveFactorZ);
				output.pack1.xyz = worldPos;
				output.pack2.xyz = worldNormalUv;
				output.vertex = TransformObjectToHClip(v.vertex.xyz);
				float4 clipPos = output.vertex;

				//Screen Position
				float4 screenPos = ComputeScreenPos(clipPos);
				output.screenPosition = screenPos;

				return output;
			}

			half4 fragment_silhouette (v2f_sil input) : SV_Target
			{

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				// Shader Properties Sampling
				float4 __silhouetteColor = ( _SilhouetteColor.rgba );

				return __silhouetteColor;
			}
			ENDHLSL
		}

		Pass
		{
			Name "Main"
			Tags
			{
				"LightMode"="UniversalForward"
			}

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard SRP library
			// All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 3.0

			// -------------------------------------
			// Material keywords
			#pragma shader_feature _ _RECEIVE_SHADOWS_OFF

			// -------------------------------------
			// Universal Render Pipeline keywords
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

			// -------------------------------------
			#pragma multi_compile_fog

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing

			#pragma vertex Vertex
			#pragma fragment Fragment

			// vertex input
			struct Attributes
			{
				float4 vertex       : POSITION;
				float3 normal       : NORMAL;
				float4 tangent      : TANGENT;
				half4 vertexColor   : COLOR;
				float4 texcoord0 : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			// vertex output / fragment input
			struct Varyings
			{
				float4 positionCS     : SV_POSITION;
				float3 normal         : NORMAL;
				float4 worldPosAndFog : TEXCOORD0;
			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord    : TEXCOORD1; // compute shadow coord per-vertex for the main light
			#endif
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				half3 vertexLights : TEXCOORD2;
			#endif
				float4 screenPosition : TEXCOORD3;
				float4 pack1 : TEXCOORD4; /* pack1.xy = texcoord0  pack1.zw = sinUvAnimVertexPos */
				float pack2 : TEXCOORD5; /* pack2.x = fogFactor */
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings Vertex(Attributes input)
			{
				Varyings output = (Varyings)0;

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				// Used for texture UV sine animation
				float2 sinUvAnimVertexPos = input.vertex.xy + input.vertex.yz;
				output.pack1.zw = sinUvAnimVertexPos;

				// Texture Coordinates
				output.pack1.xy.xy = input.texcoord0.xy * _BaseMap_ST.xy + _BaseMap_ST.zw;
				// Shader Properties Sampling
				float3 __vertexDisplacement = ( input.normal.xyz * tex2Dlod(_DisplacementTex, float4(output.pack1.xy.xy * _DisplacementTex_ST.xy + _DisplacementTex_ST.zw, 0, 0)).rgb * _DisplacementStrength );
				float __windTimeOffset = ( input.vertexColor.g );
				float __windSpeed = ( _WindSpeed );
				float __windFrequency = ( 1.0 );
				float4 __windSineScale2 = ( float4(2.3,1.7,1.4,1.2) );
				float __windSineStrength2 = ( .6 );
				float4 __windSineScale3 = ( float4(1.3,2.9,2.1,0.8) );
				float __windSineStrength3 = ( .5 );
				float4 __windSineScale4 = ( float4(3.4,2.6,3.1,1.5) );
				float __windSineStrength4 = ( .2 );
				float4 __windSineScale5 = ( float4(1.4,2.3,2.7,1.1) );
				float __windSineStrength5 = ( .4 );
				float4 __windSineScale6 = ( float4(2.9,1.6,3.3,0.9) );
				float __windSineStrength6 = ( .3 );
				float3 __windDirection = ( _WindDirection.xyz );
				float3 __windMask = ( input.vertexColor.rrr );
				float __windStrength = ( _WindStrength );
				float __wavesFrequency = ( _WavesFrequency );
				float __wavesHeight = ( _WavesHeight );
				float __wavesSpeed = ( _WavesSpeed );

				#if defined(CURVEDWORLD_IS_INSTALLED) && !defined(CURVEDWORLD_DISABLED_ON)
					#ifdef CURVEDWORLD_NORMAL_TRANSFORMATION_ON
						CURVEDWORLD_TRANSFORM_VERTEX_AND_NORMAL(input.vertex, input.normal, input.tangent)
					#else
						CURVEDWORLD_TRANSFORM_VERTEX(input.vertex)
					#endif
				#endif

				input.vertex.xyz += __vertexDisplacement;
				float3 worldPos = mul(unity_ObjectToWorld, input.vertex).xyz;
				// Wind Animation
				float windTimeOffset = __windTimeOffset;
				float windSpeed = __windSpeed;
				float3 windFrequency = worldPos.xyz * __windFrequency;
				float windPhase = (_Time.y + windTimeOffset) * windSpeed;
				float3 windFactor = sin(windPhase + windFrequency);
				float4 windSin2scale = __windSineScale2;
				float windSin2strength = __windSineStrength2;
				windFactor += sin(windPhase.xxx * windSin2scale.www + windFrequency * windSin2scale.xyz) * windSin2strength;
				float4 windSin3scale = __windSineScale3;
				float windSin3strength = __windSineStrength3;
				windFactor += sin(windPhase.xxx * windSin3scale.www + windFrequency * windSin3scale.xyz) * windSin3strength;
				float4 windSin4scale = __windSineScale4;
				float windSin4strength = __windSineStrength4;
				windFactor += sin(windPhase.xxx * windSin4scale.www + windFrequency * windSin4scale.xyz) * windSin4strength;
				float4 windSin5scale = __windSineScale5;
				float windSin5strength = __windSineStrength5;
				windFactor += sin(windPhase.xxx * windSin5scale.www + windFrequency * windSin5scale.xyz) * windSin5strength;
				float4 windSin6scale = __windSineScale6;
				float windSin6strength = __windSineStrength6;
				windFactor += sin(windPhase.xxx * windSin6scale.www + windFrequency * windSin6scale.xyz) * windSin6strength;
				float3 windDir = normalize(__windDirection);
				float3 windMask = __windMask;
				float windStrength = __windStrength;
				worldPos.xyz += windDir * windFactor * windMask * windStrength;
				input.vertex.xyz = mul(unity_WorldToObject, float4(worldPos, 1)).xyz;
				
				// Vertex water waves
				float _waveFrequency = __wavesFrequency;
				float _waveHeight = __wavesHeight;
				float3 _vertexWavePos = input.vertex.xyz * _waveFrequency;
				float _phase = _Time.y * __wavesSpeed;
				float waveFactorX = sin(_vertexWavePos.x + _phase) * _waveHeight;
				float waveFactorZ = sin(_vertexWavePos.z + _phase) * _waveHeight;
				input.vertex.xyz += input.normal.xyz * (waveFactorX + waveFactorZ);
				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);
			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				output.shadowCoord = GetShadowCoord(vertexInput);
			#endif
				float4 clipPos = vertexInput.positionCS;

				float4 screenPos = ComputeScreenPos(clipPos);
				output.screenPosition.xyzw = screenPos;

				VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normal, input.tangent);
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				// Vertex lighting
				output.vertexLights = VertexLighting(vertexInput.positionWS, vertexNormalInput.normalWS);
			#endif

				// world position
				output.worldPosAndFog = float4(vertexInput.positionWS.xyz, 0);

				// Computes fog factor per-vertex
				output.worldPosAndFog.w = ComputeFogFactor(vertexInput.positionCS.z);

				// normal
				output.normal = NormalizeNormalPerVertex(vertexNormalInput.normalWS);

				// clip position
				output.positionCS = vertexInput.positionCS;

				return output;
			}

			half4 Fragment(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float3 positionWS = input.worldPosAndFog.xyz;
				float3 normalWS = NormalizeNormalPerPixel(input.normal);
				half3 viewDirWS = SafeNormalize(GetCameraPositionWS() - positionWS);

				float2 uvSinAnim__BaseMap = (input.pack1.zw * _BaseMap_SinAnimParams.z) + (_Time.yy * _BaseMap_SinAnimParams.x);
				// Shader Properties Sampling
				float __depthViewCorrectionBias = ( 2.0 );
				float4 __albedo = ( tex2D(_BaseMap, input.pack1.xy.xy + (((sin(0.9 * uvSinAnim__BaseMap + _BaseMap_SinAnimParams.w) + sin(1.33 * uvSinAnim__BaseMap + 3.14 * _BaseMap_SinAnimParams.w) + sin(2.4 * uvSinAnim__BaseMap + 5.3 * _BaseMap_SinAnimParams.w)) / 3) * _BaseMap_SinAnimParams.y)).aaaa );
				float4 __mainColor = ( _BaseColor.rgba );
				float __alpha = ( __albedo.a * __mainColor.a );
				float __albedoHue = ( _HSV_H );
				float __albedoSaturation = ( _HSV_S );
				float __albedoValue = ( _HSV_V );
				float3 __depthColor = ( _DepthColor.rgb );
				float __depthColorDistance = ( _DepthColorDistance );
				float __foamSpread = ( _FoamSpread );
				float __foamStrength = ( _FoamStrength );
				float4 __foamColor = ( _FoamColor.rgba );
				float4 __foamSpeed = ( _FoamSpeed.xyzw );
				float2 __foamTextureBaseUv = ( input.pack1.xy.xy );
				float __foamMask = ( .0 );
				float __foamSmoothness = ( _FoamSmoothness );
				float __depthAlphaDistance = ( _DepthAlphaDistance );
				float __depthAlphaMin = ( _DepthAlphaMin );
				float __ambientIntensity = ( 1.0 );
				float __stylizedThreshold = ( tex2D(_StylizedThreshold, input.pack1.xy.xy * _StylizedThreshold_ST.xy + _StylizedThreshold_ST.zw).a );
				float __stylizedThresholdScale = ( 1.0 );
				float __rampThreshold = ( _RampThreshold );
				float __rampSmoothing = ( _RampSmoothing );
				float3 __shadowColor = ( _SColor.rgb );
				float3 __highlightColor = ( _HColor.rgb );
				float3 __diffuseTint = ( _DiffuseTint.rgb );
				float __rimMin = ( _RimMin );
				float __rimMax = ( _RimMax );
				float3 __rimColor = ( _RimColor.rgb );
				float __rimStrength = ( 1.0 );

				half ndv = abs(dot(viewDirWS, normalWS));
				half ndvRaw = ndv;

				// Sample depth texture and calculate difference with local depth
				//float sceneDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, input.[[INPUT_VALUE:screenPosition]].xy / input.[[INPUT_VALUE:screenPosition]].w);
				float sceneDepth = SampleSceneDepth(input.screenPosition.xyzw.xy / input.screenPosition.xyzw.w);
				if (unity_OrthoParams.w > 0.0)
				{
					// Orthographic camera
					#if defined(UNITY_REVERSED_Z)
						sceneDepth = 1.0 - sceneDepth;
					#endif
					sceneDepth = (sceneDepth * _ProjectionParams.z) + _ProjectionParams.y;
				}
				else
				{
					// Perspective camera
					sceneDepth = LinearEyeDepth(sceneDepth, _ZBufferParams);
				}
				
				//float localDepth = LinearEyeDepth(worldPos, UNITY_MATRIX_V);
				float localDepth = LinearEyeDepth(input.screenPosition.xyzw.z / input.screenPosition.xyzw.w, _ZBufferParams);
				float depthDiff = abs(sceneDepth - localDepth);
				depthDiff *= ndvRaw * __depthViewCorrectionBias;

				// main texture
				half3 albedo = __albedo.rgb;
				half alpha = __alpha;
				half3 emission = half3(0,0,0);
				
				//Albedo HSV
				albedo = ApplyHSV_3(albedo, __albedoHue, __albedoSaturation, __albedoValue);
				
				albedo *= __mainColor.rgb;
				
				// Depth-based color
				half3 depthColor = __depthColor;
				half3 depthColorDist = __depthColorDistance;
				albedo.rgb = lerp(depthColor, albedo.rgb, saturate(depthColorDist * depthDiff));
				
				// Depth-based water foam
				half foamSpread = __foamSpread;
				half foamStrength = __foamStrength;
				half4 foamColor = __foamColor;
				
				half4 foamSpeed = __foamSpeed;
				float2 foamUV = __foamTextureBaseUv;
				
				float2 foamUV1 = foamUV.xy + _Time.yy * foamSpeed.xy * 0.05;
				half3 foam = ( tex2D(_FoamTex, foamUV1 * _FoamTex_ST.xy + _FoamTex_ST.zw).rgb );
				
				foamUV.xy += _Time.yy * foamSpeed.zw * 0.05;
				half3 foam2 = ( tex2D(_FoamTex, foamUV * _FoamTex_ST.xy + _FoamTex_ST.zw).rgb );
				
				foam = (foam + foam2) / 2.0;
				float foamDepth = saturate(foamSpread * depthDiff) * (1.0 - __foamMask);
				half foamSmooth = __foamSmoothness;
				half foamTerm = (smoothstep(foam.r - foamSmooth, foam.r + foamSmooth, saturate(foamStrength - foamDepth)) * saturate(1 - foamDepth)) * foamColor.a;
				albedo.rgb = lerp(albedo.rgb, foamColor.rgb, foamTerm);
				alpha = lerp(alpha, foamColor.a, foamTerm);
				
				// Depth-based alpha
				alpha *= saturate((__depthAlphaDistance * depthDiff) + __depthAlphaMin);

				// main light: direction, color, distanceAttenuation, shadowAttenuation
			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord = input.shadowCoord;
			#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
				float4 shadowCoord = TransformWorldToShadowCoord(positionWS);
			#else
				float4 shadowCoord = float4(0, 0, 0, 0);
			#endif
				Light mainLight = GetMainLight(shadowCoord);

				// ambient or lightmap
				half3 bakedGI = half3(0,0,0);
				half occlusion = 1;

				half3 indirectDiffuse = bakedGI;
				
				//Ambient Cubemap
				indirectDiffuse.rgb += texCUBE(_AmbientCube, normalWS);
				
				//Directional Ambient
				indirectDiffuse.rgb += DirAmbient(normalWS);
				indirectDiffuse *= occlusion * albedo * __ambientIntensity;

				half3 lightDir = mainLight.direction;
				half3 lightColor = mainLight.color.rgb;

				half atten = mainLight.shadowAttenuation * mainLight.distanceAttenuation;

				half ndl = dot(normalWS, lightDir);
				float stylizedThreshold = __stylizedThreshold;
				stylizedThreshold -= 0.5;
				stylizedThreshold *= __stylizedThresholdScale;
				ndl += stylizedThreshold;
				half3 ramp;
				
				half rampThreshold = __rampThreshold;
				half rampSmooth = __rampSmoothing * 0.5;
				ndl = saturate(ndl);
				ramp = smoothstep(rampThreshold - rampSmooth, rampThreshold + rampSmooth, ndl);

				// apply attenuation
				ramp *= atten;

				// highlight/shadow colors
				ramp = lerp(__shadowColor, __highlightColor, ramp);
				
				// Diffuse Tint
				half3 diffuseTint = saturate(__diffuseTint + ndl);
				ramp *= diffuseTint;
				
				// output color
				half3 color = half3(0,0,0);
				// Rim Lighting
				half rim = 1 - ndvRaw;
				rim = ( rim );
				half rimMin = __rimMin;
				half rimMax = __rimMax;
				rim = smoothstep(rimMin, rimMax, rim);
				half3 rimColor = __rimColor;
				half rimStrength = __rimStrength;
				emission.rgb += rim * rimColor * rimStrength;
				color += albedo * lightColor.rgb * ramp;

				// Additional lights loop
			#ifdef _ADDITIONAL_LIGHTS
				uint additionalLightsCount = GetAdditionalLightsCount();
				for (uint lightIndex = 0u; lightIndex < additionalLightsCount; ++lightIndex)
				{
					Light light = GetAdditionalLight(lightIndex, positionWS);
					half atten = light.shadowAttenuation * light.distanceAttenuation;
					half3 lightDir = light.direction;
					half3 lightColor = light.color.rgb;

					half ndl = dot(normalWS, lightDir);
					float stylizedThreshold = __stylizedThreshold;
					stylizedThreshold -= 0.5;
					stylizedThreshold *= __stylizedThresholdScale;
					ndl += stylizedThreshold;
					half3 ramp;
					
					ndl = saturate(ndl);
					ramp = smoothstep(rampThreshold - rampSmooth, rampThreshold + rampSmooth, ndl);

					// apply attenuation (shadowmaps & point/spot lights attenuation)
					ramp *= atten;

					// apply highlight color
					ramp = lerp(half3(0,0,0), __highlightColor, ramp);
					
					// Diffuse Tint
					half3 diffuseTint = saturate(__diffuseTint + ndl);
					ramp *= diffuseTint;
					
					// output color
					color += albedo * lightColor.rgb * ramp;

				}
			#endif
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				color += input.vertexLights * albedo;
			#endif

				// apply ambient
				color += indirectDiffuse;

				color += emission;

				// Mix the pixel color with fogColor. You can optionally use MixFogColor to override the fogColor with a custom one.
				float fogFactor = input.worldPosAndFog.w;
				color = MixFog(color, fogFactor);

				return half4(color, alpha);
			}
			ENDHLSL
		}

		// Depth & Shadow Caster Passes
		HLSLINCLUDE

		// Curved World 2020
#pragma shader_feature_local CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_X_POSITIVE CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_Z_POSITIVE
#define CURVEDWORLD_BEND_ID_1
		#pragma shader_feature_local CURVEDWORLD_DISABLED_ON
		#pragma shader_feature_local CURVEDWORLD_NORMAL_TRANSFORMATION_ON
		#include "Assets/Amazing Assets/Curved World/Shaders/Core/CurvedWorldTransform.cginc"

		#if defined(SHADOW_CASTER_PASS) || defined(DEPTH_ONLY_PASS)

			#define fixed half
			#define fixed2 half2
			#define fixed3 half3
			#define fixed4 half4

			float3 _LightDirection;

			struct Attributes
			{
				float4 vertex   : POSITION;
				float3 normal   : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord0 : TEXCOORD0;
				half4 vertexColor : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS     : SV_POSITION;
				float3 normal         : NORMAL;
				float4 screenPosition : TEXCOORD1;
				float3 pack1 : TEXCOORD2; /* pack1.xyz = positionWS */
				float4 pack2 : TEXCOORD3; /* pack2.xy = texcoord0  pack2.zw = sinUvAnimVertexPos */
			#if defined(DEPTH_ONLY_PASS)
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			#endif
			};

			float4 GetShadowPositionHClip(Attributes input)
			{
				float3 positionWS = TransformObjectToWorld(input.vertex.xyz);
				float3 normalWS = TransformObjectToWorldNormal(input.normal);

				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

			#if UNITY_REVERSED_Z
				positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
			#else
				positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
			#endif

				return positionCS;
			}

			Varyings ShadowDepthPassVertex(Attributes input)
			{
				Varyings output;
				UNITY_SETUP_INSTANCE_ID(input);
				#if defined(DEPTH_ONLY_PASS)
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				#endif

				float3 worldNormalUv = mul(unity_ObjectToWorld, float4(input.normal, 1.0)).xyz;

				// Used for texture UV sine animation
				float2 sinUvAnimVertexPos = input.vertex.xy + input.vertex.yz;
				output.pack2.zw = sinUvAnimVertexPos;

				// Texture Coordinates
				output.pack2.xy.xy = input.texcoord0.xy * _BaseMap_ST.xy + _BaseMap_ST.zw;
				// Shader Properties Sampling
				float3 __vertexDisplacement = ( input.normal.xyz * tex2Dlod(_DisplacementTex, float4(output.pack2.xy.xy * _DisplacementTex_ST.xy + _DisplacementTex_ST.zw, 0, 0)).rgb * _DisplacementStrength );
				float __windTimeOffset = ( input.vertexColor.g );
				float __windSpeed = ( _WindSpeed );
				float __windFrequency = ( 1.0 );
				float4 __windSineScale2 = ( float4(2.3,1.7,1.4,1.2) );
				float __windSineStrength2 = ( .6 );
				float4 __windSineScale3 = ( float4(1.3,2.9,2.1,0.8) );
				float __windSineStrength3 = ( .5 );
				float4 __windSineScale4 = ( float4(3.4,2.6,3.1,1.5) );
				float __windSineStrength4 = ( .2 );
				float4 __windSineScale5 = ( float4(1.4,2.3,2.7,1.1) );
				float __windSineStrength5 = ( .4 );
				float4 __windSineScale6 = ( float4(2.9,1.6,3.3,0.9) );
				float __windSineStrength6 = ( .3 );
				float3 __windDirection = ( _WindDirection.xyz );
				float3 __windMask = ( input.vertexColor.rrr );
				float __windStrength = ( _WindStrength );
				float __wavesFrequency = ( _WavesFrequency );
				float __wavesHeight = ( _WavesHeight );
				float __wavesSpeed = ( _WavesSpeed );

				#if defined(CURVEDWORLD_IS_INSTALLED) && !defined(CURVEDWORLD_DISABLED_ON)
					#ifdef CURVEDWORLD_NORMAL_TRANSFORMATION_ON
						CURVEDWORLD_TRANSFORM_VERTEX_AND_NORMAL(input.vertex, input.normal, input.tangent)
					#else
						CURVEDWORLD_TRANSFORM_VERTEX(input.vertex)
					#endif
				#endif

				input.vertex.xyz += __vertexDisplacement;
				float3 worldPos = mul(unity_ObjectToWorld, input.vertex).xyz;
				// Wind Animation
				float windTimeOffset = __windTimeOffset;
				float windSpeed = __windSpeed;
				float3 windFrequency = worldPos.xyz * __windFrequency;
				float windPhase = (_Time.y + windTimeOffset) * windSpeed;
				float3 windFactor = sin(windPhase + windFrequency);
				float4 windSin2scale = __windSineScale2;
				float windSin2strength = __windSineStrength2;
				windFactor += sin(windPhase.xxx * windSin2scale.www + windFrequency * windSin2scale.xyz) * windSin2strength;
				float4 windSin3scale = __windSineScale3;
				float windSin3strength = __windSineStrength3;
				windFactor += sin(windPhase.xxx * windSin3scale.www + windFrequency * windSin3scale.xyz) * windSin3strength;
				float4 windSin4scale = __windSineScale4;
				float windSin4strength = __windSineStrength4;
				windFactor += sin(windPhase.xxx * windSin4scale.www + windFrequency * windSin4scale.xyz) * windSin4strength;
				float4 windSin5scale = __windSineScale5;
				float windSin5strength = __windSineStrength5;
				windFactor += sin(windPhase.xxx * windSin5scale.www + windFrequency * windSin5scale.xyz) * windSin5strength;
				float4 windSin6scale = __windSineScale6;
				float windSin6strength = __windSineStrength6;
				windFactor += sin(windPhase.xxx * windSin6scale.www + windFrequency * windSin6scale.xyz) * windSin6strength;
				float3 windDir = normalize(__windDirection);
				float3 windMask = __windMask;
				float windStrength = __windStrength;
				worldPos.xyz += windDir * windFactor * windMask * windStrength;
				input.vertex.xyz = mul(unity_WorldToObject, float4(worldPos, 1)).xyz;
				
				// Vertex water waves
				float _waveFrequency = __wavesFrequency;
				float _waveHeight = __wavesHeight;
				float3 _vertexWavePos = input.vertex.xyz * _waveFrequency;
				float _phase = _Time.y * __wavesSpeed;
				float waveFactorX = sin(_vertexWavePos.x + _phase) * _waveHeight;
				float waveFactorZ = sin(_vertexWavePos.z + _phase) * _waveHeight;
				input.vertex.xyz += input.normal.xyz * (waveFactorX + waveFactorZ);
				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);

				//Screen Space UV
				float4 screenPos = ComputeScreenPos(vertexInput.positionCS);
				output.screenPosition.xyzw = screenPos;
				output.normal = NormalizeNormalPerVertex(worldNormalUv);
				output.pack1.xyz = vertexInput.positionWS;

				#if defined(DEPTH_ONLY_PASS)
					output.positionCS = TransformObjectToHClip(input.vertex.xyz);
				#elif defined(SHADOW_CASTER_PASS)
					output.positionCS = GetShadowPositionHClip(input);
				#else
					output.positionCS = float4(0,0,0,0);
				#endif

				return output;
			}

			half4 ShadowDepthPassFragment(Varyings input) : SV_TARGET
			{
				#if defined(DEPTH_ONLY_PASS)
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				#endif

				float3 positionWS = input.pack1.xyz;
				float3 normalWS = NormalizeNormalPerPixel(input.normal);

				float2 uvSinAnim__BaseMap = (input.pack2.zw * _BaseMap_SinAnimParams.z) + (_Time.yy * _BaseMap_SinAnimParams.x);
				// Shader Properties Sampling
				float4 __albedo = ( tex2D(_BaseMap, input.pack2.xy.xy + (((sin(0.9 * uvSinAnim__BaseMap + _BaseMap_SinAnimParams.w) + sin(1.33 * uvSinAnim__BaseMap + 3.14 * _BaseMap_SinAnimParams.w) + sin(2.4 * uvSinAnim__BaseMap + 5.3 * _BaseMap_SinAnimParams.w)) / 3) * _BaseMap_SinAnimParams.y)).aaaa );
				float4 __mainColor = ( _BaseColor.rgba );
				float __alpha = ( __albedo.a * __mainColor.a );

				half3 viewDirWS = SafeNormalize(GetCameraPositionWS() - positionWS);
				half ndv = abs(dot(viewDirWS, normalWS));
				half ndvRaw = ndv;

				half3 albedo = __albedo.rgb;
				half alpha = __alpha;
				half3 emission = half3(0,0,0);

				return 0;
			}

		#endif
		ENDHLSL

		Pass
		{
			Name "ShadowCaster"
			Tags
			{
				"LightMode" = "ShadowCaster"
			}

			ZWrite On
			ZTest LEqual

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0

			// using simple #define doesn't work, we have to use this instead
			#pragma multi_compile SHADOW_CASTER_PASS

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing

			#pragma vertex ShadowDepthPassVertex
			#pragma fragment ShadowDepthPassFragment

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

			ENDHLSL
		}

		// Curved World 2020 - Scene Picking passes
		Pass
		{
			Name "ScenePickingPass"
			Tags { "LightMode" = "Picking" }

			BlendOp Add
			Blend One Zero
			ZWrite On
			Cull Off

			CGPROGRAM

			#include "HLSLSupport.cginc"
			#include "UnityShaderVariables.cginc"
			#include "UnityShaderUtilities.cginc"

			#pragma target 3.0

			#pragma shader_feature _ALPHATEST_ON
			#pragma shader_feature _ALPHAPREMULTIPLY_ON
			#pragma multi_compile_instancing

#pragma shader_feature_local CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_X_POSITIVE CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_Z_POSITIVE
#define CURVEDWORLD_BEND_ID_1
			#pragma shader_feature_local CURVEDWORLD_DISABLED_ON
			#pragma shader_feature_local CURVEDWORLD_NORMAL_TRANSFORMATION_ON
			#include "Assets/Amazing Assets/Curved World/Shaders/Core/CurvedWorldTransform.cginc"

			#pragma vertex vertEditorPass
			#pragma fragment fragScenePickingPass

			#include "Assets/Amazing Assets/Curved World/Shaders/Core/SceneSelection.cginc"

			ENDCG
		}

		Pass
		{
			Name "SceneSelectionPass"
			Tags { "LightMode" = "SceneSelectionPass" }

			BlendOp Add
			Blend One Zero
			ZWrite On
			Cull Off

			CGPROGRAM

			#include "HLSLSupport.cginc"
			#include "UnityShaderVariables.cginc"
			#include "UnityShaderUtilities.cginc"

			#pragma target 3.0

			#pragma shader_feature _ALPHATEST_ON
			#pragma shader_feature _ALPHAPREMULTIPLY_ON
			#pragma multi_compile_instancing

#pragma shader_feature_local CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_X_POSITIVE CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_Z_POSITIVE
#define CURVEDWORLD_BEND_ID_1
			#pragma shader_feature_local CURVEDWORLD_DISABLED_ON
			#pragma shader_feature_local CURVEDWORLD_NORMAL_TRANSFORMATION_ON
			#include "Assets/Amazing Assets/Curved World/Shaders/Core/CurvedWorldTransform.cginc"

			#pragma vertex vertEditorPass
			#pragma fragment fragSceneHighlightPass

			#include "Assets/Amazing Assets/Curved World/Shaders/Core/SceneSelection.cginc"

			ENDCG
		}
	}

	FallBack "Hidden/InternalErrorShader"
	CustomEditor "ToonyColorsPro.ShaderGenerator.MaterialInspector_SG2"
}

/* TCP_DATA u config(unity:"2020.3.2f1";ver:"2.7.0";tmplt:"SG2_Template_URP";features:list["UNITY_5_4","UNITY_5_5","UNITY_5_6","UNITY_2017_1","UNITY_2018_1","UNITY_2018_2","UNITY_2018_3","UNITY_2019_1","UNITY_2019_2","UNITY_2019_3","WIND_ANIM_SIN","WIND_ANIM","FOAM_ANIM","SMOOTH_FOAM","NO_FOAM_BACKFACE","FOG","CURVED_WORLD_2020","VSW_FOLLOWNORM","DISSOLVE_CLIP","DISSOLVE_GRADIENT","SHADOW_COLOR_MAIN_DIR","DEPTH_BUFFER_FOAM","NO_AMBIENT","CUBE_AMBIENT","DIRAMBIENT","ALBEDO_HSV","RIM","VERTEX_SIN_WAVES","DEPTH_BUFFER_COLOR","DEPTH_BUFFER_ALPHA","DEPTH_VIEW_CORRECTION","WIND_SIN_6","PASS_SILHOUETTE","SILHOUETTE_URP_FEATURE","DIFFUSE_TINT","TEXTURED_THRESHOLD","VERTEX_DISPLACEMENT","TEMPLATE_LWRP"];flags:list[];flags_extra:dict[];keywords:dict[RENDER_TYPE="Opaque",RampTextureDrawer="[TCP2Gradient]",RampTextureLabel="Ramp Texture",SHADER_TARGET="3.0",RIM_LABEL="Rim Lighting",CURVED_WORLD_2020_INCLUDE="Assets/Amazing Assets/Curved World/Shaders/Core"];shaderProperties:list[sp(name:"Albedo";imps:list[imp_mp_texture(uto:True;tov:"";tov_lbl:"";gto:True;sbt:False;scr:False;scv:"";scv_lbl:"";gsc:False;roff:False;goff:False;sin_anm:True;sin_anmv:"";sin_anmv_lbl:"";notile:False;triplanar_local:False;def:"white";locked_uv:False;uv:0;cc:4;chan:"AAAA";mip:-1;mipprop:False;ssuv_vert:False;ssuv_obj:False;uv_type:Texcoord;uv_chan:"XZ";uv_shaderproperty:__NULL__;prop:"_BaseMap";md:"";custom:False;refs:"";guid:"be0d8acf-333b-4764-b4ac-0bb7803caff3";op:Multiply;lbl:"Albedo";gpu_inst:False;locked:False;impl_index:0)];layers:list[];unlocked:list[];clones:dict[];isClone:False)];customTextures:list[];codeInjection:codeInjection(injectedFiles:list[];mark:False);matLayers:list[]) */
/* TCP_HASH 9a0757ef73b54f4aa65363509e637417 */
