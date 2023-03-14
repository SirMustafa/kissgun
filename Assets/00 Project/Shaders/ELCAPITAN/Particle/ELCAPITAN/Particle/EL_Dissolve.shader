// Toony Colors Pro+Mobile 2
// (c) 2014-2021 Jean Moreno

Shader "ELCAPITAN/Particle/EL_Dissolve"
{
	Properties
	{
		[Enum(Front, 2, Back, 1, Both, 0)] _Cull ("Render Face", Float) = 2.0
		[TCP2ToggleNoKeyword] _ZWrite ("Depth Write", Float) = 1.0
		[HideInInspector] _RenderingMode ("rendering mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("blending source", Float) = 1.0
		[HideInInspector] _DstBlend ("blending destination", Float) = 0.0
		[TCP2Separator]

		[TCP2HeaderHelp(Base)]
		_BaseColor ("Color", Color) = (1,1,1,1)
		[TCP2ColorNoAlpha] _HColor ("Highlight Color", Color) = (0.75,0.75,0.75,1)
		[TCP2ColorNoAlpha] _SColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
		[HideInInspector] __BeginGroup_ShadowHSV ("Shadow HSV", Float) = 0
		_Shadow_HSV_H ("Hue", Range(-180,180)) = 0
		_Shadow_HSV_S ("Saturation", Range(-1,1)) = 0
		_Shadow_HSV_V ("Value", Range(-1,1)) = 0
		[HideInInspector] __EndGroup ("Shadow HSV", Float) = 0
		_BaseMap ("Albedo", 2D) = "white" {}
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
		
		[TCP2HeaderHelp(Specular)]
		[TCP2ColorNoAlpha] _SpecularColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
		_SpecularShadowAttenuation ("Specular Shadow Attenuation", Float) = 0.25
		_SpecularToonSize ("Toon Size", Range(0,1)) = 0.25
		_SpecularToonSmoothness ("Toon Smoothness", Range(0.001,0.5)) = 0.05
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
		
		_StylizedThreshold ("Stylized Threshold", 2D) = "gray" {}
		[TCP2Separator]
		
		[TCP2HeaderHelp(Dissolve)]
		_DissolveMap ("Map", 2D) = "gray" {}
		[TCP2Vector4FloatsDrawer(Speed,Amplitude,Frequency,Offset)] _DissolveMap_SinAnimParams ("Map UV Sine Distortion Parameters", Float) = (1, 0.05, 1, 0)
		[HDR] _DissolveGradientTexture ("Gradient Texture", Color) = (1,1,1,1)
		_DissolveGradientWidth ("Ramp Width", Range(0,1)) = 0.2
		[TCP2Separator]
		
		[TCP2Vector4Floats(Contrast X,Contrast Y,Contrast Z,Smoothing,1,16,1,16,1,16,0.05,10)] _TriplanarSamplingStrength ("Triplanar Sampling Parameters", Vector) = (8,8,8,0.5)
		// Custom Material Properties
		 _DissolveValue ("Dissolve Value", Range(0,1)) = 0.01
		 _MyColor1 ("Color_1", Color) = (1,1,1,1)

		[TCP2HeaderHelp(Curved World 2020)]
		[CurvedWorldBendSettings] _CurvedWorldBendSettings("0,2|1,2|1", Vector) = (0, 0, 0, 0)

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
			"Queue"="AlphaTest"
		}

		HLSLINCLUDE
		#define fixed half
		#define fixed2 half2
		#define fixed3 half3
		#define fixed4 half4

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

		// Uniforms

		// Custom Material Properties
		
		// Shader Properties
		sampler2D _BaseMap;
		sampler2D _DissolveMap;
		sampler2D _StylizedThreshold;

		CBUFFER_START(UnityPerMaterial)
			
			// Custom Material Properties
			float _DissolveValue;
			fixed4 _MyColor1;

			// Shader Properties
			float4 _BaseMap_ST;
			float4 _DissolveMap_ST;
			half4 _DissolveMap_SinAnimParams;
			float _DissolveGradientWidth;
			half4 _DissolveGradientTexture;
			float _HSV_H;
			float _HSV_S;
			float _HSV_V;
			fixed4 _BaseColor;
			float4 _StylizedThreshold_ST;
			float _RampThreshold;
			float _RampSmoothing;
			float _SpecularToonSize;
			float _SpecularToonSmoothness;
			float _SpecularShadowAttenuation;
			fixed4 _SpecularColor;
			float _Shadow_HSV_H;
			float _Shadow_HSV_S;
			float _Shadow_HSV_V;
			fixed4 _SColor;
			fixed4 _HColor;
			float4 _TriplanarSamplingStrength;
			samplerCUBE _AmbientCube;
			fixed4 _TCP2_AMBIENT_RIGHT;
			fixed4 _TCP2_AMBIENT_LEFT;
			fixed4 _TCP2_AMBIENT_TOP;
			fixed4 _TCP2_AMBIENT_BOTTOM;
			fixed4 _TCP2_AMBIENT_FRONT;
			fixed4 _TCP2_AMBIENT_BACK;
		CBUFFER_END

		float4 tex2D_triplanar(sampler2D samp, float4 tiling_offset, float3 worldPos, float3 worldNormal)
		{
			half4 sample_y = ( tex2D(samp, worldPos.xz * tiling_offset.xy + tiling_offset.zw).rgba );
			fixed4 sample_x = ( tex2D(samp, worldPos.zy * tiling_offset.xy + tiling_offset.zw).rgba );
			fixed4 sample_z = ( tex2D(samp, worldPos.xy * tiling_offset.xy + tiling_offset.zw).rgba );
			
			//blending
			half3 blendWeights = pow(abs(worldNormal), _TriplanarSamplingStrength.xyz / _TriplanarSamplingStrength.w);
			blendWeights = blendWeights / (blendWeights.x + abs(blendWeights.y) + blendWeights.z);
			half4 triplanar = sample_x * blendWeights.x + sample_y * blendWeights.y + sample_z * blendWeights.z;
			
			return triplanar;
		}
			
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

		Pass
		{
			Name "Main"
			Tags
			{
				"LightMode"="UniversalForward"
			}
		Blend [_SrcBlend] [_DstBlend]
		Cull [_Cull]
		ZWrite [_ZWrite]

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
			#pragma multi_compile _ _SCREEN_SPACE_OCCLUSION

			// -------------------------------------
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile_fog

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing

			#pragma vertex Vertex
			#pragma fragment Fragment

			//--------------------------------------
			// Toony Colors Pro 2 keywords
		#pragma shader_feature _ _ALPHAPREMULTIPLY_ON

			// vertex input
			struct Attributes
			{
				float4 vertex       : POSITION;
				float3 normal       : NORMAL;
				float4 tangent      : TANGENT;
				float2 uvLM         : TEXCOORD1;
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
				float4 vertexColor : TEXCOORD3;
				float4 pack1 : TEXCOORD4; /* pack1.xyz = objPos  pack1.w = fogFactor */
				float3 pack2 : TEXCOORD5; /* pack2.xyz = objNormal */
				float4 pack3 : TEXCOORD6; /* pack3.xy = texcoord0  pack3.zw = uvLM */
				float2 pack4 : TEXCOORD7; /* pack4.xy = sinUvAnimVertexPos */
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
				output.pack4.xy = sinUvAnimVertexPos;

				// Texture Coordinates
				output.pack3.xy.xy = input.texcoord0.xy * _BaseMap_ST.xy + _BaseMap_ST.zw;
				output.pack3.zw = input.uvLM.xy * unity_LightmapST.xy + unity_LightmapST.zw;

				#if defined(CURVEDWORLD_IS_INSTALLED) && !defined(CURVEDWORLD_DISABLED_ON)
					#ifdef CURVEDWORLD_NORMAL_TRANSFORMATION_ON
						CURVEDWORLD_TRANSFORM_VERTEX_AND_NORMAL(input.vertex, input.normal, input.tangent)
					#else
						CURVEDWORLD_TRANSFORM_VERTEX(input.vertex)
					#endif
				#endif

				float3 worldPos = mul(unity_ObjectToWorld, input.vertex).xyz;
				worldPos.xyz = ( worldPos.xyz + pow(_DissolveValue, 2) * input.vertex.y * float3(0,1,0) * _MyColor1.rgb );
				input.vertex.xyz = mul(unity_WorldToObject, float4(worldPos, 1)).xyz;
				output.pack1.xyz = input.vertex.xyz;
				output.pack2.xyz = input.normal.xyz;
				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);
			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				output.shadowCoord = GetShadowCoord(vertexInput);
			#endif

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

				output.vertexColor = input.vertexColor;

				return output;
			}

			half4 Fragment(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float3 positionWS = input.worldPosAndFog.xyz;
				float3 normalWS = NormalizeNormalPerPixel(input.normal);
				half3 viewDirWS = SafeNormalize(GetCameraPositionWS() - positionWS);

				float2 uvSinAnim__DissolveMap = (input.pack4.xy * _DissolveMap_SinAnimParams.z) + (_Time.yy * _DissolveMap_SinAnimParams.x);
				// Shader Properties Sampling
				float4 __albedo = ( tex2D(_BaseMap, input.pack3.xy.xy).rgba * input.vertexColor.rgba );
				float4 __mainColor = ( _BaseColor.rgba );
				float __alpha = ( __albedo.a * __mainColor.a );
				float __dissolveMap = ( tex2D_triplanar(_DissolveMap, float4(_DissolveMap_ST.xy, _DissolveMap_ST.zw + (((sin(0.9 * uvSinAnim__DissolveMap + _DissolveMap_SinAnimParams.w) + sin(1.33 * uvSinAnim__DissolveMap + 3.14 * _DissolveMap_SinAnimParams.w) + sin(2.4 * uvSinAnim__DissolveMap + 5.3 * _DissolveMap_SinAnimParams.w)) / 3) * _DissolveMap_SinAnimParams.y)), input.pack1.xyz, input.pack2.xyz) );
				float __dissolveValue = ( _DissolveValue.x );
				float __dissolveGradientWidth = ( _DissolveGradientWidth );
				float __dissolveGradientStrength = ( 2.0 );
				float __albedoHue = ( _HSV_H );
				float __albedoSaturation = ( _HSV_S );
				float __albedoValue = ( _HSV_V );
				float __albedoHsvMask = ( __albedo.a );
				float __ambientIntensity = ( 1.0 );
				float __stylizedThreshold = ( tex2D(_StylizedThreshold, input.pack3.xy.xy * _StylizedThreshold_ST.xy + _StylizedThreshold_ST.zw).a );
				float __stylizedThresholdScale = ( 1.0 );
				float __rampThreshold = ( _RampThreshold );
				float __rampSmoothing = ( _RampSmoothing );
				float __specularToonSize = ( _SpecularToonSize );
				float __specularToonSmoothness = ( _SpecularToonSmoothness );
				float __specularShadowAttenuation = ( _SpecularShadowAttenuation );
				float3 __specularColor = ( _SpecularColor.rgb );
				float3 __maxLightIntensity = ( float3(1,1,1) );
				float __shadowHue = ( _Shadow_HSV_H );
				float __shadowSaturation = ( _Shadow_HSV_S );
				float __shadowValue = ( _Shadow_HSV_V );
				float3 __shadowColor = ( _SColor.rgb );
				float3 __highlightColor = ( _HColor.rgb );

				// main texture
				half3 albedo = __albedo.rgb;
				half alpha = __alpha;
				half3 emission = half3(0,0,0);
				
				//Dissolve
				half dissolveMap = __dissolveMap;
				half dissolveValue = __dissolveValue;
				half gradientWidth = __dissolveGradientWidth;
				float dissValue = dissolveValue*(1+2*gradientWidth) - gradientWidth;
				float dissolveUV = smoothstep(dissolveMap - gradientWidth, dissolveMap + gradientWidth, dissValue);
				clip((1-dissolveUV) - 0.001);
				half4 dissolveColor = ( _DissolveGradientTexture.rgba * _MyColor1.rgba );
				dissolveColor *= __dissolveGradientStrength * dissolveUV;
				emission += dissolveColor.rgb;
				
				//Albedo HSV
				float3 albedoHSV = ApplyHSV_3(albedo, __albedoHue, __albedoSaturation, __albedoValue);
				albedo = lerp(albedo, albedoHSV, __albedoHsvMask);
				
				albedo *= __mainColor.rgb;

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
			#ifdef LIGHTMAP_ON
				// Normal is required in case Directional lightmaps are baked
				half3 bakedGI = SampleLightmap(input.pack3.zw, normalWS);
				MixRealtimeAndBakedGI(mainLight, normalWS, bakedGI, half4(0, 0, 0, 0));
			#else
				half3 bakedGI = half3(0,0,0);
			#endif
				half occlusion = 1;

			#if defined(_SCREEN_SPACE_OCCLUSION)
				float2 normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
				AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(normalizedScreenSpaceUV);
				occlusion = min(occlusion, aoFactor.indirectAmbientOcclusion);
			#endif

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

				half3 color = half3(0,0,0);
				half3 accumulatedRamp = ramp * max(lightColor.r, max(lightColor.g, lightColor.b));
				half3 accumulatedColors = ramp * lightColor.rgb;

				//Blinn-Phong Specular
				half3 h = normalize(lightDir + viewDirWS);
				float ndh = max(0, dot (normalWS, h));
				float spec = smoothstep(__specularToonSize + __specularToonSmoothness, __specularToonSize - __specularToonSmoothness,1 - (ndh / (1+__specularToonSmoothness)));
				spec *= ndl;
				spec *= saturate(atten * ndl + __specularShadowAttenuation);
				
				//Apply specular
				emission.rgb += spec * lightColor.rgb * __specularColor;

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

					accumulatedRamp += ramp * max(lightColor.r, max(lightColor.g, lightColor.b));
					accumulatedColors += ramp * lightColor.rgb;

					//Blinn-Phong Specular
					half3 h = normalize(lightDir + viewDirWS);
					float ndh = max(0, dot (normalWS, h));
					float spec = smoothstep(__specularToonSize + __specularToonSmoothness, __specularToonSize - __specularToonSmoothness,1 - (ndh / (1+__specularToonSmoothness)));
					spec *= ndl;
					spec *= saturate(atten * ndl + __specularShadowAttenuation);
					
					//Apply specular
					emission.rgb += spec * lightColor.rgb * __specularColor;
				}
			#endif
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				color += input.vertexLights * albedo;
			#endif

				accumulatedRamp = saturate(accumulatedRamp);
				accumulatedColors = min(accumulatedColors, __maxLightIntensity);
				
				//Shadow HSV
				float3 albedoShadowHSV = ApplyHSV_3(albedo, __shadowHue, __shadowSaturation, __shadowValue);
				albedo = lerp(albedoShadowHSV, albedo, accumulatedRamp);
				half3 shadowColor = (1 - accumulatedRamp.rgb) * __shadowColor;
				accumulatedRamp = accumulatedColors.rgb * __highlightColor + shadowColor;
				color += albedo * accumulatedRamp;

				// apply ambient
				color += indirectDiffuse;

				// Premultiply blending
				#if defined(_ALPHAPREMULTIPLY_ON)
					color.rgb *= alpha;
				#endif

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
#pragma shader_feature_local CURVEDWORLD_BEND_ID_1 CURVEDWORLD_BEND_ID_2
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
				float4 vertexColor : TEXCOORD1;
				float3 pack1 : TEXCOORD2; /* pack1.xyz = positionWS */
				float3 pack2 : TEXCOORD3; /* pack2.xyz = objPos */
				float3 pack3 : TEXCOORD4; /* pack3.xyz = objNormal */
				float4 pack4 : TEXCOORD5; /* pack4.xy = texcoord0  pack4.zw = sinUvAnimVertexPos */
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
				output.pack4.zw = sinUvAnimVertexPos;

				// Texture Coordinates
				output.pack4.xy.xy = input.texcoord0.xy * _BaseMap_ST.xy + _BaseMap_ST.zw;

				#if defined(CURVEDWORLD_IS_INSTALLED) && !defined(CURVEDWORLD_DISABLED_ON)
					#ifdef CURVEDWORLD_NORMAL_TRANSFORMATION_ON
						CURVEDWORLD_TRANSFORM_VERTEX_AND_NORMAL(input.vertex, input.normal, input.tangent)
					#else
						CURVEDWORLD_TRANSFORM_VERTEX(input.vertex)
					#endif
				#endif

				float3 worldPos = mul(unity_ObjectToWorld, input.vertex).xyz;
				worldPos.xyz = ( worldPos.xyz + pow(_DissolveValue, 2) * input.vertex.y * float3(0,1,0) * _MyColor1.rgb );
				input.vertex.xyz = mul(unity_WorldToObject, float4(worldPos, 1)).xyz;
				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);
				output.vertexColor = input.vertexColor;
				output.normal = NormalizeNormalPerVertex(worldNormalUv);
				output.pack1.xyz = vertexInput.positionWS;
				output.pack2.xyz = input.vertex.xyz;
				output.pack3.xyz = input.normal.xyz;

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

				float2 uvSinAnim__DissolveMap = (input.pack4.zw * _DissolveMap_SinAnimParams.z) + (_Time.yy * _DissolveMap_SinAnimParams.x);
				// Shader Properties Sampling
				float4 __albedo = ( tex2D(_BaseMap, input.pack4.xy.xy).rgba * input.vertexColor.rgba );
				float4 __mainColor = ( _BaseColor.rgba );
				float __alpha = ( __albedo.a * __mainColor.a );
				float __dissolveMap = ( tex2D_triplanar(_DissolveMap, float4(_DissolveMap_ST.xy, _DissolveMap_ST.zw + (((sin(0.9 * uvSinAnim__DissolveMap + _DissolveMap_SinAnimParams.w) + sin(1.33 * uvSinAnim__DissolveMap + 3.14 * _DissolveMap_SinAnimParams.w) + sin(2.4 * uvSinAnim__DissolveMap + 5.3 * _DissolveMap_SinAnimParams.w)) / 3) * _DissolveMap_SinAnimParams.y)), input.pack2.xyz, input.pack3.xyz) );
				float __dissolveValue = ( _DissolveValue.x );
				float __dissolveGradientWidth = ( _DissolveGradientWidth );
				float __dissolveGradientStrength = ( 2.0 );

				half3 viewDirWS = SafeNormalize(GetCameraPositionWS() - positionWS);
				half3 albedo = __albedo.rgb;
				half alpha = __alpha;
				half3 emission = half3(0,0,0);
				
				//Dissolve
				half dissolveMap = __dissolveMap;
				half dissolveValue = __dissolveValue;
				half gradientWidth = __dissolveGradientWidth;
				float dissValue = dissolveValue*(1+2*gradientWidth) - gradientWidth;
				float dissolveUV = smoothstep(dissolveMap - gradientWidth, dissolveMap + gradientWidth, dissValue);
				clip((1-dissolveUV) - 0.001);
				half4 dissolveColor = ( _DissolveGradientTexture.rgba * _MyColor1.rgba );
				dissolveColor *= __dissolveGradientStrength * dissolveUV;
				emission += dissolveColor.rgb;

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

		Pass
		{
			Name "DepthOnly"
			Tags
			{
				"LightMode" = "DepthOnly"
			}

			ZWrite On
			ColorMask 0

			HLSLPROGRAM

			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing

			// using simple #define doesn't work, we have to use this instead
			#pragma multi_compile DEPTH_ONLY_PASS

			#pragma vertex ShadowDepthPassVertex
			#pragma fragment ShadowDepthPassFragment

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
#pragma shader_feature_local CURVEDWORLD_BEND_ID_1 CURVEDWORLD_BEND_ID_2
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
#pragma shader_feature_local CURVEDWORLD_BEND_ID_1 CURVEDWORLD_BEND_ID_2
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

/* TCP_DATA u config(unity:"2020.3.2f1";ver:"2.7.0";tmplt:"SG2_Template_URP";features:list["UNITY_5_4","UNITY_5_5","UNITY_5_6","UNITY_2017_1","UNITY_2018_1","UNITY_2018_2","UNITY_2018_3","UNITY_2019_1","UNITY_2019_2","UNITY_2019_3","DISSOLVE","DISSOLVE_CLIP","CURVED_WORLD_2020","ALBEDO_HSV","SILHOUETTE_URP_FEATURE","DISSOLVE_GRADIENT","SPEC_LEGACY","SPECULAR","SPECULAR_TOON","SPECULAR_NO_ATTEN","CLAMP_LIGHTS_INTENSITY","SHADOW_HSV","NO_AMBIENT","CUBE_AMBIENT","DIRAMBIENT","ALBEDO_HSV_MASK","TEXTURED_THRESHOLD","AUTO_TRANSPARENT_BLENDING","SSAO","FOG","ENABLE_LIGHTMAP","TEMPLATE_LWRP"];flags:list[];flags_extra:dict[];keywords:dict[RENDER_TYPE="Opaque",RampTextureDrawer="[TCP2Gradient]",RampTextureLabel="Ramp Texture",SHADER_TARGET="3.0",CURVED_WORLD_2020_INCLUDE="Assets/Amazing Assets/Curved World/Shaders/Core"];shaderProperties:list[sp(name:"Albedo";imps:list[imp_mp_texture(uto:True;tov:"";tov_lbl:"";gto:True;sbt:False;scr:False;scv:"";scv_lbl:"";gsc:False;roff:False;goff:False;sin_anm:False;sin_anmv:"";sin_anmv_lbl:"";notile:False;triplanar_local:False;def:"white";locked_uv:False;uv:0;cc:4;chan:"RGBA";mip:-1;mipprop:False;ssuv_vert:False;ssuv_obj:False;uv_type:Texcoord;uv_chan:"XZ";uv_shaderproperty:__NULL__;prop:"_BaseMap";md:"";custom:False;refs:"";guid:"07bb52f4-f321-47fe-a867-253d02bfe99d";op:Multiply;lbl:"Albedo";gpu_inst:False;locked:False;impl_index:0),imp_vcolors(cc:4;chan:"RGBA";guid:"79c57c4b-725d-4f00-a757-2d89b2f68ced";op:Multiply;lbl:"Albedo";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),,,,,,,,,,,,,,,,,,,,,,sp(name:"Dissolve Map";imps:list[imp_mp_texture(uto:True;tov:"";tov_lbl:"";gto:False;sbt:False;scr:False;scv:"";scv_lbl:"";gsc:False;roff:False;goff:False;sin_anm:True;sin_anmv:"";sin_anmv_lbl:"";notile:False;triplanar_local:True;def:"gray";locked_uv:False;uv:6;cc:1;chan:"R";mip:-1;mipprop:False;ssuv_vert:False;ssuv_obj:False;uv_type:Triplanar;uv_chan:"XZ";uv_shaderproperty:__NULL__;prop:"_DissolveMap";md:"";custom:False;refs:"";guid:"56654c9d-cbf5-4042-a885-20fd92d89eaf";op:Multiply;lbl:"Map";gpu_inst:False;locked:False;impl_index:0)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Dissolve Value";imps:list[imp_ct(lct:"_DissolveValue";cc:1;chan:"X";avchan:"X";guid:"db7e3097-4529-4eb8-bf20-bcdc81f9346c";op:Multiply;lbl:"Value";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Dissolve Gradient Texture";imps:list[imp_mp_color(def:RGBA(1, 1, 1, 1);hdr:True;cc:4;chan:"RGBA";prop:"_DissolveGradientTexture";md:"";custom:False;refs:"";guid:"54005ed1-fd37-4093-8e6b-1f1a9e4845ec";op:Multiply;lbl:"Gradient Texture";gpu_inst:False;locked:False;impl_index:-1),imp_ct(lct:"_MyColor1";cc:4;chan:"RGBA";avchan:"RGBA";guid:"56d99bd1-4322-4149-bd6b-1cf31a5c538c";op:Multiply;lbl:"Dissolve Gradient Texture";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),,,,sp(name:"Vertex Position World";imps:list[imp_hook(guid:"5dfd467d-d60c-4c5c-ba42-fdedc34be7b7";op:Multiply;lbl:"worldPos.xyz";gpu_inst:False;locked:False;impl_index:0),imp_customcode(prepend_type:Disabled;prepend_code:"";prepend_file:"";prepend_file_block:"";preprend_params:dict[];code:"+ pow({3}, 2) * input.vertex.y * float3(0,1,0)";guid:"1f110dc7-3043-4139-8347-52fcb08e23ec";op:Multiply;lbl:"Vertex Position World";gpu_inst:False;locked:False;impl_index:-1),imp_ct(lct:"_DissolveValue";cc:3;chan:"XXX";avchan:"X";guid:"37491c83-cdf3-46de-80a6-fd73e229dc43";op:Multiply;lbl:"Vertex Position World";gpu_inst:False;locked:False;impl_index:-1),imp_ct(lct:"_MyColor1";cc:3;chan:"RGB";avchan:"RGBA";guid:"2dbf0bc0-5190-474c-b6dc-8952b6efcc37";op:Multiply;lbl:"Vertex Position World";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False)];customTextures:list[ct(cimp:imp_mp_range(def:0.01;min:0;max:1;prop:"_DissolveValue";md:"";custom:True;refs:"Dissolve Value, Vertex Position (World Space)";guid:"02c89ef3-a25a-4828-89b6-597fe578a8cc";op:Multiply;lbl:"Dissolve Value";gpu_inst:False;locked:False;impl_index:-1);exp:True;uv_exp:False;imp_lbl:"Range"),ct(cimp:imp_mp_color(def:RGBA(1, 1, 1, 1);hdr:False;cc:4;chan:"RGBA";prop:"_MyColor1";md:"";custom:True;refs:"Dissolve Gradient Texture, Vertex Position (World Space)";guid:"16e4746a-b4e6-471f-8605-eaf816897ca1";op:Multiply;lbl:"Color_1";gpu_inst:False;locked:False;impl_index:-1);exp:True;uv_exp:False;imp_lbl:"Color")];codeInjection:codeInjection(injectedFiles:list[];mark:False);matLayers:list[]) */
/* TCP_HASH 1dfe1aef5efe7660682eacf7a467b6ba */
