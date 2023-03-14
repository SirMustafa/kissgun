// Toony Colors Pro+Mobile 2
// (c) 2014-2021 Jean Moreno

Shader "ELCAPITAN/Hair/EL_VertexColorHair"
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
		_Albedo ("Albedo Texture", 2D) = "white" {}
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
		_LightWrapFactor ("Light Wrap Factor", Range(0,2)) = 0.5
		[TCP2Separator]
		
		[TCP2HeaderHelp(Specular)]
		_AnisotropicTangentShift ("Tangent Shift Texture", 2D) = "gray" {}
		_AnisotropicShift1 ("Shift 1", Float) = 0.2
		_AnisotropicExponent1 ("Exponent 1", Float) = 4
		[TCP2ColorNoAlpha] _SpecularColor1 ("Specular Color 1", Color) = (0.3,0.3,0.3,1)
		[Space]
		_AnisotropicShift2 ("Shift 2", Float) = 0.5
		_AnisotropicExponent2 ("Exponent 2", Float) = 400
		[TCP2ColorNoAlpha] _SpecularColor2 ("Specular Color 2", Color) = (0.8,0.8,0.8,1)
		[Space]
		_SpecularShadowAttenuation ("Specular Shadow Attenuation", Float) = 0.25
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
		
		[TCP2HeaderHelp(Normal Mapping)]
		[NoScaleOffset] _BumpMap ("Normal Map", 2D) = "bump" {}
		_BumpScale ("Scale", Float) = 1
		[TCP2Separator]
		
		_StylizedThreshold ("Stylized Threshold", 2D) = "gray" {}
		[TCP2Separator]
		
		[TCP2HeaderHelp(Curved World 2020)]
		[CurvedWorldBendSettings] _CurvedWorldBendSettings("0|1|1", Vector) = (0, 0, 0, 0)

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
		}

		HLSLINCLUDE
		#define fixed half
		#define fixed2 half2
		#define fixed3 half3
		#define fixed4 half4

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

		// Uniforms

		// Shader Properties
		sampler2D _BumpMap;
		sampler2D _Albedo;
		sampler2D _StylizedThreshold;
		sampler2D _AnisotropicTangentShift;

		CBUFFER_START(UnityPerMaterial)
			
			// Shader Properties
			float _BumpScale;
			float4 _Albedo_ST;
			float _HSV_H;
			float _HSV_S;
			float _HSV_V;
			fixed4 _BaseColor;
			float4 _StylizedThreshold_ST;
			float _LightWrapFactor;
			float _RampThreshold;
			float _RampSmoothing;
			float4 _AnisotropicTangentShift_ST;
			float _AnisotropicShift1;
			float _AnisotropicShift2;
			float _AnisotropicExponent1;
			float _AnisotropicExponent2;
			fixed4 _SpecularColor1;
			fixed4 _SpecularColor2;
			float _SpecularShadowAttenuation;
			float _Shadow_HSV_H;
			float _Shadow_HSV_S;
			float _Shadow_HSV_V;
			fixed4 _SColor;
			fixed4 _HColor;
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
		
		//Specular help functions (from UnityStandardBRDF.cginc)
		inline half3 SpecSafeNormalize(half3 inVec)
		{
			half dp3 = max(0.001f, dot(inVec, inVec));
			return inVec * rsqrt(dp3);
		}
		// Hair Anisotropic Specular
		inline half HairStrandSpecular(float3 tangent, float3 viewDir, float3 lightDir, float exponent)
		{
			half3 halfDir = SpecSafeNormalize(lightDir + viewDir);
			half tdh = dot(tangent, halfDir);
			half sth = sqrt(1.0 - tdh * tdh);
			half dirAtten = smoothstep(-1.0, 0.0, tdh);
			return dirAtten * pow(sth, exponent);
		}
		inline float3 HairShiftTangent(float3 tangent, float3 normal, float shift)
		{
			return normalize(tangent + shift * normal);
		}
		
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
			#pragma shader_feature TCP2_VERTEX_DISPLACEMENT
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
				float4 pack1 : TEXCOORD4; /* pack1.xyz = tangent  pack1.w = fogFactor */
				float3 pack2 : TEXCOORD5; /* pack2.xyz = bitangent */
				float4 pack3 : TEXCOORD6; /* pack3.xy = texcoord0  pack3.zw = uvLM */
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings Vertex(Attributes input)
			{
				Varyings output = (Varyings)0;

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				// Texture Coordinates
				output.pack3.xy.xy = input.texcoord0.xy * _Albedo_ST.xy + _Albedo_ST.zw;
				output.pack3.zw = input.uvLM.xy * unity_LightmapST.xy + unity_LightmapST.zw;

				#if defined(CURVEDWORLD_IS_INSTALLED) && !defined(CURVEDWORLD_DISABLED_ON)
					#ifdef CURVEDWORLD_NORMAL_TRANSFORMATION_ON
						CURVEDWORLD_TRANSFORM_VERTEX_AND_NORMAL(input.vertex, input.normal, input.tangent)
					#else
						CURVEDWORLD_TRANSFORM_VERTEX(input.vertex)
					#endif
				#endif

				float3 worldPos = mul(unity_ObjectToWorld, input.vertex).xyz;
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

				// tangent
				output.pack1.xyz = vertexNormalInput.tangentWS;
				output.pack2.xyz = vertexNormalInput.bitangentWS;

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
				half3 tangentWS = input.pack1.xyz;
				half3 bitangentWS = input.pack2.xyz;
				half3x3 tangentToWorldMatrix = half3x3(tangentWS.xyz, bitangentWS.xyz, normalWS.xyz);

				// Shader Properties Sampling
				float4 __normalMap = ( tex2D(_BumpMap, input.pack3.xy.xy).rgba );
				float __bumpScale = ( _BumpScale );
				float4 __albedo = ( input.vertexColor.rgba * tex2D(_Albedo, input.pack3.xy.xy).rgba );
				float4 __mainColor = ( _BaseColor.rgba );
				float __alpha = ( __albedo.a * __mainColor.a );
				float __albedoHue = ( _HSV_H );
				float __albedoSaturation = ( _HSV_S );
				float __albedoValue = ( _HSV_V );
				float __ambientIntensity = ( 1.0 );
				float __stylizedThreshold = ( tex2D(_StylizedThreshold, input.pack3.xy.xy * _StylizedThreshold_ST.xy + _StylizedThreshold_ST.zw).a );
				float __stylizedThresholdScale = ( 1.0 );
				float __lightWrapFactor = ( _LightWrapFactor );
				float __rampThreshold = ( _RampThreshold );
				float __rampSmoothing = ( _RampSmoothing );
				float __anisotropicTangentShift = ( tex2D(_AnisotropicTangentShift, input.pack3.xy.xy * _AnisotropicTangentShift_ST.xy + _AnisotropicTangentShift_ST.zw).r );
				float __anisotropicShift1 = ( _AnisotropicShift1 );
				float __anisotropicShift2 = ( _AnisotropicShift2 );
				float __tangentShiftStrength1 = ( 1.0 );
				float __tangentShiftStrength2 = ( 1.0 );
				float __anisotropicExponent1 = ( _AnisotropicExponent1 );
				float __anisotropicExponent2 = ( _AnisotropicExponent2 );
				float3 __specularColor1 = ( _SpecularColor1.rgb );
				float3 __specularColor2 = ( _SpecularColor2.rgb );
				float __specularShadowAttenuation = ( _SpecularShadowAttenuation );
				float __shadowHue = ( _Shadow_HSV_H );
				float __shadowSaturation = ( _Shadow_HSV_S );
				float __shadowValue = ( _Shadow_HSV_V );
				float3 __shadowColor = ( _SColor.rgb );
				float3 __highlightColor = ( _HColor.rgb );

				half4 normalMap = half4(0,0,0,0);
				normalMap = __normalMap;
				half3 normalTS = UnpackNormalScale(normalMap, __bumpScale);
				normalWS = normalize( mul(normalTS, tangentToWorldMatrix) );

				half ndv = abs(dot(viewDirWS, normalWS));
				half ndvRaw = ndv;

				// main texture
				half3 albedo = __albedo.rgb;
				half alpha = __alpha;
				half3 emission = half3(0,0,0);
				
				//Albedo HSV
				albedo = ApplyHSV_3(albedo, __albedoHue, __albedoSaturation, __albedoValue);
				
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
				
				// Wrapped Lighting
				half lightWrap = __lightWrapFactor;
				ndl = (ndl + lightWrap) / (1 + lightWrap);
				
				half rampThreshold = __rampThreshold;
				half rampSmooth = __rampSmoothing * 0.5;
				ndl = saturate(ndl);
				ramp = smoothstep(rampThreshold - rampSmooth, rampThreshold + rampSmooth, ndl);

				// apply attenuation
				ramp *= atten;

				half3 color = half3(0,0,0);
				half3 accumulatedRamp = ramp * max(lightColor.r, max(lightColor.g, lightColor.b));
				half3 accumulatedColors = ramp * lightColor.rgb;

				// Hair Anisotropic Specular
				half tangentShift = __anisotropicTangentShift;
				half shift1 = __anisotropicShift1;
				half shift2 = __anisotropicShift2;
				float3 anisoTangent1 = HairShiftTangent(tangentWS, normalWS, shift1 + tangentShift * __tangentShiftStrength1);
				float3 anisoTangent2 = HairShiftTangent(tangentWS, normalWS, shift2 + tangentShift * __tangentShiftStrength2);
				half exp1 = __anisotropicExponent1;
				half exp2 = __anisotropicExponent2;
				half spec1 = HairStrandSpecular(anisoTangent1, viewDirWS, lightDir, exp1);
				half spec2 = HairStrandSpecular(anisoTangent2, viewDirWS, lightDir, exp2);
				half3 specColor1 = spec1 * __specularColor1;
				half3 specColor2 = spec2 * __specularColor2;
				specColor1 *= ndl;
				specColor2 *= ndl;
				half specShadowAtten = saturate(atten * ndl + __specularShadowAttenuation);
				specColor1 *= specShadowAtten;
				specColor2 *= specShadowAtten;
				
				//Apply specular
				emission.rgb += specColor1 + specColor2 * lightColor.rgb;

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
					
					// Wrapped Lighting
					half lightWrap = __lightWrapFactor;
					ndl = (ndl + lightWrap) / (1 + lightWrap);
					
					ndl = saturate(ndl);
					ramp = smoothstep(rampThreshold - rampSmooth, rampThreshold + rampSmooth, ndl);

					// apply attenuation (shadowmaps & point/spot lights attenuation)
					ramp *= atten;

					accumulatedRamp += ramp * max(lightColor.r, max(lightColor.g, lightColor.b));
					accumulatedColors += ramp * lightColor.rgb;

					// Hair Anisotropic Specular
					half tangentShift = __anisotropicTangentShift;
					half shift1 = __anisotropicShift1;
					half shift2 = __anisotropicShift2;
					float3 anisoTangent1 = HairShiftTangent(tangentWS, normalWS, shift1 + tangentShift * __tangentShiftStrength1);
					float3 anisoTangent2 = HairShiftTangent(tangentWS, normalWS, shift2 + tangentShift * __tangentShiftStrength2);
					half exp1 = __anisotropicExponent1;
					half exp2 = __anisotropicExponent2;
					half spec1 = HairStrandSpecular(anisoTangent1, viewDirWS, lightDir, exp1);
					half spec2 = HairStrandSpecular(anisoTangent2, viewDirWS, lightDir, exp2);
					half3 specColor1 = spec1 * __specularColor1;
					half3 specColor2 = spec2 * __specularColor2;
					specColor1 *= ndl;
					specColor2 *= ndl;
					half specShadowAtten = saturate(atten * ndl + __specularShadowAttenuation);
					specColor1 *= specShadowAtten;
					specColor2 *= specShadowAtten;
					
					//Apply specular
					emission.rgb += specColor1 + specColor2 * lightColor.rgb;
				}
			#endif
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				color += input.vertexLights * albedo;
			#endif

				accumulatedRamp = saturate(accumulatedRamp);
				
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
		#define CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_X_POSITIVE
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
				float4 vertexColor : TEXCOORD1;
				float3 pack1 : TEXCOORD2; /* pack1.xyz = positionWS */
				float2 pack2 : TEXCOORD3; /* pack2.xy = texcoord0 */
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

				// Texture Coordinates
				output.pack2.xy.xy = input.texcoord0.xy * _Albedo_ST.xy + _Albedo_ST.zw;

				#if defined(CURVEDWORLD_IS_INSTALLED) && !defined(CURVEDWORLD_DISABLED_ON)
					#ifdef CURVEDWORLD_NORMAL_TRANSFORMATION_ON
						CURVEDWORLD_TRANSFORM_VERTEX_AND_NORMAL(input.vertex, input.normal, input.tangent)
					#else
						CURVEDWORLD_TRANSFORM_VERTEX(input.vertex)
					#endif
				#endif

				float3 worldPos = mul(unity_ObjectToWorld, input.vertex).xyz;
				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);
				output.vertexColor = input.vertexColor;
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

				// Shader Properties Sampling
				float4 __albedo = ( input.vertexColor.rgba * tex2D(_Albedo, input.pack2.xy.xy).rgba );
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

			#define CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_X_POSITIVE
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

			#define CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_X_POSITIVE
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

