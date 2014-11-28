 // http://www.silverlightshow.net/items/Book-Folding-effect-using-Pixel-Shader.aspx

sampler2D inputSampler : register(s0);
sampler2D input2: register(S1);

/// <summary>The Fold Amount, zero is no effect, 1 i full fold</summary>
/// <minValue>0</minValue>
/// <maxValue>1</maxValue>
/// <defaultValue>0.2</defaultValue>
 float Progress : register(c0); 
 float4 transform(float2 uv : TEXCOORD, float4 def, float FoldAmount) : COLOR 
 { 
		float right = 1 - FoldAmount; 
	 // transforming the curent point (uv) according to the new boundaries. 
		float2 tuv = float2((uv.x - FoldAmount) / (right - FoldAmount), uv.y);
 
	 float tx = tuv.x; 
	 if (tx > 0.5) 
	 { 
		tx = 1 - tx; 
	 }
	 float top = FoldAmount * tx; 
	 float bottom = 1 - top;         
	 if (uv.y >= top && uv.y <= bottom) 
	 { 
     //linear interpolation between 0 and 1 considering the angle of folding.  
		 float ty = lerp(0, 1, (tuv.y - top) / (bottom - top)); 
		// get the pixel from the transformed x and interpolated y. 
	   return tex2D(input2, float2(tuv.x, ty)); 
 } 
	return def; 
 } 
 
 float4 main(float2 uv : TEXCOORD) : COLOR  
 {
 float FoldAmount = Progress / 2;
	 float4 def = tex2D(inputSampler, uv); 
	 float right = 1 - FoldAmount; 
	 if(uv.x > FoldAmount && uv.x < right) 
	 { 
		return transform(uv, def, FoldAmount); 
	 } 
 
	 return def; 
 }