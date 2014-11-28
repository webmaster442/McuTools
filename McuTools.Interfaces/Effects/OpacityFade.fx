/// <description>A transition effect </description>
///
/// <summary>The amount(%) of the transition from first texture to the second texture. </summary>
/// <minValue>0</minValue>
/// <maxValue>100</maxValue>
/// <defaultValue>30</defaultValue>
float Progress : register(C0);

/// <minValue>0</minValue>
/// <maxValue>.1</maxValue>
/// <defaultValue>.05</defaultValue>
float fuzzyAmount : register(C4);

sampler2D Texture1 : register(s0);

struct VS_OUTPUT
{
    float4 Position  : POSITION;
    float4 Color     : COlOR;
    float2 UV        : TEXCOORD;
};

float4 LineReveal(float2 uv,float progress)
{
   float2 lineOrigin = float2(0, 0);
   float2 lineNormal = float2(1, 0);
   float2 lineOffset = float2(1, 1);
  float2 currentLineOrigin = lerp(lineOrigin, lineOffset, progress);
  float2 normLineNormal = normalize(lineNormal);
  float4 c2 = tex2D(Texture1, uv);
  float4 transparent = c2 * float4(0.001, 0.001, 0.001, 0);
  float distFromLine = dot(normLineNormal, uv-currentLineOrigin);
  float p = saturate((distFromLine + fuzzyAmount) / (2.0 * fuzzyAmount));
  return lerp(c2, transparent, p);
}

float4 main(VS_OUTPUT input) : COlOR
{
  return LineReveal(input.UV, Progress/100);
}
