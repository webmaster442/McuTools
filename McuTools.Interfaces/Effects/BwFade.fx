
/// <description>A transition effect </description>
/// <summary>The amount(%) of the transition from first texture to the second texture. </summary>
/// <minValue>0</minValue>
/// <maxValue>100</maxValue>
/// <defaultValue>0</defaultValue>
float Progress : register(C0);

sampler2D Texture1 : register(s0);

struct VS_OUTPUT
{
    float4 Position  : POSITION;
    float4 Color     : COlOR;
    float2 UV        : TEXCOORD;
};

float4 Circle(float2 uv,float progress)
{
  float radius = -0.11 + progress * 0.92;
  float fromCenter = length(uv - float2(0.5, 0.5));
  float distFromCircle = fromCenter - radius;

  float4 c2 = tex2D(Texture1, uv);
  float4 c1 = dot(c2.rgb, float3(0.3, 0.4, 0.3));
  c1.a = c2.a;

  float p = saturate((distFromCircle + 0.11) / 0.22);
  return lerp(c2, c1, p);
}

float4 main(VS_OUTPUT input) : COlOR
{
  return Circle(input.UV, Progress/100);
}
