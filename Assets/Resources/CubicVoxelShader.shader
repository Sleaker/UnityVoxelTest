Shader "Custom/CubicVoxelShader" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Atlas32 ("Atlas", 2D) = "white" {}
	_ALPHA ("Blend Factor", Range(0, .57)) = .5
	_POWER ("Power", Range(0, 10)) = 4
	_PALLETESIZE ("Pallete Size", Float) = 3
	_PERCENT ("Percent Of Copy", Float) = .125
}
SubShader 
{
	LOD 200				
	CGPROGRAM
	#pragma target 3.0
	#pragma surface surf BlinnPhong vertex:vert nolightmap
	#pragma glsl

	#include "UnityCG.cginc"
	#pragma debug

	uniform sampler2D _Atlas32;	

	uniform float4 _Color;
	uniform float _ALPHA;
	uniform float _POWER;
	uniform float _PALLETESIZE;
	uniform float _PERCENT;

	struct Input
	{
		float4 position;				
		float3 normal;
		fixed4 material1;
		fixed4 material2;		
	};
	
	
	void vert (inout appdata_full v, out Input o)
	{

		UNITY_INITIALIZE_OUTPUT(Input,o);
		o.normal = v.normal;		
		o.position = v.vertex;	
		o.material1 = v.color;
		o.material2 = fixed4(v.texcoord.xy, v.texcoord1.xy);
	
	}
	
	void surf (Input IN, inout SurfaceOutput o)
	{
		float3 p = IN.position.xzy;
		float3 nrml = IN.normal.xzy;
		fixed4 material1 = IN.material1;
		fixed4 material2 = IN.material2;	
		float k = _PALLETESIZE;	
		float b = _PERCENT;

		float3 flip = float3(nrml.x < 0, nrml.y >= 0, nrml.z < 0);
		
		float3 blend = saturate(abs(normalize(nrml)) - _ALPHA);

		blend = pow(blend, float3(_POWER, _POWER, _POWER));
		blend /= dot(blend, float3(1, 1, 1));

		fixed2 zindex = lerp(material2.xz, material2.yw, flip.y);

		fixed4 matindex = round(fixed4(material1.xy * 255.0, zindex * 255.0));

		


		float3 tmp1, tmp2;

		tmp1.xy = frac(-p.xy);
		tmp2 = frac(p);
		tmp1 = lerp(tmp2.yxx, tmp1.yxx, flip);

		float kf = pow(2, ceil(log2(k * ( 1 + 2 * b))));		
		float bk = b / kf;

		tmp1 = (tmp1 / kf) + bk;

		tmp2 = (tmp2.zzy / kf) + bk; 

		float4 tmp4 = floor(matindex / k);
		float4 tmp3 = round(fmod(matindex, k));
		

		float4 tx1, tx2, tx3;
		float g = (1 + 2 * b) / kf;

		tx1.xy = tmp3.x * g + tmp1.xy;
		tx1.zw = tmp4.x * g + tmp2.xy;

		tx2.xy = tmp3.y * g + tmp1.xy;
		tx2.zw = tmp4.y * g + tmp2.xy;

		tx3.xy = tmp3.zw * g + tmp1.z;
		tx3.zw = tmp4.zw * g + tmp2.z;

		float3 temp = max(abs(ddx(p)), abs(ddy(p)));
		temp = max(temp.yxx, temp.zzy);

		float3 lambda = float3(log2(temp.x), log2(temp.y), log2(temp.z));

		//Primary xyz
		fixed4 x1 = tex2Dlod(_Atlas32, float4(tx1.xz, 0, lambda.x));
		fixed4 y1 = tex2Dlod(_Atlas32, float4(tx1.yw, 0, lambda.y));
		fixed4 z1 = tex2Dlod(_Atlas32, float4(tx3.xz, 0, lambda.z));

		//Secondary xyz
		fixed4 x2 = tex2Dlod(_Atlas32, float4(tx2.xz, 0, lambda.x));
		fixed4 y2 = tex2Dlod(_Atlas32, float4(tx2.yw, 0, lambda.y));
		fixed4 z2 = tex2Dlod(_Atlas32, float4(tx3.yw, 0, lambda.z));

		fixed4 color1 = x1 * blend.x + y1 * blend.y + z1 * blend.z;
		fixed4 color2 = x2 * blend.x + y2 * blend.y + z2 * blend.z;

		//o.Albedo = y1;

		o.Albedo = lerp(color1, color2, material1.z);


	}


	ENDCG
	
}

Fallback "VertexLit"
}
