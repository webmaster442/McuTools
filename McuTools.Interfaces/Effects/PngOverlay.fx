sampler2D input : register(s0);

// new HLSL shader
// modify the comment parameters to reflect your shader parameters

/// <summary>Fill color</summary>
float4 InColor : register(C0);

float4 main(float2 uv : TEXCOORD) : COLOR 
{ 
	
	float4 sampled = tex2D( input , uv.xy);
	if(InColor.a == 0 ) return sampled;	
	if(sampled.a == 0) return float4(0, 0, 0, 0);
	float4 inverse = float4(1, 1, 1, 1) - sampled;
	float4 final = inverse * InColor;
	final.a = sampled.a;
	return final;
	
}