/* TCP_DATA u config(unity:"2020.3.12f1";ver:"2.7.0";tmplt:"SG2_Template_URP";features:list["UNITY_5_4","UNITY_5_5","UNITY_5_6","UNITY_2017_1","UNITY_2018_1","UNITY_2018_2","UNITY_2018_3","UNITY_2019_1","UNITY_2019_2","UNITY_2019_3","VERTEX_DISP_SHADER_FEATURE","SUBSURFACE_AMB_COLOR","SS_NO_LIGHTCOLOR","SS_MULTIPLICATIVE","MATCAP_PERSPECTIVE_CORRECTION","MATCAP_SHADER_FEATURE","ALBEDO_HSV","SPECULAR_NO_ATTEN","REFL_ROUGH","NO_AMBIENT","CUBE_AMBIENT","DIRAMBIENT","CURVED_WORLD_2020","SKETCH_AMBIENT","SKETCH_SHADER_FEATURE","SILHOUETTE_URP_FEATURE","SILHOUETTE_STENCIL","FOG","ENABLE_LIGHTMAP","SHADOW_HSV","BLEND_TEX1","BLEND_TEX2","BLEND_TEX3","BLEND_TEX4","AUTO_TRANSPARENT_BLENDING","SPECULAR","TEXTURED_THRESHOLD","OUTLINE_CONSTANT_SIZE","VERTICAL_FOG_COLOR","SSAO","WRAPPED_LIGHTING_CUSTOM","RIM_VERTEX","BUMP","BUMP_SCALE","TEMPLATE_LWRP","SPECULAR_ANISOTROPIC_HAIR"];flags:list[];flags_extra:dict[pragma_gpu_instancing=list[]];keywords:dict[RENDER_TYPE="Opaque",RampTextureDrawer="[TCP2Gradient]",RampTextureLabel="Ramp Texture",SHADER_TARGET="3.0",RIM_LABEL="Rim Lighting",VERTEXMOTION_INCLUDE="Assets/VertExmotion/Shaders/VertExmotion.cginc",CURVED_WORLD_2020_INCLUDE="Assets/Amazing Assets/Curved World/Shaders/Core",CURVED_WORLD_INCLUDE="Assets/VacuumShaders/Curved World/Shaders/cginc/CurvedWorld_Base.cginc",BLEND_TEX1_CHNL="r",BLEND_TEX2_CHNL="g",BLEND_TEX3_CHNL="b",BLEND_TEX4_CHNL="a",GPU_INSTANCING_MAX_COUNT_VALUE="50"];shaderProperties:list[sp(name:"Albedo";imps:list[imp_vcolors(cc:4;chan:"RGBA";guid:"af3bbfea-b911-4974-b85a-dbea92a80377";op:Multiply;lbl:"Albedo";gpu_inst:False;locked:False;impl_index:-1),imp_mp_texture(uto:True;tov:"";tov_lbl:"";gto:True;sbt:False;scr:False;scv:"";scv_lbl:"";gsc:False;roff:False;goff:False;sin_anm:False;sin_anmv:"";sin_anmv_lbl:"";notile:False;triplanar_local:False;def:"white";locked_uv:False;uv:0;cc:4;chan:"RGBA";mip:-1;mipprop:False;ssuv_vert:False;ssuv_obj:False;uv_type:Texcoord;uv_chan:"XZ";uv_shaderproperty:__NULL__;prop:"_Albedo";md:"";custom:False;refs:"";guid:"6c2c465b-7acf-4561-8e57-6ef3068e552d";op:Multiply;lbl:"Albedo Texture";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,sp(name:"Vertical Fog Smoothness";imps:list[imp_mp_float(def:0;prop:"_VerticalFogSmoothness";md:"";custom:False;refs:"";guid:"911b7a9b-333c-42a5-be1f-6c011ea20a76";op:Multiply;lbl:"Smoothness";gpu_inst:False;locked:False;impl_index:0)];layers:list[];unlocked:list[];clones:dict[];isClone:False)];customTextures:list[];codeInjection:codeInjection(injectedFiles:list[];mark:False);matLayers:list[]) */
/* TCP_HASH bfa52bac45684f226385e4c68835bcf0 */
