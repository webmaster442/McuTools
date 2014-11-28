/// <description>An effect that pivots the output around a center point.</description>
sampler2D inputSampler : register(s0);
sampler2D input2: register(S1);

 
 /// <summary>Animation Progress</summary>
/// <minValue>0</minValue>
/// <maxValue>1</maxValue>
/// <defaultValue>0</defaultValue>
 float Progress: register(c0);

 /// <summary>The Left </summary>
/// <minValue>0</minValue>
/// <maxValue>0,5</maxValue>
/// <defaultValue>0</defaultValue>
 float edge : register(c1);
 
 float4 transform(float2 uv : TEXCOORD, float pivotAmount, float4 def) : COLOR 
 {

		float right = 1 - pivotAmount; 
	 // transforming the curent point (uv) according to the new boundaries. 
		float2 tuv = float2((uv.x - pivotAmount) / (right - pivotAmount), uv.y);
 
   float tx = tuv.x -edge; 
   if (tx > edge) 
   { 
    tx = 1 - tx; 
   }
	float top = pivotAmount * tx; 
  // float top = 1;
	 float bottom = 1 - top;    
		 float ty = lerp(0, 1, (tuv.y - top ) / (bottom - top)); 	 
	 if (uv.y >= top && uv.y <= bottom) 
	 { 
     //linear interpolation between 0 and 1 considering the angle of folding.  
		// get the pixel from the transformed x and interpolated y. 
	   return tex2D(input2, float2(tuv.x , ty)); 
 } 
	return def;
 } 
 
 float4 main(float2 uv : TEXCOORD) : COLOR  
 {
	 	float pivotAmount = Progress / 2;
float4 def = tex2D(inputSampler, uv);	 	
	 float right = 1 - pivotAmount; 
	 if(uv.x > pivotAmount && uv.x < right) 
	 { 
		return transform(uv, pivotAmount, def); 
	 }
return def;  	 
 
	 return 0; 
 }