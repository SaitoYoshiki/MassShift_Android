Shader "Custom/Water" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Amp("Amp", Float) = 0.02
		_Hz("Hz", Float) = 20
		_Cycle("Cycle", Float) = 1
	}
	SubShader {
		Tags {
			"Queue" = "TransParent"
			"RenderType"="Transparent"
		}
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert alpha
		#pragma target 3.0

		fixed4 _Color;
		float _Amp;
		float _Hz;
		float _Cycle;

		struct Input {
			float2 uv_MainTex;
		};

		void vert(inout appdata_full v, out Input o){
			UNITY_INITIALIZE_OUTPUT(Input, o);
			float amp = _Amp * sin(_Time*_Hz + v.vertex.x / _Cycle);
			v.vertex.xyz = float3(v.vertex.x, v.vertex.y + amp, v.vertex.z);
		}

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}