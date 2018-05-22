Shader "outline"
{
	Properties
	{
		[HDR]_Color("_Color", Color) = (1,1,1,1)
		_Width("_Width", Float) = 0.4
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			float4 _Color;
			float _Width;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = v.vertex;
				o.vertex += float4(normalize(v.normal) * _Width, 0);
				o.vertex = UnityObjectToClipPos(o.vertex);
				return o;
			}


			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = _Color;
				return col;
			}
			ENDCG
		}
	}
}
