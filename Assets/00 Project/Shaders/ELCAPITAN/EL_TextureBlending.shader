// Toony Colors Pro+Mobile 2
// (c) 2014-2021 Jean Moreno

Shader "ELCAPITAN/EL_TextureBlending"
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
		_BaseMap ("Albedo", 2D) = "black" {}
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
		[Toggle(TCP2_SPECULAR)] _UseSpecular ("Enable Specular", Float) = 0
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
		
		[TCP2HeaderHelp(Normal Mapping)]
		[NoScaleOffset] _BumpMap ("Normal Map", 2D) = "bump" {}
		_BumpScale ("Scale", Float) = 1
		[NoScaleOffset] _ParallaxMap ("Height Map", 2D) = "black" {}
		_Parallax ("Height", Range(0.005,0.08)) = 0.02
		[TCP2Separator]
		
		[TCP2HeaderHelp(Texture Blending)]
		_BlendTex1 ("Texture 1", 2D) = "white" {}
		[NoScaleOffset] _BlendMap1 ("Normal Map 1", 2D) = "bump" {}
		_BlendTex2 ("Texture 2", 2D) = "white" {}
		[NoScaleOffset] _BlendMap2 ("Normal Map 2", 2D) = "bump" {}
		_BlendTex3 ("Texture 3", 2D) = "white" {}
		[NoScaleOffset] _BlendMap3 ("Normal Map 3", 2D) = "bump" {}
		[TCP2HeaderHelp(Height Blending Parameters)]
		[TCP2Vector4Floats(R,G,B,A,0.001,2,0.001,2,0.001,2,0.001,2)] _VColorBlendSmooth ("Height Smoothing", Vector) = (0.25,0.25,0.25,0.25)
		[TCP2Vector4Floats(R,G,B,A)] _VColorBlendOffset ("Height Offset", Vector) = (0,0,0,0)
		[TCP2HelpBox(Info,Height will be taken from each texture alpha channel.  No alpha in the texture will result in linear blending.)]
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
		sampler2D _ParallaxMap;
		sampler2D _BlendTex1;
		sampler2D _BlendTex2;
		sampler2D _BlendTex3;
		sampler2D _BumpMap;
		sampler2D _BlendMap1;
		sampler2D _BlendMap2;
		sampler2D _BlendMap3;
		sampler2D _BaseMap;

		CBUFFER_START(UnityPerMaterial)
			
			// Shader Properties
			float _Parallax;
			float4 _BlendTex1_ST;
			float4 _BlendTex2_ST;
			float4 _BlendTex3_ST;
			float _BumpScale;
			float4 _BaseMap_ST;
			float _HSV_H;
			float _HSV_S;
			float _HSV_V;
			fixed4 _BaseColor;
			float _RampThreshold;
			float _RampSmoothing;
			float _SpecularToonSize;
			float _SpecularToonSmoothness;
			float _SpecularShadowAttenuation;
			fixed4 _SpecularColor;
			fixed4 _SColor;
			fixed4 _HColor;
			samplerCUBE _AmbientCube;
			fixed4 _TCP2_AMBIENT_RIGHT;
			fixed4 _TCP2_AMBIENT_LEFT;
			fixed4 _TCP2_AMBIENT_TOP;
			fixed4 _TCP2_AMBIENT_BOTTOM;
			fixed4 _TCP2_AMBIENT_FRONT;
			fixed4 _TCP2_AMBIENT_BACK;
			
			//Texture Blending Params
			float4 _VColorBlendSmooth;
			float4 _VColorBlendOffset;
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
		
		// Calculates UV offset for parallax bump mapping
		inline float2 TCP2_ParallaxOffset( half h, half height, half3 viewDir )
		{
			h = h * height - height/2.0;
			float3 v = normalize(viewDir);
			v.z += 0.42;
			return h * (v.xy / v.z);
		}
		
		// Height-based texture blending
		float4 blend_height_smooth(float4 texture1, float height1, float4 texture2, float height2, float smoothing)
		{
			float ma = max(texture1.a + height1, texture2.a + height2) - smoothing;
			float b1 = max(texture1.a + height1 - ma, 0);
			float b2 = max(texture2.a + height2 - ma, 0);
			return (texture1 * b1 + texture2 * b2) / (b1 + b2);
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
			#pragma shader_feature TCP2_SPECULAR
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
				//Parallax Offset
				float __parallaxHeightMap = ( tex2D(_ParallaxMap, input.pack3.xy.xy).a );
				float __parallaxHeight = ( _Parallax );
				half height = __parallaxHeightMap;
				half3 viewDirTS = mul(viewDirWS, tangentToWorldMatrix);
				float2 offset = TCP2_ParallaxOffset(height, __parallaxHeight, viewDirTS);
				input.pack3.xy += offset;

				// Shader Properties Sampling
				float4 __blendingSource = ( input.vertexColor.rgba );
				float __blendingHeightContrast = ( 2.5 );
				float4 __blendTexture1 = ( tex2D(_BlendTex1, input.pack3.xy.xy * _BlendTex1_ST.xy + _BlendTex1_ST.zw).rgba );
				float4 __blendTexture2 = ( tex2D(_BlendTex2, input.pack3.xy.xy * _BlendTex2_ST.xy + _BlendTex2_ST.zw).rgba );
				float4 __blendTexture3 = ( tex2D(_BlendTex3, input.pack3.xy.xy * _BlendTex3_ST.xy + _BlendTex3_ST.zw).rgba );
				float4 __normalMap = ( tex2D(_BumpMap, input.pack3.xy.xy).rgba );
				float4 __blendNormalMap1 = ( tex2D(_BlendMap1, input.pack3.xy.xy * _BlendTex1_ST.xy + _BlendTex1_ST.zw).rgba );
				float4 __blendNormalMap2 = ( tex2D(_BlendMap2, input.pack3.xy.xy * _BlendTex2_ST.xy + _BlendTex2_ST.zw).rgba );
				float4 __blendNormalMap3 = ( tex2D(_BlendMap3, input.pack3.xy.xy * _BlendTex3_ST.xy + _BlendTex3_ST.zw).rgba );
				float __bumpScale = ( _BumpScale );
				float4 __albedo = ( tex2D(_BaseMap, input.pack3.xy.xy).rgba );
				float4 __mainColor = ( _BaseColor.rgba );
				float __alpha = ( __albedo.a * __mainColor.a );
				float __albedoHue = ( _HSV_H );
				float __albedoSaturation = ( _HSV_S );
				float __albedoValue = ( _HSV_V );
				float __ambientIntensity = ( 1.0 );
				float __rampThreshold = ( _RampThreshold );
				float __rampSmoothing = ( _RampSmoothing );
				float __specularToonSize = ( _SpecularToonSize );
				float __specularToonSmoothness = ( _SpecularToonSmoothness );
				float __specularShadowAttenuation = ( _SpecularShadowAttenuation );
				float3 __specularColor = ( _SpecularColor.rgb );
				float3 __maxLightIntensity = ( float3(1,1,1) );
				float3 __shadowColor = ( _SColor.rgb );
				float3 __highlightColor = ( _HColor.rgb );

				// Texture Blending: initialize
				fixed4 blendingSource = __blendingSource;
				float contrast = __blendingHeightContrast;
				float contrast_half = contrast/2;
				fixed4 tex1 = __blendTexture1;
				fixed4 tex2 = __blendTexture2;
				fixed4 tex3 = __blendTexture3;
				half4 normalMap = half4(0,0,0,0);
				normalMap = __normalMap;
				
				// Texture Blending: normal maps
				half4 bump1 = __blendNormalMap1;
				half4 bump2 = __blendNormalMap2;
				half4 bump3 = __blendNormalMap3;
				
				normalMap = lerp(normalMap, blend_height_smooth(normalMap, tex1.a, bump1, blendingSource.r * contrast - contrast_half + tex1.a + _VColorBlendOffset.x, _VColorBlendSmooth.x), saturate(blendingSource.r * contrast_half));
				normalMap = lerp(normalMap, blend_height_smooth(normalMap, tex2.a, bump2, blendingSource.g * contrast - contrast_half + tex2.a + _VColorBlendOffset.y, _VColorBlendSmooth.y), saturate(blendingSource.g * contrast_half));
				normalMap = lerp(normalMap, blend_height_smooth(normalMap, tex3.a, bump3, blendingSource.b * contrast - contrast_half + tex3.a + _VColorBlendOffset.z, _VColorBlendSmooth.z), saturate(blendingSource.b * contrast_half));
				half3 normalTS = UnpackNormalScale(normalMap, __bumpScale);
				normalWS = normalize( mul(normalTS, tangentToWorldMatrix) );

				// main texture
				half3 albedo = __albedo.rgb;
				half alpha = __alpha;
				half3 emission = half3(0,0,0);
				half4 albedoAlpha = half4(albedo, alpha);
				
				// Texture Blending: sample
				albedoAlpha = lerp(albedoAlpha, blend_height_smooth(albedoAlpha, albedoAlpha.a, tex1, blendingSource.r * contrast - contrast_half + tex1.a + _VColorBlendOffset.x, _VColorBlendSmooth.x), saturate(blendingSource.r * contrast_half));
				albedoAlpha = lerp(albedoAlpha, blend_height_smooth(albedoAlpha, albedoAlpha.a, tex2, blendingSource.g * contrast - contrast_half + tex2.a + _VColorBlendOffset.y, _VColorBlendSmooth.y), saturate(blendingSource.g * contrast_half));
				albedoAlpha = lerp(albedoAlpha, blend_height_smooth(albedoAlpha, albedoAlpha.a, tex3, blendingSource.b * contrast - contrast_half + tex3.a + _VColorBlendOffset.z, _VColorBlendSmooth.z), saturate(blendingSource.b * contrast_half));
				albedo = albedoAlpha.rgb;
				alpha = albedoAlpha.a;
				
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

				#if defined(TCP2_SPECULAR)
				//Blinn-Phong Specular
				half3 h = normalize(lightDir + viewDirWS);
				float ndh = max(0, dot (normalWS, h));
				float spec = smoothstep(__specularToonSize + __specularToonSmoothness, __specularToonSize - __specularToonSmoothness,1 - (ndh / (1+__specularToonSmoothness)));
				spec *= ndl;
				spec *= saturate(atten * ndl + __specularShadowAttenuation);
				
				//Apply specular
				emission.rgb += spec * lightColor.rgb * __specularColor;
				#endif

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
					half3 ramp;
					
					ndl = saturate(ndl);
					ramp = smoothstep(rampThreshold - rampSmooth, rampThreshold + rampSmooth, ndl);

					// apply attenuation (shadowmaps & point/spot lights attenuation)
					ramp *= atten;

					accumulatedRamp += ramp * max(lightColor.r, max(lightColor.g, lightColor.b));
					accumulatedColors += ramp * lightColor.rgb;

					#if defined(TCP2_SPECULAR)
					//Blinn-Phong Specular
					half3 h = normalize(lightDir + viewDirWS);
					float ndh = max(0, dot (normalWS, h));
					float spec = smoothstep(__specularToonSize + __specularToonSmoothness, __specularToonSize - __specularToonSmoothness,1 - (ndh / (1+__specularToonSmoothness)));
					spec *= ndl;
					spec *= saturate(atten * ndl + __specularShadowAttenuation);
					
					//Apply specular
					emission.rgb += spec * lightColor.rgb * __specularColor;
					#endif
				}
			#endif
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				color += input.vertexLights * albedo;
			#endif

				accumulatedRamp = saturate(accumulatedRamp);
				accumulatedColors = min(accumulatedColors, __maxLightIntensity);
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
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS     : SV_POSITION;
				float3 normal         : NORMAL;
				float3 pack0 : TEXCOORD1; /* pack0.xyz = positionWS */
				float2 pack1 : TEXCOORD2; /* pack1.xy = texcoord0 */
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
				output.pack1.xy.xy = input.texcoord0.xy * _BaseMap_ST.xy + _BaseMap_ST.zw;

				#if defined(CURVEDWORLD_IS_INSTALLED) && !defined(CURVEDWORLD_DISABLED_ON)
					#ifdef CURVEDWORLD_NORMAL_TRANSFORMATION_ON
						CURVEDWORLD_TRANSFORM_VERTEX_AND_NORMAL(input.vertex, input.normal, input.tangent)
					#else
						CURVEDWORLD_TRANSFORM_VERTEX(input.vertex)
					#endif
				#endif

				float3 worldPos = mul(unity_ObjectToWorld, input.vertex).xyz;
				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);
				output.normal = NormalizeNormalPerVertex(worldNormalUv);
				output.pack0.xyz = vertexInput.positionWS;

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

				float3 positionWS = input.pack0.xyz;
				float3 normalWS = NormalizeNormalPerPixel(input.normal);

				// Shader Properties Sampling
				float4 __albedo = ( tex2D(_BaseMap, input.pack1.xy.xy).rgba );
				float4 __mainColor = ( _BaseColor.rgba );
				float __alpha = ( __albedo.a * __mainColor.a );

				half3 viewDirWS = SafeNormalize(GetCameraPositionWS() - positionWS);
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

