Shader "Lines With Tessellation" {
Properties {
	_Color ("Color", Color) = (1,0.5,0.5,1)
	_Subdivision ("Subdiv", Range(1,64)) = 16.0
	_Waves ("Waves", Range(3.0,40.0)) = 15.0
}
SubShader {
Pass {

CGPROGRAM
#pragma vertex VS
#pragma fragment PS
#pragma hull HS
#pragma domain DS

#include "UnityCG.cginc"

float _Subdivision;
float _Waves;
float4 _Color;


struct appdata
{
    float4 vertex : POSITION;
};

struct vs2hs
{
    float3 pos : POS;
};

struct hsConst
{
    float tessFactor[2] : SV_TessFactor;
};

struct hs2ds
{
    float3 pos : POS;
};

struct ds2ps
{
    float4 pos : SV_Position;
};

vs2hs VS (appdata v)
{
    vs2hs o;
    o.pos = v.vertex.xyz;
    return o;
}

hsConst HSConst (InputPatch<vs2hs, 2> I)
{
    hsConst o;
	o.tessFactor[0] = 1.0f;
	o.tessFactor[1] = _Subdivision;
    return o;
}

[domain("isoline")]
[partitioning("fractional_odd")]
[outputtopology("line")]
[patchconstantfunc("HSConst")]
[outputcontrolpoints(2)]
hs2ds HS (InputPatch<vs2hs, 2> I, uint id : SV_OutputControlPointID)
{
    hs2ds o;
    o.pos = I[id].pos;
    return o;
}


[domain("isoline")]
ds2ps DS (hsConst IC, const OutputPatch<hs2ds, 2> I, float2 uv : SV_DomainLocation)
{
    ds2ps o;
    float3 pos = lerp (I[0].pos, I[1].pos, uv.x);
    float len = length(I[0].pos - I[1].pos);
    pos.x += sin(uv.x*_Waves) * len * 0.1;
    o.pos = mul (UNITY_MATRIX_MVP, float4(pos,1.0));
    return o;
}


float4 PS (ds2ps I) : COLOR0
{
	return _Color;
}

ENDCG

}
}

Fallback "Line Shader"
}
