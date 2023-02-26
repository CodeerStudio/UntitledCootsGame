// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Leaves"
{
	Properties
	{
		_Texture("Texture", 2D) = "white" {}
		_Topcolor("Top color", Color) = (0,0,0,0)
		_Bottomcolor("Bottom color", Color) = (0,0,0,0)
		_Windspeed("Windspeed", Float) = 0
		_Cutoff( "Mask Clip Value", Float ) = 0.678
		_Noisescale("Noise scale", Float) = 0
		_Windforce("Wind force", Float) = 0
		_Minvertexpos("Min vertex pos", Float) = 0
		_Maxvertexpos("Max vertex pos", Float) = 0
		_Shadowcolor("Shadow color", Color) = (0,0,0,0)
		_Boost("Boost", Range( 1 , 2)) = 0
		_HueShiftMin("HueShiftMin", Float) = 0
		_HueShiftMax("HueShiftMax", Float) = 0
		_HueOffset("HueOffset", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TreeTransparentCutout"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float _Windspeed;
		uniform float _Noisescale;
		uniform float _Windforce;
		uniform sampler2D _Texture;
		uniform float4 _Texture_ST;
		uniform float _HueShiftMin;
		uniform float _HueShiftMax;
		uniform float _HueOffset;
		uniform float4 _Bottomcolor;
		uniform float4 _Topcolor;
		uniform float _Minvertexpos;
		uniform float _Maxvertexpos;
		uniform float _Boost;
		uniform float4 _Shadowcolor;
		uniform float _Cutoff = 0.678;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float mulTime6 = _Time.y * _Windspeed;
			float simplePerlin2D9 = snoise( ( ase_worldPos + mulTime6 ).xy*_Noisescale );
			simplePerlin2D9 = simplePerlin2D9*0.5 + 0.5;
			float3 windEffect11 = ( float3(1,0,0) * ( simplePerlin2D9 * _Windforce ) );
			v.vertex.xyz += windEffect11;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float2 uv_Texture = i.uv_texcoord * _Texture_ST.xy + _Texture_ST.zw;
			float4 MainLeafTexture54 = tex2D( _Texture, uv_Texture );
			float temp_output_18_0_g5 = _HueOffset;
			float2 appendResult17_g5 = (float2(temp_output_18_0_g5 , temp_output_18_0_g5));
			float4 transform1_g5 = mul(unity_WorldToObject,float4( 0,0,0,1 ));
			float dotResult4_g6 = dot( ( float4( appendResult17_g5, 0.0 , 0.0 ) + transform1_g5 ).xy , float2( 12.9898,78.233 ) );
			float lerpResult10_g6 = lerp( _HueShiftMin , _HueShiftMax , frac( ( sin( dotResult4_g6 ) * 43758.55 ) ));
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 lerpResult34 = lerp( _Bottomcolor , _Topcolor , (0.0 + (ase_vertex3Pos.y - _Minvertexpos) * (1.0 - 0.0) / (_Maxvertexpos - _Minvertexpos)));
			float3 hsvTorgb5_g5 = RGBToHSV( lerpResult34.rgb );
			float3 hsvTorgb7_g5 = HSVToRGB( float3(( lerpResult10_g6 + hsvTorgb5_g5.x ),hsvTorgb5_g5.y,hsvTorgb5_g5.z) );
			float4 Albedo52 = ( float4( hsvTorgb7_g5 , 0.0 ) * MainLeafTexture54 );
			float4 lerpResult78 = lerp( _Shadowcolor , float4( 1,1,1,1 ) , ase_lightAtten);
			c.rgb = ( saturate( ( Albedo52 * _Boost ) ) * lerpResult78 ).rgb;
			c.a = 1;
			clip( MainLeafTexture54.r - _Cutoff );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows novertexlights vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18301
199.5;274.5;1100;646;2383.633;439.437;2.34535;True;False
Node;AmplifyShaderEditor.RangedFloatNode;47;-816.9387,-739.7443;Inherit;False;Property;_Maxvertexpos;Max vertex pos;8;0;Create;True;0;0;False;0;False;0;1.91;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;35;-824.1726,-1024.751;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;46;-815.3337,-837.6161;Inherit;False;Property;_Minvertexpos;Min vertex pos;7;0;Create;True;0;0;False;0;False;0;-3.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-461.5936,-1163.745;Inherit;False;Property;_Topcolor;Top color;1;0;Create;True;0;0;False;0;False;0,0,0,0;1,0.6589699,0.2783018,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;32;-457.4055,-1366.224;Inherit;False;Property;_Bottomcolor;Bottom color;2;0;Create;True;0;0;False;0;False;0,0,0,0;1,0.4684191,0.3254716,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;45;-515.3003,-880.9362;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-620.6965,-99.27311;Inherit;True;Property;_Texture;Texture;0;0;Create;True;0;0;False;0;False;-1;None;5fff32a39832df940876a3547d8e8567;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;4;-94.7099,1679.53;Inherit;False;Property;_Windspeed;Windspeed;3;0;Create;True;0;0;False;0;False;0;0.06;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;102;215.1974,-634.5807;Inherit;False;Property;_HueShiftMin;HueShiftMin;11;0;Create;True;0;0;False;0;False;0;-0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;109;297.7839,-1048.931;Inherit;False;Property;_HueOffset;HueOffset;13;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;103;190.6675,-790.2068;Inherit;False;Property;_HueShiftMax;HueShiftMax;12;0;Create;True;0;0;False;0;False;0;0.04;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;34;-15.91538,-927.5483;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;54;-228.2083,-46.87245;Inherit;False;MainLeafTexture;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleTimeNode;6;131.972,1620.009;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;108;578.83,-934.662;Inherit;False;WorldPosHueShift;-1;;5;6a31b090e2b9e5b4597d94a814a3a788;0;4;18;FLOAT;0;False;9;COLOR;0,0,0,0;False;10;FLOAT;0;False;11;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;7;-3.690001,1144.628;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;55;970.7714,-521.3007;Inherit;False;54;MainLeafTexture;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;1310.554,-732.0372;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;10;81.32389,1409.897;Inherit;False;Property;_Noisescale;Noise scale;5;0;Create;True;0;0;False;0;False;0;11.96;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;30;469.492,1274.876;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;52;1595.759,-782.5361;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;9;622.8973,1392.421;Inherit;False;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;906.3165,1593.923;Inherit;False;Property;_Windforce;Wind force;6;0;Create;True;0;0;False;0;False;0;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;-1597.878,-181.1055;Inherit;False;52;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;1136.436,1448.376;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;48;1114.898,1190.429;Inherit;False;Constant;_Winddirection;Wind direction;9;0;Create;True;0;0;False;0;False;1,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;82;-1663.893,-38.5649;Inherit;False;Property;_Boost;Boost;10;0;Create;True;0;0;False;0;False;0;1.195;1;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;80;-1604.126,171.1844;Inherit;False;Property;_Shadowcolor;Shadow color;9;0;Create;True;0;0;False;0;False;0,0,0,0;0.8207547,0.7317106,0.7929284,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;1412.557,1328.473;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightAttenuation;74;-1536.189,669.6298;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-1325.846,-119.3574;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;11;1618.357,1395.52;Inherit;False;windEffect;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;83;-1105.102,-56.7205;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;78;-1242.893,319.8276;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,1;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-922.9211,203.51;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;12;-216.7679,351.2076;Inherit;False;11;windEffect;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;226.0719,-106.9792;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Leaves;False;False;False;False;False;True;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.678;True;True;0;True;TreeTransparentCutout;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;4;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;45;0;35;2
WireConnection;45;1;46;0
WireConnection;45;2;47;0
WireConnection;34;0;32;0
WireConnection;34;1;2;0
WireConnection;34;2;45;0
WireConnection;54;0;1;0
WireConnection;6;0;4;0
WireConnection;108;18;109;0
WireConnection;108;9;34;0
WireConnection;108;10;103;0
WireConnection;108;11;102;0
WireConnection;3;0;108;0
WireConnection;3;1;55;0
WireConnection;30;0;7;0
WireConnection;30;1;6;0
WireConnection;52;0;3;0
WireConnection;9;0;30;0
WireConnection;9;1;10;0
WireConnection;16;0;9;0
WireConnection;16;1;13;0
WireConnection;49;0;48;0
WireConnection;49;1;16;0
WireConnection;84;0;53;0
WireConnection;84;1;82;0
WireConnection;11;0;49;0
WireConnection;83;0;84;0
WireConnection;78;0;80;0
WireConnection;78;2;74;0
WireConnection;77;0;83;0
WireConnection;77;1;78;0
WireConnection;0;10;54;0
WireConnection;0;13;77;0
WireConnection;0;11;12;0
ASEEND*/
//CHKSM=7869636B0A5CE0CD65094DD63B1F34FD8381ABB1