/* TCP_DATA u config(unity:"2020.3.2f1";ver:"2.7.0";tmplt:"SG2_Template_URP";features:list["UNITY_5_4","UNITY_5_5","UNITY_5_6","UNITY_2017_1","UNITY_2018_1","UNITY_2018_2","UNITY_2018_3","UNITY_2019_1","UNITY_2019_2","UNITY_2019_3","ALBEDO_HSV","TEXTURE_BLENDING","BLEND_TEX1","BLEND_TEX2","BLEND_TEX3","SSAO","FOG","CURVED_WORLD_2020","TEXBLEND_HEIGHT","TEXBLEND_BUMP","BUMP","BUMP_SCALE","PARALLAX","ENABLE_LIGHTMAP","NO_AMBIENT","CUBE_AMBIENT","DIRAMBIENT","SMOOTH_FOAM","AUTO_TRANSPARENT_BLENDING","CLAMP_LIGHTS_INTENSITY","RIM_LIGHTMASK","RIM_VERTEX","RIM_DIR","RIM_DIR_PERSP_CORRECTION","SPEC_LEGACY","SPECULAR","SPECULAR_TOON","SPECULAR_SHADER_FEATURE","SPECULAR_NO_ATTEN","TEMPLATE_LWRP"];flags:list[];flags_extra:dict[];keywords:dict[RENDER_TYPE="Opaque",RampTextureDrawer="[TCP2Gradient]",RampTextureLabel="Ramp Texture",SHADER_TARGET="3.0",BLEND_TEX1_CHNL="r",BLEND_TEX2_CHNL="g",BLEND_TEX3_CHNL="b",CURVED_WORLD_2020_INCLUDE="Assets/Amazing Assets/Curved World/Shaders/Core",BLEND_TEX4_CHNL="a",RIM_LABEL="Rim Lighting"];shaderProperties:list[sp(name:"Albedo";imps:list[imp_mp_texture(uto:True;tov:"";tov_lbl:"";gto:True;sbt:False;scr:False;scv:"";scv_lbl:"";gsc:False;roff:False;goff:False;sin_anm:False;sin_anmv:"";sin_anmv_lbl:"";notile:False;triplanar_local:False;def:"black";locked_uv:False;uv:0;cc:4;chan:"RGBA";mip:-1;mipprop:False;ssuv_vert:False;ssuv_obj:False;uv_type:Texcoord;uv_chan:"XZ";uv_shaderproperty:__NULL__;prop:"_BaseMap";md:"";custom:False;refs:"";guid:"ffcaca74-6f0a-42c5-9e90-baae577d1cd0";op:Multiply;lbl:"Albedo";gpu_inst:False;locked:False;impl_index:0)];layers:list[];unlocked:list["35f98d","6a81da"];clones:dict[35f98d=sp(name:"Albedo_35f98d";imps:list[imp_mp_color(def:RGBA(1, 1, 1, 1);hdr:True;cc:4;chan:"RGBA";prop:"_Albedo1";md:"";custom:False;refs:"";guid:"da3cfb8f-50dc-4a0d-89ff-683eb722d99b";op:Multiply;lbl:"Albedo";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:True),6a81da=sp(name:"Albedo_6a81da";imps:list[imp_mp_texture(uto:True;tov:"";tov_lbl:"";gto:True;sbt:False;scr:False;scv:"";scv_lbl:"";gsc:False;roff:False;goff:False;sin_anm:False;sin_anmv:"";sin_anmv_lbl:"";notile:False;triplanar_local:False;def:"black";locked_uv:False;uv:0;cc:4;chan:"RGBA";mip:-1;mipprop:False;ssuv_vert:False;ssuv_obj:False;uv_type:Texcoord;uv_chan:"XZ";uv_shaderproperty:__NULL__;prop:"_BaseMap_6a81da";md:"";custom:False;refs:"";guid:"00000000-0000-0000-0000-000000000000";op:Multiply;lbl:"Albedo";gpu_inst:False;locked:False;impl_index:0)];layers:list[];unlocked:list[];clones:dict[];isClone:True)];isClone:False),,,,,,,,,,,,,,,,,,,,sp(name:"Blending Source";imps:list[imp_vcolors(cc:4;chan:"RGBA";guid:"c4f6af8c-b1a2-4504-9cdb-bb248d033884";op:Multiply;lbl:"Blending Source";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list["6a81da"];clones:dict[6a81da=sp(name:"Blending Source_6a81da";imps:list[imp_mp_color(def:RGBA(1, 1, 1, 1);hdr:False;cc:4;chan:"RGBA";prop:"_BlendingSource_6a81da";md:"";custom:False;refs:"";guid:"784bbf03-c5d5-4972-9982-9016534cc5fb";op:Multiply;lbl:"Blending Source";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:True)];isClone:False)];customTextures:list[];codeInjection:codeInjection(injectedFiles:list[];mark:False);matLayers:list[]) */
/* TCP_HASH ea5310cbe48b0ff2a76a4d7ef018e415 */
