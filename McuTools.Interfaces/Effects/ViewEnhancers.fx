sampler2D input : register(s0);

/// <summary>0 - No preprocess, 1 - Grayscale, 2 - Invert, 3 - Invert Grayscale</summary>
/// <minValue>0/minValue>
/// <maxValue>4</maxValue>
/// <defaultValue>0</defaultValue>
float PreProcess : register(C0);

/// <summary>The brightness offset.</summary>
/// <minValue>-1</minValue>
/// <maxValue>1</maxValue>
/// <defaultValue>0</defaultValue>
float Brightness : register(C1);

/// <summary>The contrast multiplier.</summary>
/// <minValue>0</minValue>
/// <maxValue>2</maxValue>
/// <defaultValue>1</defaultValue>
float Contrast : register(C2);

/// <summary>The center point of the magnified region.</summary>
/// <minValue>0,0</minValue>
/// <maxValue>1,1</maxValue>
/// <defaultValue>0.5,0.5</defaultValue>
float2 CenterPoint : register(C3);

/// <summary>The magnification factor.</summary>
/// <minValue>1</minValue>
/// <maxValue>5</maxValue>
/// <defaultValue>1</defaultValue>
float MagnificationAmount : register(C4);


/// <summary>The amount of sharpening.</summary>
/// <minValue>0</minValue>
/// <maxValue>2</maxValue>
/// <defaultValue>1</defaultValue>
float Amount : register(C5);


/// <summary>The size of the input (in pixels).</summary>
/// <type>Size</type>
/// <minValue>1,1</minValue>
/// <maxValue>1000,1000</maxValue>
/// <defaultValue>640,480</defaultValue>
float2 InputSize : register(C6);

/// <summary>Color Overlay</summary>
float4 ColorOverlay: register(C7);

//-----------------------------------------------------------------------------
// Input preprocessing
//-----------------------------------------------------------------------------
float4 Preproc(float4 input)
{
	if(PreProcess == 0) return input;
	else if (PreProcess == 1) return dot(input, float3(0.3, 0.59, 0.11));
	else if (PreProcess == 2) return float4(1, 1, 1, 1) - input;
	else if (PreProcess == 3)  return float4(1, 1, 1, 1) - dot(input, float3(0.3, 0.59, 0.11));
	else return input * ColorOverlay;     	
}

//-----------------------------------------------------------------------------
// Brightness & Contrast
//-----------------------------------------------------------------------------
float4 BrightnessContrast(float4 pixelColor)
{
	pixelColor.rgb /= pixelColor.a;
	// Apply contrast.
	pixelColor.rgb = ((pixelColor.rgb - 0.5f) * max(Contrast, 0)) + 0.5f;   
    // Apply brightness.
	pixelColor.rgb += Brightness;
	// Return final pixel color.
	pixelColor.rgb *= pixelColor.a;
	return pixelColor;
}

//-----------------------------------------------------------------------------
// Zoom & Sharpening
//-----------------------------------------------------------------------------
float4 ZoomSharp(float2 uv)
{
	float2 centerToPixel = uv - CenterPoint;
	float dist = length(centerToPixel / float2(1, 1.5));
	float2 samplePoint = CenterPoint + centerToPixel / MagnificationAmount;
	
	float2 offset = 1 / InputSize;
	float4 color = tex2D(input, samplePoint);
	
	color.rgb += tex2D(input, samplePoint - offset) * Amount;
	color.rgb -= tex2D(input, samplePoint + offset) * Amount;
	
	return color;
}

//-----------------------------------------------------------------------------
// Main
//-----------------------------------------------------------------------------
float4 main(float2 uv : TEXCOORD) : COLOR 
{
	float4 inputcolor = ZoomSharp(uv);
	float4 temp = Preproc(inputcolor);
	temp.a = 1;
	temp = BrightnessContrast(temp);
	return temp;
}