Shader "Line Shader" {
Properties {
	_Color ("Color", Color) = (1,0.5,0.5,1)
}
SubShader {
	Pass {
		BindChannels { Bind "vertex", vertex }
		SetTexture [_MainTex] { constantColor [_Color] combine constant }
	}
}
}
