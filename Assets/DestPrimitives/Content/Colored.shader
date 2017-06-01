Shader "Dest/Colored" 
{
	Properties
	{
		_Color ("Main Color", COLOR) = (1,1,1,1)
	}
	
	SubShader
	{
		Color [_Color]
		Pass
		{
			ZTest LEqual
		}
	} 
}
